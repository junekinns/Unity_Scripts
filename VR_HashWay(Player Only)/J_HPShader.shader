Shader "Unlit/J_HPShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Guage ("Guage", Range(0,1)) = 0
		_Red("Red", Range(0,1)) = 0
		_Green("Green", Range(0,1)) = 0
		_Blue("Blue", Range(0,1)) = 0
		_Alpha("Alpha", Range(0,1)) = 0

	}
	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
		LOD 100

		ZWrite off
        ZTest always
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Guage;
			float _Red;
			float _Green;
			float _Blue;
			float _Alpha;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);

				if(i.uv.x > _Guage){
					col.a = 0;
				}

				col.r *= _Red;
				col.g *= _Green;
				col.b *= _Blue;
				col.a *= _Alpha;

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
