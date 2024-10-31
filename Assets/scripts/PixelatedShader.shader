Shader "Unlit/PixelatedShader"
{
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _PixelDensity ("Pixel Density", Float) = 100
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _PixelDensity;

            v2f vert (appdata_t v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // Calculate pixel size based on pixel density
                float2 pixelSize = float2(1.0 / _PixelDensity, 1.0 / _PixelDensity);
                // Snap UV to the nearest pixel
                float2 uv = floor(i.uv / pixelSize) * pixelSize;
                return tex2D(_MainTex, uv);
            }
            ENDCG
        }
    }
}
