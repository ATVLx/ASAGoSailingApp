#ifndef CALMWATER_HELPER_INCLUDED
#define CALMWATER_HELPER_INCLUDED


inline half3 SafeNormalize(half3 inVec)
{
	half dp3 = max(0.001f, dot(inVec, inVec));
	return inVec * rsqrt(dp3);
}

//==========================================================================================================
// Wave
//==========================================================================================================
inline float sineWaveXY(appdata_full v, float speed, float amplitude, float frequency){
	float time = _Time.y * speed;
	float f = sin(time + v.vertex.x * frequency) + cos(time + v.vertex.z * frequency);
	return (f * (amplitude * 0.01));
}

//==========================================================================================================
// Rim
//==========================================================================================================
inline fixed RimLight (half3 vDir,fixed3 n,fixed rimPower){
	return pow(1.0 - saturate(dot(SafeNormalize(vDir),n)),rimPower);
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


float _Smoothing;

half3 GerstnerNormal (half2 xzVtx, half4 amp, half4 freq, half4 speed, half4 dirAB, half4 dirCD) 
{
	half3 nrml = half3(0,2.0,0);
//	half3 nrml = normalize(mul ((float3x3)UNITY_MATRIX_IT_MV, vnormal));
//	nrml.y *= 2;
	
	half4 AB = freq.xxyy * amp.xxyy * dirAB.xyzw;
	half4 CD = freq.zzww * amp.zzww * dirCD.xyzw;
	
	half4 dotABCD = freq.xyzw * half4(dot(dirAB.xy, xzVtx), dot(dirAB.zw, xzVtx), dot(dirCD.xy, xzVtx), dot(dirCD.zw, xzVtx));
	half4 TIME = _Time.yyyy * speed;
	
	half4 COS = cos (dotABCD + TIME);
	
	nrml.x -= dot(COS, half4(AB.xz, CD.xz));
	nrml.z -= dot(COS, half4(AB.yw, CD.yw));
	
	nrml.xz *= _Smoothing;
	nrml = normalize (nrml);

	return nrml;			
}	

half3 GerstnerOffset (half2 xzVtx, half steepness, half4 amp, half4 freq, half4 speed, half4 dirAB, half4 dirCD) 
{
	half3 offsets;
	
	half4 AB = steepness * amp.xxyy * dirAB.xyzw;
	half4 CD = steepness * amp.zzww * dirCD.xyzw;
	
	half4 dotABCD = freq.xyzw * half4(dot(dirAB.xy, xzVtx), dot(dirAB.zw, xzVtx), dot(dirCD.xy, xzVtx), dot(dirCD.zw, xzVtx));
	half4 TIME = _Time.yyyy * speed;
	
	half4 COS = cos (dotABCD + TIME);
	half4 SIN = sin (dotABCD + TIME);
	
	offsets.x = dot(COS, half4(AB.xz, CD.xz));
	offsets.z = dot(COS, half4(AB.yw, CD.yw));
	offsets.y = dot(SIN, amp);

	return offsets;			
}	

void Gerstner (	out half3 offs, out half3 nrml,
				 half3 vtx, half3 tileableVtx, 
				 half4 amplitude, half4 frequency, half4 steepness, 
				 half4 speed, half4 directionAB, half4 directionCD) 
{
		offs = GerstnerOffset(tileableVtx.xz, steepness, amplitude, frequency, speed, directionAB, directionCD);
		nrml = GerstnerNormal(tileableVtx.xz + offs.xz, amplitude, frequency, speed, directionAB, directionCD);							
}


#endif
