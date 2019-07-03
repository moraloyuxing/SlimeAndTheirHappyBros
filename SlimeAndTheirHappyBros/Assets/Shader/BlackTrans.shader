Shader "ImageEffect/BlackTrans"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_MaskTex("Mask", 2D) = "white" {}
		_MaskScale("MaskScale",Range(0.0,1.0)) = 0.1
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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



            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

			fixed _MaskScale;
            sampler2D _MainTex;
			sampler2D _MaskTex;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
				fixed2 maskUV;
				maskUV.x = 0.5 + (i.uv.x - 0.5) / _MaskScale;
				maskUV.y = 0.5 + 0.5625 * ((i.uv.y - 0.5) / (_MaskScale));//
				fixed4 mask = tex2D(_MaskTex, maskUV);
				if (mask.a < 0.5) {
					col.rgb = float3(0, 0, 0);
				}
				col.rgb *= (mask.a);
				//col.a *= (1.0 - mask.a);
                return col;
            }
            ENDCG
        }
    }
}
