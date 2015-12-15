// Calm Water ========================================
// Author: Yan Verde
// Date: 12/01/15
// ===================================================

Shader "CalmWater/CalmWater - DoubleSided"
{
	Properties 
	{	
		//Color
		_Color("Shallow Color",Color) = (1,1,1,1)
		_DepthColor("Depth Color",Color) = (0,0,0,0)
		_Depth("Depth",float) = 0.5
		
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
		_NoiseSpeed("Displacement Speed",float) = 1
		_NoisePower("Displacement Strength",float) = 0.5
		_NoiseFrequency("Displacement Frequency",float) = 1
		
	}
	
	CGINCLUDE
	
	
	//==========================================================================================================
	// Wave
	//==========================================================================================================
	inline float sineWaveXY(appdata_full v, float speed, float amplitude, float frequency){
		float time = _Time.y * speed;
		float f = sin(time + v.vertex.x * frequency);
		f += cos(time + v.vertex.z * frequency);
		return (f * amplitude);
	}

	//==========================================================================================================
	// Rim
	//==========================================================================================================
	inline fixed RimLight (half3 vDir,fixed3 n,fixed rimPower){
		return pow(1.0 - saturate(dot(Unity_SafeNormalize(vDir),n)),rimPower);
	}
	//==========================================================================================================
	// UnpackNormals blend and scale
	//==========================================================================================================
	half3 UnpackNormalBlend ( half4 n1, half4 n2, half scale){
	#if defined(UNITY_NO_DXT5nm)
		return n1.xyz * 2 - 1;
	#else
		half3 normal;
		normal.xy = (n1.wy * 2 - 1) + (n2.wy * 2 - 1);
		#if (SHADER_TARGET >= 30)
			normal.xy *= scale;
		#endif
		normal.z = sqrt(1.0 - saturate(dot(normal.xy, normal.xy)));
		
		return normalize(normal);
	#endif
	}
	inline fixed4 UnityPhongLight (SurfaceOutput s, half3 viewDir, UnityLight light)
	{		
			half NdotL = max(0.0, dot(s.Normal, normalize(light.dir)));
			half NdotV = max(0.0, dot(s.Normal, normalize(viewDir) ));
			
			half3 h = normalize (light.dir + viewDir);
			half reflectiveFactor = max (0, dot (s.Normal, h));
			half spec 	= pow(reflectiveFactor,  s.Gloss * 128.0 + 5.0 ) * s.Specular;
		
			half4 c;
			
			c.rgb = (s.Albedo * NdotL * light.color + light.color * _SpecColor.rgb * spec);
			c.a = s.Alpha;
			return c;
	}

	inline fixed4 LightingPhong (SurfaceOutput s, half3 viewDir, UnityGI gi)
	{
		fixed4 c;
		c = UnityPhongLight (s, viewDir, gi.light);

		#if defined(DIRLIGHTMAP_SEPARATE)
			#ifdef LIGHTMAP_ON
				c += UnityPhongLight (s, viewDir, gi.light2);
			#endif
			#ifdef DYNAMICLIGHTMAP_ON
				c += UnityPhongLight (s, viewDir, gi.light3);
			#endif
		#endif

		#ifdef UNITY_LIGHT_FUNCTION_APPLY_INDIRECT
			c.rgb += s.Albedo * gi.indirect.diffuse;
		#endif

		return c;
	}

	inline void LightingPhong_GI (
		SurfaceOutput s,
		UnityGIInput data,
		inout UnityGI gi)
	{
		gi = UnityGlobalIllumination (data, 1.0, s.Gloss, s.Normal, false);
	}
	
	
	sampler2D _GrabTexture;
	sampler2D _CameraDepthTexture;
	sampler2D _ReflectionTex;
	
	sampler2D _MainTex;
	sampler2D _BumpMap;
	half4 _BumpMap_ST;
	
	sampler2D _FoamTex;
	samplerCUBE _Cube;

	fixed4 _Color;
	fixed4 _DepthColor;
	fixed4 _CubeColor;
	fixed3 _FoamColor;

	half4 _GrabTexture_TexelSize;
	
	float _BumpStrength;
	float _SpeedX;
	float _SpeedY;
	float _Depth;
	float _Distortion;
	float _Reflection;
	float _RimPower;
	float _FoamSize;
	float _NoiseSpeed;
	float _NoisePower;
	float _NoiseFrequency;
	
	half _Smoothness;

	struct Input
	{
		float2 uv_BumpMap;
		float2 uv_FoamTex;
		float4 GrabUV;
		float4 AnimUV;
		float3 worldPos;
		float3 vNormal;
	};

	void vert (inout appdata_full v, out Input o)
	{
		UNITY_INITIALIZE_OUTPUT(Input, o);
		
		#if _DISPLACEMENT_ON
		
		v.vertex.y = sineWaveXY(v, _NoiseSpeed, _NoisePower, _NoiseFrequency);

		#endif
		//Depth Texture
		o.GrabUV = ComputeGrabScreenPos(mul(UNITY_MATRIX_MVP, v.vertex));
		COMPUTE_EYEDEPTH(o.GrabUV.z);
		
		//UV Animation
		o.AnimUV.xy = TRANSFORM_TEX(v.texcoord,_BumpMap);
		o.AnimUV.zw = TRANSFORM_TEX(v.texcoord,_BumpMap) * 0.5;
		
		o.AnimUV.x += frac(_SpeedX * _Time.x);
		o.AnimUV.y += frac(_SpeedY * _Time.x);
		o.AnimUV.z -= frac(_SpeedX * 0.5 * _Time.x);
		o.AnimUV.w -= frac(_SpeedY * 0.5 * _Time.x);
		
		o.vNormal = abs(v.normal);
	}
	
	ENDCG

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
			#pragma surface surf Phong vertex:vert alpha:fade nolightmap nodirlightmap noforwardadd
			#pragma target 3.0

			#pragma shader_feature _REFLECTIONTYPE_MIXED _REFLECTIONTYPE_CUBEMAP _REFLECTIONTYPE_REALTIME 
			#pragma shader_feature _DISTORTIONQUALITY_HIGH _DISTORTIONQUALITY_LOW
			#pragma shader_feature _FOAM_OFF _FOAM_ON
			#pragma shader_feature _DISPLACEMENT_OFF _DISPLACEMENT_ON


			void surf (Input IN, inout SurfaceOutput o)
			{
				
				float3 vDir = (IN.worldPos - _WorldSpaceCameraPos);
				
				//Fresnel
				fixed fresnel = saturate(sqrt(RimLight(-vDir,IN.vNormal,_RimPower)) * 2);
				
				// NormalMaps
				fixed4 n1			= tex2D(_BumpMap, IN.AnimUV.xy);
				fixed4 n2 			= tex2D(_BumpMap, IN.AnimUV.zw);
				fixed3 finalBump 	= UnpackNormalBlend(n1,n2,(1.0 - fresnel) * _BumpStrength);
				
				// Distortion =================================================================
				float4 DistUV = IN.GrabUV;
				float2 offset = finalBump.xy * _GrabTexture_TexelSize.xy * _Distortion;
				DistUV.xy = offset * DistUV.z + DistUV.xy;
				
				//Depth Texture Clean
				half sceneZ = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(IN.GrabUV)).r);
				//Depth Texture Distorted
				half DistZ 	= LinearEyeDepth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(DistUV)).r);
				//Depth
				fixed depth = abs(DistZ - IN.GrabUV.z);
				//Clean Depth
				fixed cleanDepth = abs(sceneZ - IN.GrabUV.z);
				
				// Refraction ============================================================
				fixed4 refraction = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(DistUV));
				
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
				fixed3 rtReflections = tex2Dproj(_ReflectionTex, UNITY_PROJ_COORD(DistUV)) * _Reflection;
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
				
				finalColor = lerp(finalColor,finalReflection,fresnel);
				
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
				//o.Metallic = 0.1;     
    			//o.Smoothness = _Smoothness; 
				o.Specular 		= _SpecColor.a * 2.0;
				o.Gloss 		= _Smoothness;
				o.Alpha 		= saturate(_FoamSize * cleanDepth) * _Color.a;
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

			#pragma shader_feature _FOAM_OFF _FOAM_ON
			#pragma shader_feature _DISPLACEMENT_OFF _DISPLACEMENT_ON


			void surf (Input IN, inout SurfaceOutput o)
			{				
				// NormalMaps
				fixed4 n1			= tex2D(_BumpMap, IN.AnimUV.xy);
				fixed4 n2 			= tex2D(_BumpMap, IN.AnimUV.zw);
				fixed3 finalBump 	= UnpackNormalBlend(n1,n2,_BumpStrength);
				
				// Distortion =================================================================
				float4 DistUV = IN.GrabUV;
				float2 offset = finalBump.xy * _GrabTexture_TexelSize.xy * _Distortion;
				DistUV.xy = offset * DistUV.z + DistUV.xy;
				
				//Depth Texture Clean
				half sceneZ = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(IN.GrabUV)).r);
				//Depth Texture Distorted
				half DistZ 	= LinearEyeDepth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(DistUV)).r);
				//Depth
				fixed depth = abs(DistZ - IN.GrabUV.z);
				//Clean Depth
				fixed cleanDepth = abs(sceneZ - IN.GrabUV.z);
				
				// Refraction ============================================================
				fixed4 refraction = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(DistUV));
				//Final color with depth and refraction
				fixed3 finalColor = lerp(_Color.rgb,_DepthColor,saturate(depth / _Depth)) * refraction;
				
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