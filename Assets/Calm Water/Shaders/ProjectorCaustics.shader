Shader "Projector/Caustics" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_CausticTex ("Cookie", 2D) = "" {}
		_Speed("Caustic Speed",float) = 1
		_Tiling("Tiling",float) = 1
		_FalloffTex ("FallOff", 2D) = "" {}
	}
	
	Subshader {
		Tags {"Queue"="Transparent"}
		Pass {
			ZWrite Off
			ColorMask RGB
			Blend SrcColor One
			Offset -1, -1
	
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#include "UnityCG.cginc"
			
			struct v2f {
				float4 uvCaustic : TEXCOORD0;
				float4 uvFalloff : TEXCOORD1;
				UNITY_FOG_COORDS(2)
				float4 pos : SV_POSITION;
			};
			
			float4x4 _Projector;
			float4x4 _ProjectorClip;
			float _Speed;
			float _Tiling;
			
			v2f vert (float4 vertex : POSITION)
			{
				v2f o;
				o.pos = mul (UNITY_MATRIX_MVP, vertex);

				//vertex.xy *= _Tiling;
				o.uvCaustic = mul (_Projector, vertex);
				o.uvFalloff = mul (_ProjectorClip, vertex);
				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}
			
			fixed4 _Color;
			sampler2D _CausticTex;
			sampler2D _FalloffTex;
			
			fixed4 frag (v2f i) : SV_Target
			{

				float4 uvCausticTiled = i.uvCaustic;
				uvCausticTiled.xy *= _Tiling;

				float4 uvCaustic = uvCausticTiled;
				uvCaustic.xy += frac(_Speed * _Time.x);
				float4 uvCaustic2 = uvCausticTiled;
				uvCaustic2.xy -= frac(_Speed * _Time.x);
				uvCaustic2.zw *= 0.5;

				fixed4 causticTex = tex2Dproj (_CausticTex, UNITY_PROJ_COORD(uvCaustic));
				fixed4 causticTex2 = tex2Dproj (_CausticTex, UNITY_PROJ_COORD(uvCaustic2));

				fixed4 finalCaustic = causticTex + causticTex2;

				finalCaustic.rgb *= _Color.rgb;
				finalCaustic.a = 1.0-finalCaustic.a;
	
				fixed4 texF = tex2Dproj (_FalloffTex, UNITY_PROJ_COORD(i.uvFalloff));
				fixed4 res = finalCaustic * texF.a;

				UNITY_APPLY_FOG_COLOR(i.fogCoord, res, fixed4(0,0,0,0));
				return res;
			}
			ENDCG
		}
	}
}
