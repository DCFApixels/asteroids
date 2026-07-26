using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

public sealed class CRTPostEffecter : ScriptableRendererFeature
{
    private const int ShaderPassIndex = 0;

    [Header("Render")]
    public Material material;
    public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    public bool applyInSceneView = true;

    [Header("White Noise Burst")]
    public int whiteNoiseFrequency = 1;
    public float whiteNoiseLength = 0.1f;

    [Header("Screen Jump")]
    public int screenJumpFrequency = 1;
    public float screenJumpLength = 0.2f;
    public float screenJumpMinLevel = 0.1f;
    public float screenJumpMaxLevel = 0.9f;

    [Header("Slippage")]
    public bool isSlippageNoise = true;
    public float slippageStrength = 0.005f;
    public float slippageInterval = 1f;
    public float slippageScrollSpeed = 33f;
    public float slippageSize = 11f;

    [Header("Low Resolution")]
    public bool isLowResolution = true;
    public Vector2Int resolutions;
    public FilterMode lowResolutionFilterMode = FilterMode.Point;

    private CRTPostPass _pass;

    public override void Create()
    {
        _pass ??= new CRTPostPass();
        _pass.renderPassEvent = renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (material == null || material.shader == null)
        {
            Debug.LogWarning($"{nameof(CRTPostEffecter)} skipped: material is missing.");
            return;
        }

        if (material.passCount <= ShaderPassIndex)
        {
            Debug.LogWarning($"{nameof(CRTPostEffecter)} skipped: material has no shader pass {ShaderPassIndex}.");
            return;
        }

        CameraData cameraData = renderingData.cameraData;
        if (cameraData.cameraType == CameraType.Preview || cameraData.cameraType == CameraType.Reflection)
        {
            return;
        }

        if (!applyInSceneView && cameraData.isSceneViewCamera)
        {
            return;
        }

        _pass.renderPassEvent = renderPassEvent;
        _pass.Setup(this, cameraData.cameraTargetDescriptor);
        renderer.EnqueuePass(_pass);
    }

    protected override void Dispose(bool disposing)
    {
        _pass?.Dispose();
        _pass = null;
    }

    private sealed class CRTPostPass : ScriptableRenderPass
    {
        private const string PassName = "CRT Post Effect";
        private const string FullResolutionTextureName = "_CRTPostEffectTexture";
        private const string LowResolutionTextureName = "_CRTPostEffectLowResolutionTexture";

        private static readonly int ScreenJumpLevel = Shader.PropertyToID("_ScreenJumpLevel");
        private static readonly int WhiteNoiseGate = Shader.PropertyToID("_WhiteNoiseGate");
        private static readonly int SlippageStrength = Shader.PropertyToID("_SlippageStrength");
        private static readonly int SlippageSize = Shader.PropertyToID("_SlippageSize");
        private static readonly int SlippageInterval = Shader.PropertyToID("_SlippageInterval");
        private static readonly int SlippageScrollSpeed = Shader.PropertyToID("_SlippageScrollSpeed");
        private static readonly int SlippageNoiseOnOff = Shader.PropertyToID("_SlippageNoiseOnOff");

        private readonly ProfilingSampler _profilingSampler = new(PassName);

        private Material _sourceMaterial;
        private Material _runtimeMaterial;
        private RTHandle _fullResolutionHandle;
        private RTHandle _lowResolutionHandle;

        private bool _isLowResolution;
        private Vector2Int _resolutionOverride;
        private FilterMode _lowResolutionFilterMode;
        private float _whiteNoiseTimeLeft;
        private float _screenJumpTimeLeft;
        private float _currentScreenJumpLevel;

        public void Setup(CRTPostEffecter settings, RenderTextureDescriptor cameraTextureDescriptor)
        {
            requiresIntermediateTexture = true;

            _isLowResolution = settings.isLowResolution;
            _resolutionOverride = settings.resolutions;
            _lowResolutionFilterMode = settings.lowResolutionFilterMode;

            UpdateRuntimeMaterial(settings.material);
            UpdateRuntimeProperties(settings, cameraTextureDescriptor);
        }

