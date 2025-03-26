Shader "Asteroids/Nebula"
{
    Properties
    {
        _Seed ("Seed", Float) = 1
        _Color ("Color", Color) = (1,1,1,1)
        _OffsetR ("OffsetR", Float) = 0
        _OffsetG ("OffsetG", Float) = 0
        _OffsetB ("OffsetB", Float) = 0
        _OffsetA ("OffsetA", Float) = 0
        _OffsetAMultiplier ("OffsetAMultiplier", Float) = 0
        _Frequency ("Frequency", Float) = 1
        _FrequencyA ("FrequencyA", Float) = 1
        _Speed ("Speed", Float) = 1
        _Power ("Power", Float) = 1
        _ColorOffset ("ColorOffset", Float) = 0

        //_NoiseX ("NoiseX", Float) = 0
        //_FrequencyX ("FrequencyX", Float) = 0
        _ColorContrast ("ColorContrast", Float) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
        Blend One One
        ZWrite Off Fog { Mode Off }
	    Lighting Off 
        ZTest On
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "BitangentNoise_v0.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float4 vertex : SV_POSITION;
            };

            float1 _Seed;
            float4 _Color;
            float _OffsetR;
            float _OffsetG;
            float _OffsetB;
            float _OffsetA;
            float _OffsetAMultiplier;
            float _Frequency;
            float _FrequencyA;
            float _Speed;
            float _Power;
            float _ColorOffset;

            //float _NoiseX;
            //float _FrequencyX;
            float _ColorContrast;


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            float smoothstep(float x, float edge0, float edge1)
            {
                float t = clamp((x - edge0) / (edge1 - edge0), 0.0, 1.0);
                return t * t * (3.0 - 2.0 * t);
            }
            float2 smoothstep(float2 x, float2 edge0, float2 edge1)
            {
                float2 t = clamp((x - edge0) / (edge1 - edge0), 0.0, 1.0);
                return t * t * (3.0 - 2.0 * t);
            }
            float3 smoothstep(float3 x, float3 edge0, float3 edge1)
            {
                float3 t = clamp((x - edge0) / (edge1 - edge0), 0.0, 1.0);
                return t * t * (3.0 - 2.0 * t);
            }
            float4 smoothstep(float4 x, float4 edge0, float4 edge1)
            {
                float4 t = clamp((x - edge0) / (edge1 - edge0), 0.0, 1.0);
                return t * t * (3.0 - 2.0 * t);
            }

            #define PI 3.14159265359
            static const float b = 2.0;

            float abscos(float v)
            {
                float angle = (2.0 * PI * v) / b;
                float cosValue = cos(angle);
                float absCos = abs(cosValue);
                return 1.0 - absCos;
            }

            float4 UpContrast(float4 rgb) {
                float minChannel = min(min(rgb.r, rgb.g), rgb.b);
                float maxChannel = max(max(rgb.r, rgb.g), rgb.b);
                float factor = 1.0 / (maxChannel - minChannel + 1e-5); // +1e-5 чтобы избежать деления на 0
    
                return float4(
                    (rgb.r - minChannel) * factor,
                    (rgb.g - minChannel) * factor,
                    (rgb.b - minChannel) * factor,
                    rgb.a
                );
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float time = _Time.y * _Speed + _Seed;

                float r = BitangentNoise3D(float3(i.uv * _Frequency, _OffsetR + time))  * _Power + _ColorOffset;
                float g = BitangentNoise3D(float3(i.uv * _Frequency, _OffsetG + time))  * _Power + _ColorOffset;
                float b = BitangentNoise3D(float3(i.uv * _Frequency, _OffsetB + time))  * _Power + _ColorOffset;
                float a = BitangentNoise3D(float3(i.uv * _FrequencyA, _OffsetA + time)) * _Power + _ColorOffset;

                //float noiseX = BitangentNoise3D(float3(i.uv * _NoiseX, 100 + time));
                //float frequencyX = BitangentNoise3D(float3(i.uv * _FrequencyX, 200 + time));
                //float XXX = smoothstep(noiseX + frequencyX, 0, 1);

                float4 color = float4(r, g, b, a);
                color = smoothstep(color, 0, 1);
                color = lerp(color, UpContrast(color), _ColorContrast);

                float x = smoothstep(color.r + color.g + color.b, 0, 1);
                x = abscos(x);
                color.a *= x;

                color.a = (1 - _OffsetAMultiplier) + _OffsetAMultiplier * color.a;

                color = color * _Color * i.color;
                return color * color.a;
            }
            ENDCG
        }
    }
}
