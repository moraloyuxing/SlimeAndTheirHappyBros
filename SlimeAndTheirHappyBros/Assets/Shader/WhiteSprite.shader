Shader "Custom/WhiteSprite"
{
	Properties
	{
		
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_colorID("Color ID", int) = 0
		_diffTrans("different gray", Float) = 0
		_WhiteColor("White", Color) = (1,1,1,1)
		_RedColor("Red", Color) = (1,0,0,1)
		_YellowColor("Yellow", Color) = (1,1,0,1)
		_OrangeColor("Orange", Color) = (1,0.5,0,1)
		_BlueColor("Blue", Color) = (0,0,1,1)
		_PurpleColor("Purple", Color) = (1,0,1,1)
		_GreenColor("Green", Color) = (0,1,0,1)

		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
	}

		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
			}

			Cull Off
			Lighting Off
			ZWrite Off
			ZTest Off
			Blend One OneMinusSrcAlpha

			Pass
			{
			CGPROGRAM
				#pragma vertex SpriteVert2
				#pragma fragment SpriteFrag2
				#pragma target 2.0
				#pragma multi_compile_instancing
				#pragma multi_compile _ PIXELSNAP_ON
				#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
				#include "UnitySprites.cginc"

			 struct fragmentOutput
			{
				half4 color : COLOR;
				float depth : DEPTH;
			};

		float _diffTrans;
		int _colorID;
		fixed4 _WhiteColor;
		fixed4 _RedColor;
		fixed4 _YellowColor;
		fixed4 _OrangeColor;
		fixed4 _BlueColor;
		fixed4 _PurpleColor;
		fixed4 _GreenColor;


			v2f SpriteVert2(appdata_t IN)
			{
				v2f OUT;

				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

				OUT.vertex = UnityFlipSprite(IN.vertex, _Flip);
				OUT.vertex = UnityObjectToClipPos(OUT.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color; //* _Color * _RendererColor;

				#ifdef PIXELSNAP_ON
					OUT.vertex = UnityPixelSnap(OUT.vertex);
				#endif

				return OUT;
		}

			fixed4 SpriteFrag2(v2f IN) : SV_Target
			{


				fixed4 c = SampleSpriteTexture(IN.texcoord)* IN.color;
			if (abs(c.x - c.y) < _diffTrans && abs(c.z - c.y) < _diffTrans) {
				if(_colorID == 0)c = c * _WhiteColor;
				else if (_colorID == 1)c = c * _RedColor;
				else if (_colorID == 2)c = c * _YellowColor;
				else if (_colorID == 3)c = c * _OrangeColor;
				else if (_colorID == 4)c = c * _BlueColor;
				else if (_colorID == 5)c = c * _PurpleColor;
				else if (_colorID == 6)c = c * _GreenColor;
			}
			//if(c.x > _diffTrans) c = c*IN.color;
				c.rgb *= c.a;
				//c.depth = 0.0f;
				//fixed4 c = fixed4(1.0,1.0,1.0,1.0);

				return c;
			}


			ENDCG
			}
		}
}
