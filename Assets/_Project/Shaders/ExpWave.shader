Shader "Asteroids/ExpWave"
{
  Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,1)
        _FresnelPower ("Fresnel Power", Range(0.1, 10)) = 3.0
        _FresnelScale ("Fresnel Scale", Range(0, 1)) = 0.5
        _FresnelBias ("Fresnel Bias", Range(0, 1)) = 0.1
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        
        Blend SrcAlpha One // Аддитивное смешивание
        ZWrite Off
        Cull Back
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };
            
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float3 normal : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _FresnelPower;
            float _FresnelScale;
            float _FresnelBias;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = normalize(_WorldSpaceCameraPos - mul(unity_ObjectToWorld, v.vertex).xyz);
                o.color = v.color;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color * i.color;
                float fresnel = saturate(_FresnelBias + _FresnelScale * pow(1.0 - dot(normalize(i.normal), normalize(i.viewDir)), _FresnelPower));
                col.a *= fresnel;
                
                return col;
            }
            ENDCG
        }
    }
    
    FallBack "Transparent"
}
