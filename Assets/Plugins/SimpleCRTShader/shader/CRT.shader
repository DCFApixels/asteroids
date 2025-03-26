Shader "Simple CRT"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DecalTex ("Decal Texture", 2D) = "white" {}
        _FilmDirtTex ("Dirt Texture", 2D) = "white" {}
        

        [Toggle] _Scanline("Scanline", Float) = 0
        _ScanlineIntensity ("Scanline Intensity", Float) = 0.15
        _ScanlineSpeed ("Scanline Speed", Float) = 1
        [Toggle] _Monochorome("Monochorome", Float) = 0
        _MonochoromeIntensity ("Monochorome Intensity", Float) = 0.5
        [Toggle] _WhiteNoise("White Noise", Float) = 0
        _WhiteNoiseIntensity("White Noise Intensity", Float) = 1
        [Toggle] _ScreenJump("Screen Jump", Float) = 0
        _ScreenJumpLevel("Screen Jump Level", Float) = 1
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
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 decaluv : TEXCOORD1;
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
            
            float _FlickeringStrength;
            float _FlickeringCycle;
            
            int _SlippageOnOff;
            float _SlippageStrength;
            float _SlippageInterval;
            float _SlippageScrollSpeed;
            float _SlippageNoiseOnOff;
            float _SlippageSize;

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

            sampler2D _DecalTex;
            float4 _DecalTex_ST;
            int _DecalTexOnOff;
            float2 _DecalTexPos;
            float2 _DecalTexScale;

            int _FilmDirtOnOff;
            sampler2D _FilmDirtTex;
            float4 _FilmDirtTex_ST;

            float GetRandom(float x);
            float EaseIn(float t0, float t1, float t);

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
                o.decaluv = TRANSFORM_TEX(v.uv, _DecalTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

#if _SCREENJUMP_ON /////Jump noise
                uv.y = frac(uv.y + _ScreenJumpLevel);
#endif

                /////frickering
                float flickeringNoise = GetRandom(_Time.y);
                float flickeringMask = pow(abs(sin(i.uv.y * _FlickeringCycle + _Time.y)), 10);
                uv.x = uv.x + (flickeringNoise * _FlickeringStrength * flickeringMask); 
                /////

                /////slippage
                float scrollSpeed = _Time.x * _SlippageScrollSpeed;
                float slippageMask = pow(abs(sin(i.uv.y * _SlippageInterval + scrollSpeed)), _SlippageSize);
                float stepMask = round(sin(i.uv.y * _SlippageInterval + scrollSpeed - 1));
                uv.x = uv.x + (_SlippageNoiseOnOff * _SlippageStrength * slippageMask * stepMask) * _SlippageOnOff; 
                /////


                float4 color = tex2D(_MainTex, float2(uv.x, uv.y));

#if _CHROMATICABERRATION_ON /////Chromatic Aberration

                float polar = pow(polarCoordinates(i.uv, float2(0.5, 0.5), 1, 1).r, _ChromaticAberrationStrengthPolar);
                float polarScale = lerp(_ChromaticAberrationStrengthMin, _ChromaticAberrationStrength, polar);

                float red = tex2D(_MainTex, float2(uv.x - polarScale, uv.y)).r;
                float green = tex2D(_MainTex, float2(uv.x, uv.y)).g;
                float blue = tex2D(_MainTex, float2(uv.x + polarScale, uv.y)).b; 
                color.xyz = lerp(color.xyz, float3(red, green, blue), _ChromaticAberrationIntensity);
#endif

#if _MULTIPLEGHOST_ON
                //float4 ghost1st = tex2D(_MainTex, uv - float2(1, 0) * _MultipleGhostStrength * _MultipleGhostOnOff);
                //float4 ghost2nd = tex2D(_MainTex, uv - float2(1, 0) * _MultipleGhostStrength * 2 * _MultipleGhostOnOff);
                //color = color * 0.8 + ghost1st * 0.15 + ghost2nd * 0.05;

                float3 ghost1st = tex2D(_MainTex, uv - float2(1, 0) * _MultipleGhostStrength);
                float3 ghost2nd = tex2D(_MainTex, uv - float2(1, 0) * _MultipleGhostStrength * 2);
                color.rgb = lerp(color.rgb, (color.rgb * 0.8 + ghost1st * 0.15 + ghost2nd * 0.05), _MultipleGhostIntensity);
                //color = ;
#endif

                /////File dirt
                float2 pp = -1.0 + 2.0 * uv;
                float time = _Time.x;
                float aaRad = 0.1;
                float2 nseLookup2 = pp + time * 1000;
                float3 nse2 =
                    tex2D(_FilmDirtTex, 0.1 * nseLookup2.xy).xyz +
                    tex2D(_FilmDirtTex, 0.01 * nseLookup2.xy).xyz +
                    tex2D(_FilmDirtTex, 0.004 * nseLookup2.xy).xyz;
                float thresh = 0.6;
                float mul1 = smoothstep(thresh - aaRad, thresh + aaRad, nse2.x);
                float mul2 = smoothstep(thresh - aaRad, thresh + aaRad, nse2.y);
                float mul3 = smoothstep(thresh - aaRad, thresh + aaRad, nse2.z);
                
                float seed = tex2D(_FilmDirtTex, float2(time * 0.35, time)).x;
                
                float result = clamp(0, 1, seed + 0.7);
                
                result += 0.06 * EaseIn(19.2, 19.4, time);

                float band = 0.05;
                if(_FilmDirtOnOff == 1)
                {
                if( 0.3 < seed && 0.3 + band > seed )
                    color *=  mul1 * result;
                else if( 0.6 < seed && 0.6 + band > seed )
                    color *= mul2 * result;
                else if( 0.9 < seed && 0.9 + band > seed )
                    color *= mul3 * result;
                }
                /////

                /////Letter box
                float band_uv = fmod(_MainTex_TexelSize.z, 640) / _MainTex_TexelSize.z / 2;
                if(i.uv.x < band_uv || 1 - band_uv < i.uv.x)
                {
                    float pi = 6.28318530718; 
                    float directions = 16.0; 
                    float quality = 3.0; 
                    float size = 8.0; 
                
                    float2 Radius = size * _MainTex_ST.zw;
                    float4 samplingColor = tex2D(_MainTex, uv);
                    
                    for(float d = 0.0; d < pi; d += pi / directions)
                    {
                        for(float i = 1.0 / quality; i <= 1.0; i += 1.0 / quality)
                        {
                            samplingColor += tex2D(_MainTex, uv + float2(cos(d), sin(d)) * 0.015 * i);		
                        }
                    }
                    samplingColor /= quality * directions - 15.0;
                    
                    if(_LetterBoxOnOff == 1)
                    {
                        color = color;
                    }
                    else if(_LetterBoxType == 0) // LetterBox is Black
                    {
                        color = 0;
                    }
                    else if(_LetterBoxType == 1) // LetterBox is Blur
                    {
                        color = samplingColor;
                    }
                }
                /////

#if _WHITENOISE_ON
                float whiteNoise = frac(sin(dot(i.uv, float2(12.9898, 78.233)) + _Time.x) * 43758.5453);
                color.rgb = lerp(color.rgb, whiteNoise, _WhiteNoiseIntensity);
#endif

                /////Decal texture
                float4 decal = tex2D(_DecalTex, (i.decaluv - _DecalTexPos) * _DecalTexScale) * _DecalTexOnOff;
                color = color * (1 - decal.a) + decal;
                /////

#if _SCANLINE_ON /////Scanline
                float scanline = sin((i.uv.y + _Time.x * _ScanlineSpeed) * 800.0) * 0.04 * _ScanlineIntensity;
                color -= scanline;

                ////////scanline noise
                //float noiseAlpha = 01;
                //if(pow(sin(uv.y + _Time.y * 2), 200) >= 0.999)
                //{
                //    noiseAlpha *= GetRandom(uv.y);
                //    color *= noiseAlpha;
                //}
#endif


#if _MONOCHOROME_ON //////Monochorome
                float mc =  0.299 * color.r + 0.587 * color.g + 0.114 * color.b;
                color.xyz = lerp(color.xyz, mc, _MonochoromeIntensity); 
#endif

                return color;
            }

            float GetRandom(float x)
            {
                return frac(sin(dot(x, float2(12.9898, 78.233))) * 43758.5453);
            }

            float EaseIn(float t0, float t1, float t)
            {
                return 2.0 * smoothstep(t0, 2.0 * t1 - t0, t);
            }
            ENDCG
        }
    }
}