        private void UpdateRuntimeMaterial(Material sourceMaterial)
        {
            if (_sourceMaterial != sourceMaterial || _runtimeMaterial == null)
            {
                CoreUtils.Destroy(_runtimeMaterial);
                _sourceMaterial = sourceMaterial;
                _runtimeMaterial = CoreUtils.CreateEngineMaterial(sourceMaterial.shader);
            }

            _runtimeMaterial.CopyPropertiesFromMaterial(sourceMaterial);
        }

        private void UpdateRuntimeProperties(CRTPostEffecter settings, RenderTextureDescriptor cameraTextureDescriptor)
        {
            float deltaTime = Application.isPlaying ? Time.unscaledDeltaTime : 1f / 60f;

            _whiteNoiseTimeLeft = Mathf.Max(0f, _whiteNoiseTimeLeft - deltaTime);
            if (_whiteNoiseTimeLeft <= 0f && ShouldStartBurst(settings.whiteNoiseFrequency))
            {
                _whiteNoiseTimeLeft = Mathf.Max(0f, settings.whiteNoiseLength);
            }

            float screenJumpLevel = 0f;
            _screenJumpTimeLeft = Mathf.Max(0f, _screenJumpTimeLeft - deltaTime);
            if (settings.screenJumpFrequency > 0)
            {
                if (_screenJumpTimeLeft <= 0f && ShouldStartBurst(settings.screenJumpFrequency))
                {
                    _currentScreenJumpLevel = Random.Range(settings.screenJumpMinLevel, settings.screenJumpMaxLevel);
                    screenJumpLevel = _currentScreenJumpLevel;
                    _screenJumpTimeLeft = Mathf.Max(0f, settings.screenJumpLength);
                }
                else if (_screenJumpTimeLeft > 0f)
                {
                    screenJumpLevel = _currentScreenJumpLevel;
                }
                else
                {
                    _currentScreenJumpLevel = 0f;
                }
            }

            _runtimeMaterial.SetFloat(WhiteNoiseGate, _whiteNoiseTimeLeft > 0f || settings.whiteNoiseFrequency <= 0 ? 1f : 0f);
            _runtimeMaterial.SetFloat(ScreenJumpLevel, screenJumpLevel);
            _runtimeMaterial.SetFloat(SlippageInterval, settings.slippageInterval);
            _runtimeMaterial.SetFloat(SlippageNoiseOnOff, settings.isSlippageNoise ? Random.value : 1f);
            _runtimeMaterial.SetFloat(SlippageScrollSpeed, settings.slippageScrollSpeed);
            _runtimeMaterial.SetFloat(SlippageStrength, settings.slippageStrength);
            _runtimeMaterial.SetFloat(SlippageSize, settings.slippageSize);

            _resolutionOverride.x = Mathf.Min(_resolutionOverride.x, cameraTextureDescriptor.width);
            _resolutionOverride.y = Mathf.Min(_resolutionOverride.y, cameraTextureDescriptor.height);
        }

        private static bool ShouldStartBurst(int frequency)
        {
            return frequency > 0 && Random.Range(0, 1000) < frequency;
        }

#pragma warning disable 618, 672
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (_runtimeMaterial == null)
            {
                return;
            }

            RTHandle source = renderingData.cameraData.renderer.cameraColorTargetHandle;
            RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.depthBufferBits = 0;
            descriptor.depthStencilFormat = GraphicsFormat.None;
            descriptor.msaaSamples = 1;

