#ifndef CALMWATER_INCLUDED
#define CALMWATER_INCLUDED

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

float _EdgeFade;
float _BumpStrength;
float _SpeedX;
float _SpeedY;
float _Depth;
float _Distortion;
float _Reflection;
float _RimPower;
float _FoamSize;
float _Amplitude;
float _Frequency;
uniform float _Steepness;
uniform float4 _WSpeed;
uniform float4 _WDirectionAB;
uniform float4 _WDirectionCD;

half _Smoothness;

struct Input
{
	float2 uv_BumpMap;
	float2 uv_FoamTex;
	float4 GrabUV;
	float4 DepthUV;
	float4 AnimUV;
	float3 worldPos;
	float3 vNormal;
};

void vert (inout appdata_full v, out Input o)
{
	UNITY_INITIALIZE_OUTPUT(Input, o);
	
	#if _DISPLACEMENT_ON
	//v.vertex.y = sineWaveXY(v, _NoiseSpeed, _NoisePower, _NoiseFrequency);

	half3 worldSpaceVertex = mul(_Object2World,(v.vertex)).xyz;
	half3 vtxForAni = (worldSpaceVertex).xzz;

	// Gerstner Wave
	half3 nrml;
	half3 offsets;

	Gerstner (
		offsets, nrml, v.vertex.xyz, vtxForAni,				// offsets, nrml will be written
		_Amplitude * 0.01,									// amplitude
		_Frequency,											// frequency
		_Steepness,											// steepness
		_WSpeed,											// speed
		_WDirectionAB,										// direction # 1, 2
		_WDirectionCD										// direction # 3, 4
	);

	v.vertex.xyz += offsets;
	v.normal = nrml;
	//v.color.r = offsets.y;

	#endif

	float4 pos = mul(UNITY_MATRIX_MVP, v.vertex);
	//Depth Texture UV
	o.DepthUV = ComputeScreenPos(pos);
	COMPUTE_EYEDEPTH(o.DepthUV.z);
	//DistortionUV
	o.GrabUV = ComputeGrabScreenPos(pos);

	//UV Animation
	o.AnimUV.xy = TRANSFORM_TEX(v.texcoord,_BumpMap);
	o.AnimUV.zw = TRANSFORM_TEX(v.texcoord,_BumpMap) * 0.5;
	
	o.AnimUV.x += frac(_SpeedX * _Time.x);
	o.AnimUV.y += frac(_SpeedY * _Time.x);
	o.AnimUV.z -= frac(_SpeedX * 0.5 * _Time.x);
	o.AnimUV.w -= frac(_SpeedY * 0.5 * _Time.x);
	
	o.vNormal = abs(v.normal);
}

#endif
