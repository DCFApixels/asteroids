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
        [Toggle] _ScreenJump("Screen Jump", Float) = 0
        _ScreenJumpLevel("Screen Jump Level", Float) = 1
        [Toggle] _Flickering("Flickering", Float) = 0
        _FlickeringStrength("Flickering Strength", Float) = 1
        _FlickeringCycle("Flickering Cycle", Float) = 1
        [Toggle] _Slippage("Slippage", Float) = 0
        _SlippageStrength ("Slippage Strength", Float) = 0
        _SlippageInterval ("Slippage Interval", Float) = 0
        _SlippageScrollSpeed ("Slippage ScrollSpeed", Float) = 0
        //_SlippageNoiseOnOff ("Slippage Noise OnOff", Float) = 0
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
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #include "UnityCG.cginc"

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma shader_feature_local _WHITENOISE_ON
            #pragma shader_feature_local _SCANLINE_ON
            #pragma shader_feature_local _MONOCHOROME_ON
            #pragma shader_feature_local _SCREENJUMP_ON
            #pragma shader_feature_local _CHROMATICABERRATION_ON
            #pragma shader_feature_local _MULTIPLEGHOST_ON
            #pragma shader_feature_local _FLICKERING_ON
            #pragma shader_feature_local _SLIPPAGE_ON
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;

#if _WHITENOISE_ON
            float _WhiteNoiseIntensity;
#endif

#if _SCANLINE_ON
            float _ScanlineIntensity;
            float _ScanlineSpeed;
#endif
#if _MONOCHOROME_ON
            float _MonochoromeIntensity;
#endif

            int _LetterBoxOnOff;
            int _LetterBoxEdgeBlur;
            int _LetterBoxType;

#if _SCREENJUMP_ON
            float _ScreenJumpLevel;
#endif
            
#if _FLICKERING_ON
            float _FlickeringStrength;
            float _FlickeringCycle;
#endif
            
#if _SLIPPAGE_ON
            float _SlippageStrength;
            float _SlippageInterval;
            float _SlippageScrollSpeed;
            float _SlippageNoiseOnOff;
            float _SlippageSize;
#endif

#if _CHROMATICABERRATION_ON
            float _ChromaticAberrationIntensity;
            float _ChromaticAberrationStrength;
            float _ChromaticAberrationStrengthMin;
            float _ChromaticAberrationStrengthPolar;
#endif

#if _MULTIPLEGHOST_ON
            float _MultipleGhostStrength;
            float _MultipleGhostIntensity;
#endif

            float GetRandom(float x)
            {
                return frac(sin(dot(x, float2(12.9898, 78.233))) * 43758.5453);
            }
            float EaseIn(float t0, float t1, float t)
            {
                return 2.0 * smoothstep(t0, 2.0 * t1 - t0, t);
            }
            float2 polarCoordinates(float2 UV, float2 Center, float RadialScale, float LengthScale)
            {
                float2 delta = UV - Center;
                float radius = length(delta) * 2 * RadialScale;
                float angle = atan2(delta.x, delta.y) * 1.0/6.28 * LengthScale;
                return float2(radius, angle);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

#if _SCREENJUMP_ON
                uv.y = frac(uv.y + _ScreenJumpLevel);
#endif

#if _FLICKERING_ON
                float flickeringNoise = GetRandom(_Time.y);
                float flickeringMask = pow(abs(sin(i.uv.y * _FlickeringCycle + _Time.y)), 10);
                uv.x = uv.x + (flickeringNoise * _FlickeringStrength * flickeringMask); 
#endif

#if _SLIPPAGE_ON
                float scrollSpeed = _Time.x * _SlippageScrollSpeed;
                float slippageMask = pow(abs(sin(i.uv.y * _SlippageInterval + scrollSpeed)), _SlippageSize);
                float stepMask = round(sin(i.uv.y * _SlippageInterval + scrollSpeed - 1));
                uv.x = uv.x + (_SlippageNoiseOnOff * _SlippageStrength * slippageMask * stepMask); 
#endif

                float4 color = tex2D(_MainTex, float2(uv.x, uv.y));

#if _CHROMATICABERRATION_ON

                float polar = pow(polarCoordinates(i.uv, float2(0.5, 0.5), 1, 1).r, _ChromaticAberrationStrengthPolar);
                float polarScale = lerp(_ChromaticAberrationStrengthMin, _ChromaticAberrationStrength, polar);

                float red = tex2D(_MainTex, float2(uv.x - polarScale, uv.y)).r;
                float green = tex2D(_MainTex, float2(uv.x, uv.y)).g;
                float blue = tex2D(_MainTex, float2(uv.x + polarScale, uv.y)).b; 
                color.xyz = lerp(color.xyz, float3(red, green, blue), _ChromaticAberrationIntensity);
#endif

#if _MULTIPLEGHOST_ON
                float3 ghost1st = tex2D(_MainTex, uv - float2(1, 0) * _MultipleGhostStrength);
                float3 ghost2nd = tex2D(_MainTex, uv - float2(1, 0) * _MultipleGhostStrength * 2);
                color.rgb = lerp(color.rgb, (color.rgb * 0.8 + ghost1st * 0.15 + ghost2nd * 0.05), _MultipleGhostIntensity);
#endif

#if _WHITENOISE_ON
                float whiteNoise = frac(sin(dot(i.uv, float2(12.9898, 78.233)) + _Time.x) * 43758.5453);
                color.rgb = lerp(color.rgb, whiteNoise, _WhiteNoiseIntensity);
#endif

#if _SCANLINE_ON
                float scanline = sin((i.uv.y + _Time.x * _ScanlineSpeed) * 800.0) * 0.04 * _ScanlineIntensity;
                color -= scanline;

                //// noise
                //float noiseAlpha = 01;
                //if(pow(sin(uv.y + _Time.y * 2), 200) >= 0.999)
                //{
                //    noiseAlpha *= GetRandom(uv.y);
                //    color *= noiseAlpha;
                //}
#endif

#if _MONOCHOROME_ON
                float mc =  0.299 * color.r + 0.587 * color.g + 0.114 * color.b;
                color.xyz = lerp(color.xyz, mc, _MonochoromeIntensity); 
#endif

                return color;
            }
            ENDCG
        }
    }
}
