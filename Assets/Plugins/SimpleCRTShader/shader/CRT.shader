Shader "Simple CRT"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        [Toggle] _Scanline("Scanline", Float) = 0
        _ScanlineIntensity ("Scanline Intensity", Float) = 0.15
        _ScanlineSpeed ("Scanline Speed", Float) = 1
        [Toggle] _Monochorome("Monochorome", Float) = 0
        _MonochoromeIntensity ("Monochorome Intensity", Float) = 0.5
        [Toggle] _WhiteNoise("White Noise", Float) = 0
        _WhiteNoiseIntensity("White Noise Intensity", Float) = 1
        [HideInInspector] _WhiteNoiseGate("White Noise Gate", Float) = 1
        [Toggle] _ScreenJump("Screen Jump", Float) = 0
        _ScreenJumpLevel("Screen Jump Level", Float) = 1
        [Toggle] _Flickering("Flickering", Float) = 0
        _FlickeringStrength("Flickering Strength", Float) = 1
        _FlickeringCycle("Flickering Cycle", Float) = 1
        [Toggle] _Slippage("Slippage", Float) = 0
        _SlippageStrength ("Slippage Strength", Float) = 0
        _SlippageInterval ("Slippage Interval", Float) = 0
        _SlippageScrollSpeed ("Slippage ScrollSpeed", Float) = 0
        [HideInInspector] _SlippageNoiseOnOff ("Slippage Noise OnOff", Float) = 1
        _SlippageSize ("Slippage Size", Float) = 0
        [Toggle] _ChromaticAberration("Chromatic Aberration", Float) = 0
        _ChromaticAberrationIntensity("Chromatic Aberration Intensity", Float) = 1
        _ChromaticAberrationStrength("Chromatic Aberration Strength", Float) = 0.003
        _ChromaticAberrationStrengthMin("Chromatic Aberration Strength Min", Float) = 0
        _ChromaticAberrationStrengthPolar("Chromatic Aberration Strength Polar", Float) = 0
        [Toggle] _MultipleGhost("Multiple Ghost", Float) = 0
        _MultipleGhostStrength("Multiple Ghost Strength", Float) = 0
        _MultipleGhostIntensity("Multiple Ghost Intensity", Float) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }

        ZWrite Off
        ZTest Always
        Cull Off

        Pass
        {
            Name "CRTPostProcess"

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_instancing
            #pragma shader_feature_local _WHITENOISE_ON
            #pragma shader_feature_local _SCANLINE_ON
            #pragma shader_feature_local _MONOCHOROME_ON
            #pragma shader_feature_local _SCREENJUMP_ON
            #pragma shader_feature_local _CHROMATICABERRATION_ON
            #pragma shader_feature_local _MULTIPLEGHOST_ON
            #pragma shader_feature_local _FLICKERING_ON
            #pragma shader_feature_local _SLIPPAGE_ON

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _MainTex_TexelSize;
                float _WhiteNoiseIntensity;
                float _WhiteNoiseGate;
                float _ScanlineIntensity;
                float _ScanlineSpeed;
                float _MonochoromeIntensity;
                float _ScreenJumpLevel;
                float _FlickeringStrength;
                float _FlickeringCycle;
                float _SlippageStrength;
                float _SlippageInterval;
                float _SlippageScrollSpeed;
                float _SlippageNoiseOnOff;
                float _SlippageSize;
                float _ChromaticAberrationIntensity;
                float _ChromaticAberrationStrength;
                float _ChromaticAberrationStrengthMin;
                float _ChromaticAberrationStrengthPolar;
                float _MultipleGhostStrength;
                float _MultipleGhostIntensity;
            CBUFFER_END

            float GetRandom(float x)
            {
                return frac(sin(dot(float2(x, x), float2(12.9898, 78.233))) * 43758.5453);
            }

            float2 PolarCoordinates(float2 uv, float2 center, float radialScale, float lengthScale)
            {
                float2 delta = uv - center;
                float radius = length(delta) * 2.0 * radialScale;
                float angle = atan2(delta.x, delta.y) * 1.0 / 6.28 * lengthScale;
                return float2(radius, angle);
            }

            half4 SampleSource(float2 uv)
            {
                return SAMPLE_TEXTURE2D_X_LOD(_BlitTexture, sampler_LinearRepeat, uv, _BlitMipLevel);
            }

            half4 Frag(Varyings input) : SV_Target0
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float2 uv = input.texcoord.xy;

#if _SCREENJUMP_ON
                uv.y = frac(uv.y + _ScreenJumpLevel);
#endif

#if _FLICKERING_ON
                float flickeringNoise = GetRandom(_Time.y);
                float flickeringMask = pow(abs(sin(input.texcoord.y * _FlickeringCycle + _Time.y)), 10.0);
                uv.x = uv.x + (flickeringNoise * _FlickeringStrength * flickeringMask);
#endif

#if _SLIPPAGE_ON
                float scrollSpeed = _Time.x * _SlippageScrollSpeed;
                float slippageMask = pow(abs(sin(input.texcoord.y * _SlippageInterval + scrollSpeed)), _SlippageSize);
                float stepMask = round(sin(input.texcoord.y * _SlippageInterval + scrollSpeed - 1.0));
                uv.x = uv.x + (_SlippageNoiseOnOff * _SlippageStrength * slippageMask * stepMask);
#endif

                half4 color = SampleSource(float2(uv.x, uv.y));

#if _CHROMATICABERRATION_ON
                float polar = pow(PolarCoordinates(input.texcoord, float2(0.5, 0.5), 1.0, 1.0).r, _ChromaticAberrationStrengthPolar);
                float polarScale = lerp(_ChromaticAberrationStrengthMin, _ChromaticAberrationStrength, polar);

                half red = SampleSource(float2(uv.x - polarScale, uv.y)).r;
                half green = SampleSource(float2(uv.x, uv.y)).g;
                half blue = SampleSource(float2(uv.x + polarScale, uv.y)).b;
                color.rgb = lerp(color.rgb, half3(red, green, blue), _ChromaticAberrationIntensity);
#endif

#if _MULTIPLEGHOST_ON
                half3 ghost1st = SampleSource(uv - float2(1.0, 0.0) * _MultipleGhostStrength).rgb;
                half3 ghost2nd = SampleSource(uv - float2(1.0, 0.0) * _MultipleGhostStrength * 2.0).rgb;
                color.rgb = lerp(color.rgb, (color.rgb * 0.8 + ghost1st * 0.15 + ghost2nd * 0.05), _MultipleGhostIntensity);
#endif

#if _WHITENOISE_ON
                half whiteNoise = frac(sin(dot(input.texcoord, float2(12.9898, 78.233)) + _Time.x) * 43758.5453);
                color.rgb = lerp(color.rgb, whiteNoise.xxx, _WhiteNoiseIntensity * _WhiteNoiseGate);
#endif

#if _SCANLINE_ON
                half scanline = sin((input.texcoord.y + _Time.x * _ScanlineSpeed) * 800.0) * 0.04 * _ScanlineIntensity;
                color -= scanline;
#endif

#if _MONOCHOROME_ON
                half mc = 0.299 * color.r + 0.587 * color.g + 0.114 * color.b;
                color.rgb = lerp(color.rgb, mc.xxx, _MonochoromeIntensity);
#endif

                return color;
            }
            ENDHLSL
        }
    }
}
