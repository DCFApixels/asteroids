using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class CRTPostEffecter : MonoBehaviour
{
    public Material material;
    private Material _checkMaterialRef;
    private Material _materialClone;

    public int whiteNoiseFrequency = 1;
    public float whiteNoiseLength = 0.1f;
    private float _whiteNoiseTimeLeft;

    public int screenJumpFrequency = 1;
    public float screenJumpLength = 0.2f;
    public float screenJumpMinLevel = 0.1f;
    public float screenJumpMaxLevel = 0.9f;
    private float _screenJumpTimeLeft;

    public float flickeringStrength = 0.002f;
    public float flickeringCycle = 111f;

    public bool isSlippageNoise = true;
    public float slippageStrength = 0.005f;
    public float slippageInterval = 1f;
    public float slippageScrollSpeed = 33f;
    public float slippageSize = 11f;

    public float chromaticAberrationStrength = 0.005f;

    public float multipleGhostStrength = 0.01f;

    public bool isLowResolution = true;
    public Vector2Int resolutions;

    #region Properties in shader
    private static readonly int _ScreenJumpLevel = Shader.PropertyToID("_ScreenJumpLevel");
    private static readonly int _SlippageStrength = Shader.PropertyToID("_SlippageStrength");
    private static readonly int _SlippageSize = Shader.PropertyToID("_SlippageSize");
    private static readonly int _SlippageInterval = Shader.PropertyToID("_SlippageInterval");
    private static readonly int _SlippageScrollSpeed = Shader.PropertyToID("_SlippageScrollSpeed");
    private static readonly int _SlippageNoiseOnOff = Shader.PropertyToID("_SlippageNoiseOnOff");
    #endregion


    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        ///////White noise
        _whiteNoiseTimeLeft -= 0.01f;
        if (_whiteNoiseTimeLeft <= 0)
        {
            if (Random.Range(0, 1000) < whiteNoiseFrequency)
            {
                _whiteNoiseTimeLeft = whiteNoiseLength;
            }
        }
        //////

        //////Slippage
        material.SetFloat(_SlippageInterval, slippageInterval);
        material.SetFloat(_SlippageNoiseOnOff, isSlippageNoise ? Random.Range(0, 1f) : 1);
        material.SetFloat(_SlippageScrollSpeed, slippageScrollSpeed);
        material.SetFloat(_SlippageStrength, slippageStrength); 
        material.SetFloat(_SlippageSize, slippageSize);
        //////
        
        //////Screen Jump Noise
        if(screenJumpFrequency > 0)
        {
            _screenJumpTimeLeft -= 0.01f;
            if (_screenJumpTimeLeft <= 0)
            {
                if (Random.Range(0, 1000) < screenJumpFrequency)
                {
                    var level = Random.Range(screenJumpMinLevel, screenJumpMaxLevel);
                    material.SetFloat(_ScreenJumpLevel, level);
                    _screenJumpTimeLeft = screenJumpLength;
                }
                else
                {
                    material.SetFloat(_ScreenJumpLevel, 0);
                }
            }
        }
        //////

        //////Low resolution
        if (isLowResolution)
        {
            var target = RenderTexture.GetTemporary(src.width / 2, src.height / 2);
            Graphics.Blit(src, target);
            Graphics.Blit(target, dest, material);
            RenderTexture.ReleaseTemporary(target);
        }
        else
        {
            Graphics.Blit(src, dest, material);
        }
        //////

    }
}