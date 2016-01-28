// Calm Water ========================================
// Author: Yan Verde
// Date: 12/01/15
// ===================================================

Shader "CalmWater/CalmWater [DoubleSided]"
{
	Properties 
	{	
		//Color
		_Color("Shallow Color",Color) = (1,1,1,1)
		_DepthColor("Depth Color",Color) = (0,0,0,0)
		_Depth("Depth",float) = 0.5
		_EdgeFade("Edge Fade",float) = 1
		
		//Spec
		_SpecColor ("SpecularColor",Color) = (1,1,1,1)
		_Smoothness ("Smoothness",Range(0,5)) = 0.5
		
		//Normal Map
		_BumpMap("NormalMap", 2D) = "bump" {}
		_BumpStrength ("Bump Strength",Range(0,1)) = 1
		
		//Animation
		_SpeedX("Speed [X]",float) = 0.5
		_SpeedY("Speed [Y]",float) = 0.5
		
		//Distortion
		_Distortion("Distortion", Range(0,100) ) = 50.0
		[KeywordEnum(High,Low)]
		_DistortionQuality("Distortion Quality",Float) = 0
		
		//Reflection
		[KeywordEnum(Mixed,RealTime,CubeMap)] 
		_ReflectionType("ReflectionType", Float)  = 0
		
		_CubeColor("CubeMap Color [RGB] Intensity [A]",Color) = (1,1,1,1)
		[NoScaleOffset]
		_Cube("CubeMap", Cube) = "black" {}
		[NoScaleOffset]
		_ReflectionTex ("Internal reflection", 2D) = "black" {}
		
		_Reflection("Reflection", Range(0,1) ) = 0
		_RimPower("Fresnel Angle", Range(1,45) ) = 5
		
		//Foam
		[Toggle(_FOAM_ON)] _FOAM ("Enable Foam", Float) = 0
		_FoamColor("FoamColor",Color) = (1,1,1,1)
		_FoamTex("Foam Texture", 2D) = "black" {}
		_FoamSize("Fade Size",float) = 0.5
		
		//Displacement
		[Toggle(_DISPLACEMENT_ON)] _DISPLACEMENT ("Enable Displacement", Float) = 0
		_Amplitude ("Amplitude", float) = 0.05
		_Frequency("Frequency",float) = 1
		_Steepness ("Wave Steepness", float) = 1
		_WSpeed ("Wave Speed", Vector) = (1.2, 1.375, 1.1, 1.5)
		_WDirectionAB ("Wave1 Direction", Vector) = (0.3 ,0.85, 0.85, 0.25)
		_WDirectionCD ("Wave2 Direction", Vector) = (0.1 ,0.9, 0.5, 0.5)

		_Smoothing("Smoothing",range(0,1)) = 0
		
	}

	SubShader
		{
			LOD 500

			GrabPass
			{
				Name "BASE"
				Tags { "LightMode" = "Always" }
			}
			
			Tags { "Queue"="Transparent" "RenderType"="Transparent" "ForceNoShadowCasting" = "True"}
			Cull Back
			ZWrite Off
			ZTest LEqual

			CGPROGRAM
			#pragma surface surf Phong vertex:vert alpha:fade nolightmap nodirlightmap
			#pragma target 3.0

			#pragma shader_feature _REFLECTIONTYPE_MIXED _REFLECTIONTYPE_CUBEMAP _REFLECTIONTYPE_REALTIME 
			#pragma shader_feature _DISTORTIONQUALITY_HIGH _DISTORTIONQUALITY_LOW
			#pragma shader_feature _FOAM_OFF _FOAM_ON
			#pragma shader_feature _DISPLACEMENT_OFF _DISPLACEMENT_ON

			#include "CalmWater_Helper.cginc"
			#include "CalmWater.cginc"


			void surf (Input IN, inout SurfaceOutput o)
			{
				
				float3 vDir = (IN.worldPos - _WorldSpaceCameraPos);
				
				//Fresnel
				fixed fresnel = saturate(sqrt(RimLight(-vDir,IN.vNormal,_RimPower)) * 2);
				
				// NormalMaps
				fixed4 n1			= tex2D(_BumpMap, IN.AnimUV.xy);
				fixed4 n2 			= tex2D(_BumpMap, IN.AnimUV.zw);
				fixed3 finalBump 	= UnpackNormalBlend(n1,n2,(1.0 - fresnel) * _BumpStrength);
				//fixed3 finalBump 	= UnpackNormalBlend(n1,n2,(1.0 - fresnel) * _BumpStrength);
				float2 offset 		= finalBump.xy * _GrabTexture_TexelSize.xy * _Distortion;

				// Depth Distortion ===================================================
				float4 DepthUV = IN.DepthUV;
				DepthUV.xy = offset * DepthUV.z + DepthUV.xy;
				// GrabPass Distortion ================================================
				float4 GrabUV = IN.GrabUV;
				GrabUV.xy = offset * GrabUV.z + GrabUV.xy;

				//Depth Texture Clean
				half sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(IN.DepthUV)).r);
				//Depth Texture Distorted
				half DistZ 	= LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(DepthUV)).r);
				//Depth
				fixed depth = abs(DistZ - DepthUV.z);
				//Clean Depth
				fixed cleanDepth = abs(sceneZ - IN.DepthUV.z);
				
				// Refraction ============================================================
				fixed4 refraction = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(GrabUV));
				
				#if _DISTORTIONQUALITY_HIGH 
				//Hide refraction from objects over the surface		     
				if(DistZ < IN.GrabUV.z){
					//Undistorted depth mask
					depth = cleanDepth;
					//Refraction over surface
					refraction = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(IN.GrabUV) );   
				}
				#endif
				//Final color with depth and refraction
				fixed3 finalColor = lerp(_Color.rgb,_DepthColor,saturate(depth / _Depth)) * refraction;

				//Reflection =====================================================================
				//Reverse cubeMap Y to look like reflection
				#if _REFLECTIONTYPE_MIXED || _REFLECTIONTYPE_CUBEMAP
				vDir.y = -vDir.y;
				fixed3 cubeMap = texCUBE(_Cube, reflect(vDir, finalBump)).rgb * _CubeColor.rgb;
				#endif
				
				#if _REFLECTIONTYPE_MIXED || _REFLECTIONTYPE_REALTIME
				//Real Time reflections

				//TODO: Upgrade to GrabUV
				fixed3 rtReflections = tex2Dproj(_ReflectionTex, UNITY_PROJ_COORD(DepthUV)) * _Reflection;
				#endif
				
				#if _REFLECTIONTYPE_MIXED
				fixed3 finalReflection = lerp(cubeMap,rtReflections, 0.5);
				#endif
				
				#if _REFLECTIONTYPE_REALTIME
				fixed3 finalReflection = rtReflections;
				#endif
				
				#if _REFLECTIONTYPE_CUBEMAP
				fixed3 finalReflection = cubeMap;
				#endif
				
				finalColor = lerp(finalColor,finalReflection,fresnel * _Reflection);
				
				//FOAM ======================================================================
				#if _FOAM_ON
				//Foam Texture with animation
				fixed foamTex = tex2D(_FoamTex,IN.uv_FoamTex + (finalBump.xy * 0.02)).r;
				//Final Foam 
				fixed3 foam = saturate((1.0 - abs(_FoamSize * cleanDepth)));
				foam *= _FoamColor * foamTex;
				
				o.Emission		= 2.0 * min(1.0,foam);
				#endif

				o.Albedo 		= finalColor;
				o.Normal 		= finalBump;
				o.Specular 		= _SpecColor.a * 2.0;
				o.Gloss 		= _Smoothness;
				o.Alpha 		= saturate(_EdgeFade * cleanDepth) * _Color.a;

			}
			ENDCG
			
			
			// ==========
			// Back face
			//===========
			
			Tags { "Queue"="Transparent" "RenderType"="Transparent" "ForceNoShadowCasting" = "True"}
			Cull Front
			ZWrite Off
			ZTest LEqual
			//Blend SrcAlpha OneMinusSrcAlpha
			//AlphaTest Greater 0

			CGPROGRAM
			#pragma surface surf Phong vertex:vert alpha:fade nolightmap nodirlightmap noforwardadd
			#pragma target 3.0

			#include "CalmWater_Helper.cginc"
			#include "CalmWater.cginc"

			#pragma shader_feature _FOAM_OFF _FOAM_ON
			#pragma shader_feature _DISPLACEMENT_OFF _DISPLACEMENT_ON


			void surf (Input IN, inout SurfaceOutput o)
			{				
				// NormalMaps
				fixed4 n1			= tex2D(_BumpMap, IN.AnimUV.xy);
				fixed4 n2 			= tex2D(_BumpMap, IN.AnimUV.zw);
				fixed3 finalBump 	= UnpackNormalBlend(n1,n2,_BumpStrength);
				float2 offset = finalBump.xy * _GrabTexture_TexelSize.xy * _Distortion;

				// Depth Distortion ===================================================
				float4 DepthUV = IN.DepthUV;
				DepthUV.xy = offset * DepthUV.z + DepthUV.xy;
				// GrabPass Distortion ================================================
				float4 GrabUV = IN.GrabUV;
				GrabUV.xy = offset * GrabUV.z + GrabUV.xy;

				//Depth Texture Clean
				half sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(IN.DepthUV)).r);
				//Depth Texture Distorted
				half DistZ 	= LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(DepthUV)).r);
				//Depth
				fixed depth = abs(DistZ - DepthUV.z);
				//Clean Depth
				fixed cleanDepth = abs(sceneZ - IN.DepthUV.z);
				
				// Refraction ============================================================
				fixed4 refraction = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(GrabUV));
				//Final color with depth and refraction
				fixed3 finalColor = lerp(_Color,_DepthColor,0.5) * refraction;
				
				//FOAM ======================================================================
				#if _FOAM_ON
				//Foam Texture with animation
				fixed foamTex = tex2D(_FoamTex,IN.uv_FoamTex + (finalBump.xy * 0.02)).r;
				//Final Foam 
				fixed3 foam = saturate((1.0 - abs(_FoamSize * cleanDepth)));
				foam *= _FoamColor * foamTex;
				
				o.Emission		= 2.0 * min(1.0,foam);
				#endif
				
				o.Albedo 		= finalColor;
				o.Normal 		= finalBump;
				o.Specular 		= _SpecColor.a * 2.0;
				o.Gloss 		= _Smoothness;
				o.Alpha 		= saturate(_FoamSize * cleanDepth) * _Color.a;
			}
			ENDCG
		}
		CustomEditor "CalmWaterInspector"
		Fallback "Diffuse"
}