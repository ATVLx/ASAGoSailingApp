
Shader "CalmWater/Calm Water [DX11] [Double Sided]"{
	Properties {
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

		//[Header(Distortion)]
		//Distortion
		_Distortion("Distortion", Range(0,100) ) = 50.0
		[KeywordEnum(High,Low)]
		_DistortionQuality("Distortion Quality",Float) = 0

		//[Header(Reflection)]
		//Reflection
		[KeywordEnum(Mixed,RealTime,CubeMap)] 
		_ReflectionType("ReflectionType", Float)  = 0
		
		_CubeColor("CubeMap Color [RGB] Intensity [A]",Color) = (1,1,1,1)
		[NoScaleOffset]
		_Cube("CubeMap", Cube) = "black" {}
		[NoScaleOffset]
		_ReflectionTex ("Internal reflection", 2D) = "black" {}
		
		_Reflection("Reflection", Range(0,1) ) = 1
		_RimPower("Fresnel Angle", Range(1,45) ) = 5

		//[Header(Foam)]
		//Foam
		[Toggle(_FOAM_ON)] _FOAM ("Enable Foam", Float) = 0
		_FoamColor("FoamColor",Color) = (1,1,1,1)
		_FoamTex("Foam Texture", 2D) = "black" {}
		_FoamSize("Fade Size",float) = 0.5

		//[Header(Displacement)]
		//Displacement
		[Toggle(_DISPLACEMENT_ON)] _DISPLACEMENT ("Enable Displacement", Float) = 0
		_Amplitude ("Amplitude", float) = 0.05
		_Frequency("Frequency",float) = 1
		_Steepness ("Wave Steepness",float) = 1
		_WSpeed ("Wave Speed", Vector) = (1.2, 1.375, 1.1, 1.5)
		_WDirectionAB ("Wave1 Direction", Vector) = (0.3 ,0.85, 0.85, 0.25)
		_WDirectionCD ("Wave2 Direction", Vector) = (0.1 ,0.9, 0.5, 0.5)

		_Smoothing("Smoothing",range(0,1)) = 1

		//[Header(Tessellation)]
		_Tess ("Tessellation", Range(1,32)) = 4

	}

	SubShader {

		Tags { "Queue"="Transparent" "RenderType"="Opaque" }

 		GrabPass
		{
			Name "BASE"
			Tags { 
				"LightMode" = "Always" 
			}
		}

        Pass {
       		Tags {"LightMode" = "ForwardBase"}
            Name "FORWARD"
            ZWrite On
            Cull Back
           	//Blend SrcAlpha OneMinusSrcAlpha
 
            CGPROGRAM
            #pragma hull hull
            #pragma domain domain
            #pragma vertex tessvert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma exclude_renderers xbox360 ps3
            #pragma target 5.0
            #define UNITY_PASS_FORWARDBASE

            #include "AutoLight.cginc"
            #include "UnityCG.cginc"
            #include "Tessellation.cginc"
            #include "CalmWater_Helper.cginc"
            #include "CalmWater_DX11.cginc"


			#pragma shader_feature _REFLECTIONTYPE_MIXED _REFLECTIONTYPE_CUBEMAP _REFLECTIONTYPE_REALTIME 
			#pragma shader_feature _DISTORTIONQUALITY_HIGH _DISTORTIONQUALITY_LOW
			#pragma shader_feature _FOAM_OFF _FOAM_ON
			#pragma shader_feature _DISPLACEMENT_OFF _DISPLACEMENT_ON


             fixed4 frag( v2f i ) : SV_Target
             {

				float3 worldPos 	= float3(i.tspace0.w, i.tspace1.w, i.tspace2.w);
				float3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
				//float fresnel		= pow(saturate(dot(worldViewDir,i.color.rgb)),_RimPower * 0.25);


				#ifndef USING_DIRECTIONAL_LIGHT
					fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
				#else
					fixed3 lightDir = _WorldSpaceLightPos0.xyz;
				#endif

             	// NormalMaps
				fixed4 n1			= tex2D(_BumpMap, i.AnimUV.xy);
				fixed4 n2 			= tex2D(_BumpMap, i.AnimUV.zw);
				fixed3 finalBump 	= UnpackNormalBlend(n1,n2, _BumpStrength);

				fixed3 worldN;
				worldN.x 	= dot(i.tspace0.xyz, finalBump);
				worldN.y 	= dot(i.tspace1.xyz, finalBump);
				worldN.z 	= dot(i.tspace2.xyz, finalBump);
				worldN		= normalize(worldN);

				float2 offset = worldN.xz * _GrabTexture_TexelSize.xy * _Distortion;

				// Depth Distortion ===================================================
				float4 DepthUV = i.DepthUV;
				DepthUV.xy = offset * DepthUV.z + DepthUV.xy;
				// GrabPass Distortion ================================================
				float4 GrabUV = i.GrabUV;
				GrabUV.xy = offset * GrabUV.z + GrabUV.xy;

				//Depth Texture Clean
				half sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.DepthUV)).r);
				//Depth Texture Distorted
				half DistZ 	= LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(DepthUV)).r);
				//Depth
				fixed depth = abs(DistZ - DepthUV.z);
				//Clean Depth
				fixed cleanDepth = abs(sceneZ - i.DepthUV.z);

				// Refraction ============================================================
				fixed4 refraction 		= tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(GrabUV));
				fixed4 cleanRefreaction = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.GrabUV));

				#if _DISTORTIONQUALITY_HIGH 
				//Hide refraction from objects over the surface		     
				if(DistZ < i.GrabUV.z){
					//Undistorted depth mask
					depth = cleanDepth;
					//Refraction over surface
					refraction = cleanRefreaction;//tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.GrabUV) );   
				}
				#endif

				//Final color with depth and refraction
				fixed3 finalColor = lerp(_Color.rgb,_DepthColor,saturate(depth / _Depth)) * refraction;

				//Reflection =====================================================================
				//Reverse cubeMap Y to look like reflection
				#if _REFLECTIONTYPE_MIXED || _REFLECTIONTYPE_CUBEMAP
                half3 worldRefl 	= reflect(-worldViewDir, worldN);
                fixed3 cubeMap 		= texCUBE(_Cube, worldRefl).rgb * _CubeColor.rgb;
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

				//FOAM ======================================================================
				#if _FOAM_ON
				//Foam Texture with animation
				fixed foamTex = tex2D(_FoamTex,i.FoamUV + (finalBump.xy * 0.02)).r;
				//Final Foam 
				fixed3 foam =saturate((1.0 - abs(_FoamSize * cleanDepth)));
				foam *= _FoamColor * foamTex;
				
				finalColor	+= 2.0 * min(1.0,foam);
				#endif

				// Phong Lighting Model

				// Terms

				fixed NdotV = saturate( 1.0 - max(0.0,dot(worldN,worldViewDir)) );
				fixed NdotL = max(0.0,dot(worldN,lightDir));
				half fresnel = pow(NdotV,_RimPower);

				// Apply Reflection
				finalColor = lerp(finalColor,finalReflection,fresnel * _Reflection);

                //Specular
                half spec 			= pow(max(0.0, dot(reflect(-lightDir, worldN), worldViewDir)), _Smoothness * 128.0);
				fixed3 specColor 	= _LightColor0.rgb * _SpecColor.rgb * spec;
				fixed3 diff 		= ((2.0 * finalColor) * NdotL * _LightColor0.rgb + specColor);// * UNITY_LIGHTMODEL_AMBIENT.rgb;


				fixed4 c;
				c.rgb = lerp(cleanRefreaction,diff,saturate(_EdgeFade * cleanDepth));
				c.a =  _LightColor0.a;// * atten;//saturate(_FoamSize * cleanDepth) * _Color.a;

				UNITY_APPLY_FOG(i.fogCoord, c);


				return c;
             }
            ENDCG
        }

        Pass {
       		Tags {"LightMode" = "ForwardBase"}
            Name "FORWARD"
            ZWrite On
            Cull Front
           	//Blend SrcAlpha OneMinusSrcAlpha
 
            CGPROGRAM
            #pragma hull hull
            #pragma domain domain
            #pragma vertex tessvert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma exclude_renderers xbox360 ps3
            #pragma target 5.0
            #define UNITY_PASS_FORWARDBASE

            #include "AutoLight.cginc"
            #include "UnityCG.cginc"
            #include "Tessellation.cginc"
            #include "CalmWater_Helper.cginc"
            #include "CalmWater_DX11.cginc"


			#pragma shader_feature _REFLECTIONTYPE_MIXED _REFLECTIONTYPE_CUBEMAP _REFLECTIONTYPE_REALTIME 
			#pragma shader_feature _DISTORTIONQUALITY_HIGH _DISTORTIONQUALITY_LOW
			#pragma shader_feature _FOAM_OFF _FOAM_ON
			#pragma shader_feature _DISPLACEMENT_OFF _DISPLACEMENT_ON


             fixed4 frag( v2f i ) : SV_Target
             {

				float3 worldPos 	= float3(i.tspace0.w, i.tspace1.w, i.tspace2.w);
				float3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
				//float fresnel		= pow(saturate(dot(worldViewDir,i.color.rgb)),_RimPower * 0.25);


				#ifndef USING_DIRECTIONAL_LIGHT
					fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
				#else
					fixed3 lightDir = _WorldSpaceLightPos0.xyz;
				#endif

             	// NormalMaps
				fixed4 n1			= tex2D(_BumpMap, i.AnimUV.xy);
				fixed4 n2 			= tex2D(_BumpMap, i.AnimUV.zw);
				fixed3 finalBump 	= UnpackNormalBlend(n1,n2, _BumpStrength);

				fixed3 worldN;
				worldN.x 	= dot(i.tspace0.xyz, finalBump);
				worldN.y 	= dot(i.tspace1.xyz, finalBump);
				worldN.z 	= dot(i.tspace2.xyz, finalBump);
				worldN		= normalize(worldN);

				float2 offset = worldN.xz * _GrabTexture_TexelSize.xy * _Distortion;

				// Depth Distortion ===================================================
				float4 DepthUV = i.DepthUV;
				DepthUV.xy = offset * DepthUV.z + DepthUV.xy;
				// GrabPass Distortion ================================================
				float4 GrabUV = i.GrabUV;
				GrabUV.xy = offset * GrabUV.z + GrabUV.xy;

				//Depth Texture Clean
				half sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.DepthUV)).r);
				//Depth Texture Distorted
				half DistZ 	= LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(DepthUV)).r);
				//Depth
				fixed depth = abs(DistZ - DepthUV.z);
				//Clean Depth
				fixed cleanDepth = abs(sceneZ - i.DepthUV.z);

				// Refraction ============================================================
				fixed4 refraction 		= tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(GrabUV));
				fixed4 cleanRefreaction = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.GrabUV));

				//Final color with depth and refraction
				fixed3 finalColor = lerp(_Color.rgb,_DepthColor,0.5) * refraction;
		
				//FOAM ======================================================================
				#if _FOAM_ON
				//Foam Texture with animation
				fixed foamTex = tex2D(_FoamTex,i.FoamUV + (finalBump.xy * 0.02)).r;
				//Final Foam 
				fixed3 foam =saturate((1.0 - abs(_FoamSize * cleanDepth)));
				foam *= _FoamColor * foamTex;
				
				finalColor	+= 2.0 * min(1.0,foam);
				#endif

				// Phong Lighting Model

				// Terms

				fixed NdotV = saturate( 1.0 - max(0.0,dot(worldN,-worldViewDir)) );
				fixed NdotL = max(0.0,dot(worldN,lightDir));
				half fresnel = pow(NdotV,_RimPower);

				// Apply Reflection
				finalColor = lerp(finalColor,finalColor*2,fresnel * _Reflection);

                //Specular
                half spec 			= pow(max(0.0, dot(reflect(-lightDir, worldN), -worldViewDir)), _Smoothness * 128.0);
				fixed3 specColor 	= _LightColor0.rgb * _SpecColor.rgb * spec;
				fixed3 diff 		= ((finalColor) * NdotL * _LightColor0.rgb + specColor);// * UNITY_LIGHTMODEL_AMBIENT.rgb;


				fixed4 c;
				c.rgb = lerp(cleanRefreaction,diff,saturate(_EdgeFade * cleanDepth));
				c.a =  _LightColor0.a;// * atten;//saturate(_FoamSize * cleanDepth) * _Color.a;

				UNITY_APPLY_FOG(i.fogCoord, c);


				return c;
             }
            ENDCG
        }


       // =====

        Pass {
            Tags {"LightMode" = "ForwardAdd"}
            Blend One One
            Cull Back
            CGPROGRAM
            #pragma hull hull
            #pragma domain domain
            #pragma vertex tessvert
            #pragma fragment frag

            #pragma multi_compile_fwdadd  
            #pragma multi_compile_fog
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma exclude_renderers xbox360 ps3
            #pragma target 5.0


            #define UNITY_PASS_FORWARDADD
            #include "AutoLight.cginc"
            #include "UnityCG.cginc"
            #include "Tessellation.cginc"
            #include "CalmWater_Helper.cginc"
            #include "CalmWater_DX11.cginc"

			#pragma shader_feature _REFLECTIONTYPE_MIXED _REFLECTIONTYPE_CUBEMAP _REFLECTIONTYPE_REALTIME 
			#pragma shader_feature _DISTORTIONQUALITY_HIGH _DISTORTIONQUALITY_LOW
			#pragma shader_feature _DISPLACEMENT_OFF _DISPLACEMENT_ON


            fixed4 frag(v2f i) : SV_Target
            {

                float3 worldPos 	= float3(i.tspace0.w, i.tspace1.w, i.tspace2.w);
				float3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));

				#ifndef USING_DIRECTIONAL_LIGHT
					fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
				#else
					fixed3 lightDir = _WorldSpaceLightPos0.xyz;
				#endif

             	// NormalMaps
				fixed4 n1			= tex2D(_BumpMap, i.AnimUV.xy);
				fixed4 n2 			= tex2D(_BumpMap, i.AnimUV.zw);
				fixed3 finalBump 	= UnpackNormalBlend(n1,n2,_BumpStrength);

				// World Normals
				fixed3 worldN;
				worldN.x = dot(i.tspace0.xyz, finalBump);
				worldN.y = dot(i.tspace1.xyz, finalBump);
				worldN.z = dot(i.tspace2.xyz, finalBump);
				worldN = normalize(worldN);

				float2 offset = worldN.xz * _GrabTexture_TexelSize.xy * _Distortion;

				// Depth Distortion ===================================================
				float4 DepthUV = i.DepthUV;
				DepthUV.xy = offset * DepthUV.z + DepthUV.xy;
				// GrabPass Distortion ================================================
				float4 GrabUV = i.GrabUV;
				GrabUV.xy = offset * GrabUV.z + GrabUV.xy;

				//Depth Texture Clean
				half sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.DepthUV)).r);
				//Depth Texture Distorted
				half DistZ 	= LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(DepthUV)).r);
				//Depth
				fixed depth = abs(DistZ - DepthUV.z);
				//Clean Depth
				fixed cleanDepth = abs(sceneZ - i.DepthUV.z);

				// Refraction ============================================================
				fixed4 refraction 		= tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(GrabUV));
				fixed4 cleanRefreaction = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.GrabUV));

				#if _DISTORTIONQUALITY_HIGH 
				//Hide refraction from objects over the surface		     
				if(DistZ < i.GrabUV.z){
					//Undistorted depth mask
					depth = cleanDepth;
					//Refraction over surface
					refraction = cleanRefreaction;
				}
				#endif

				//Final color with depth and refraction
				fixed3 finalColor = lerp(_Color.rgb,_DepthColor,saturate(depth / _Depth)) * refraction;


				//Reflection =====================================================================
				//Reverse cubeMap Y to look like reflection
				#if _REFLECTIONTYPE_MIXED || _REFLECTIONTYPE_CUBEMAP
                half3 worldRefl 	= reflect(-worldViewDir, worldN);
                fixed3 cubeMap = texCUBE(_Cube, worldRefl).rgb * _CubeColor.rgb;
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

				// Terms
				fixed atten = LIGHT_ATTENUATION(i);
				fixed NdotV = saturate( 1.0 - max(0.0,dot(worldN,worldViewDir)) );
				fixed NdotL = max(0.0,dot(worldN,lightDir));
				half fresnel = pow(NdotV,_RimPower);

				// Apply Reflection
				finalColor = lerp(finalColor,finalReflection,fresnel);

                //Specular
                half spec 			= pow(max(0.0, dot(reflect(-lightDir, worldN), worldViewDir)), _Smoothness * 128.0);
				fixed3 specColor 	= _LightColor0.rgb * _SpecColor.rgb * spec;
				fixed3 diff 		= ((2.0 * finalColor) * NdotL * _LightColor0.rgb + specColor);


                fixed4 c;
                c.rgb 	= (diff * saturate(_EdgeFade * cleanDepth)) * atten;
                c.a 	= _LightColor0.a;

				return c;

            }
        ENDCG
        }

        }
    CustomEditor "CalmWaterInspector"
    FallBack "CalmWater/CalmWater [DoubleSided]"
}
 