            CommandBuffer cmd = CommandBufferPool.Get(PassName);
            using (new ProfilingScope(cmd, _profilingSampler))
            {
                if (_isLowResolution)
                {
                    RenderTextureDescriptor lowResolutionDescriptor = GetLowResolutionDescriptor(descriptor);
                    RenderingUtils.ReAllocateHandleIfNeeded(
                        ref _lowResolutionHandle,
                        lowResolutionDescriptor,
                        _lowResolutionFilterMode,
                        TextureWrapMode.Clamp,
                        name: LowResolutionTextureName);

                    Blitter.BlitCameraTexture(cmd, source, _lowResolutionHandle, 0f, false);
                    Blitter.BlitCameraTexture(cmd, _lowResolutionHandle, source, _runtimeMaterial, ShaderPassIndex);
                }
                else
                {
                    RenderingUtils.ReAllocateHandleIfNeeded(
                        ref _fullResolutionHandle,
                        descriptor,
                        FilterMode.Bilinear,
                        TextureWrapMode.Clamp,
                        name: FullResolutionTextureName);

                    Blitter.BlitCameraTexture(cmd, source, _fullResolutionHandle, _runtimeMaterial, ShaderPassIndex);
                    Blitter.BlitCameraTexture(cmd, _fullResolutionHandle, source, 0f, false);
                }
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
#pragma warning restore 618, 672

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (_runtimeMaterial == null)
            {
                return;
            }

            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            if (resourceData.isActiveTargetBackBuffer)
            {
                return;
            }

            TextureHandle source = resourceData.activeColorTexture;
            if (!source.IsValid())
            {
                return;
            }

            TextureHandle effectSource = source;
            if (_isLowResolution)
            {
                TextureDesc lowResolutionDescriptor = GetLowResolutionTextureDesc(renderGraph.GetTextureDesc(source));
                TextureHandle lowResolutionTexture = renderGraph.CreateTexture(lowResolutionDescriptor);

                RenderGraphUtils.BlitMaterialParameters downsampleParameters =
                    new(source, lowResolutionTexture, Blitter.GetBlitMaterial(TextureDimension.Tex2D), 0);
                renderGraph.AddBlitPass(downsampleParameters, $"{PassName} Downsample");

                effectSource = lowResolutionTexture;
            }

            TextureDesc destinationDescriptor = renderGraph.GetTextureDesc(source);
            destinationDescriptor.name = FullResolutionTextureName;
            destinationDescriptor.clearBuffer = false;
            destinationDescriptor.depthBufferBits = DepthBits.None;
            destinationDescriptor.msaaSamples = MSAASamples.None;
            TextureHandle destination = renderGraph.CreateTexture(destinationDescriptor);

            RenderGraphUtils.BlitMaterialParameters effectParameters =
                new(effectSource, destination, _runtimeMaterial, ShaderPassIndex);
            renderGraph.AddBlitPass(effectParameters, PassName);

            resourceData.cameraColor = destination;
        }

        private RenderTextureDescriptor GetLowResolutionDescriptor(RenderTextureDescriptor sourceDescriptor)
        {
            if (_resolutionOverride.x > 0 && _resolutionOverride.y > 0)
            {
                sourceDescriptor.width = _resolutionOverride.x;
                sourceDescriptor.height = _resolutionOverride.y;
            }
            else
            {
                sourceDescriptor.width = Mathf.Max(1, sourceDescriptor.width / 2);
                sourceDescriptor.height = Mathf.Max(1, sourceDescriptor.height / 2);
            }

            sourceDescriptor.depthBufferBits = 0;
            sourceDescriptor.depthStencilFormat = GraphicsFormat.None;
            sourceDescriptor.msaaSamples = 1;
            return sourceDescriptor;
        }

        private TextureDesc GetLowResolutionTextureDesc(TextureDesc sourceDescriptor)
        {
            sourceDescriptor.name = LowResolutionTextureName;
            sourceDescriptor.clearBuffer = false;
            sourceDescriptor.depthBufferBits = DepthBits.None;
            sourceDescriptor.msaaSamples = MSAASamples.None;
            sourceDescriptor.filterMode = _lowResolutionFilterMode;
            sourceDescriptor.wrapMode = TextureWrapMode.Clamp;

            if (_resolutionOverride.x > 0 && _resolutionOverride.y > 0)
            {
                sourceDescriptor.sizeMode = TextureSizeMode.Explicit;
                sourceDescriptor.width = _resolutionOverride.x;
                sourceDescriptor.height = _resolutionOverride.y;
            }
            else if (sourceDescriptor.sizeMode == TextureSizeMode.Explicit)
            {
                sourceDescriptor.width = Mathf.Max(1, sourceDescriptor.width / 2);
                sourceDescriptor.height = Mathf.Max(1, sourceDescriptor.height / 2);
            }
            else
            {
                sourceDescriptor.sizeMode = TextureSizeMode.Scale;
                sourceDescriptor.scale *= 0.5f;
            }

            return sourceDescriptor;
        }

        public void Dispose()
        {
            CoreUtils.Destroy(_runtimeMaterial);
            _runtimeMaterial = null;
            _sourceMaterial = null;

            _fullResolutionHandle?.Release();
            _fullResolutionHandle = null;

            _lowResolutionHandle?.Release();
            _lowResolutionHandle = null;
        }
    }
}
