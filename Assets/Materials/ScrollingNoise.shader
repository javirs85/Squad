Shader "Unlit/ScrollingNoise"
{
    Properties
    {
        _MainTex ("Noise Texture", 2D) = "white" {}
        _Speed ("Scroll Speed", Vector) = (0, 1, 0, 0)
        _Color ("Color Tint", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        LOD 100

        Pass
        {
            ZWrite Off
            Cull Off
            Lighting Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Speed;
            float4 _Color;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float t = floor(_Time.y * 10.0);
                float seed = frac(sin(dot(float2(t * 123.4, t * 567.8), float2(12.9898, 78.233))) * 43758.5453);
                float2 randomOffset = frac(float2(seed, seed * 1.37));
                float2 uv = frac(i.uv + randomOffset);
                float noise = tex2D(_MainTex, uv).r;

                fixed4 col = fixed4(noise, noise, noise, 1.0) * _Color;
                return col;
            }
            ENDCG
        }
    }
}
