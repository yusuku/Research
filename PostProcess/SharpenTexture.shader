Shader "Custom/SharpenTexture"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Sharpness ("Sharpness", Range(0, 5)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
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
            float4 _MainTex_TexelSize;
            float _Sharpness;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 offsets[9] = {
                    float2(-1, -1), float2(0, -1), float2(1, -1),
                    float2(-1,  0), float2(0,  0), float2(1,  0),
                    float2(-1,  1), float2(0,  1), float2(1,  1)
                };

                float kernel[9] = {
                    0, 1, 0,
                    1,  -4, 1,
                    0, 1, 0
                };

                float4 color = float4(0, 0, 0, 0);
                for (int j = 0; j < 9; j++)
                {
                    float2 offset = offsets[j] * _MainTex_TexelSize.xy;
                    color += tex2D(_MainTex, i.uv + offset) * kernel[j];
                }

                return lerp(tex2D(_MainTex, i.uv), color, _Sharpness);
            }
            ENDCG
        }
    }
}
