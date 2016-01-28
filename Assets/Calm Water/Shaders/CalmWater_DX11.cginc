#ifndef CALMWATER_DX11_INCLUDED
#define CALMWATER_DX11_INCLUDED



sampler2D _GrabTexture;
sampler2D _CameraDepthTexture;
sampler2D _ReflectionTex;

sampler2D _MainTex;
sampler2D _BumpMap;
half4 _BumpMap_ST;

sampler2D _FoamTex;
half4 _FoamTex_ST;
samplerCUBE _Cube;

fixed4 _Color;
fixed3 _SpecColor;
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
float _Tess;

uniform fixed4 _LightColor0;

struct appdata {
    float4 vertex 	: POSITION;
    float3 normal 	: NORMAL;
    float4 tangent 	: TANGENT;
    float2 texcoord : TEXCOORD0;
    float4 color 	: COLOR;
};


// V2F
struct v2f {
    float4 pos 		: SV_POSITION;
    float2 texcoord : TEXCOORD0;

    half4 tspace0 : TEXCOORD1; // tangent.x, bitangent.x, normal.x
    half4 tspace1 : TEXCOORD2; // tangent.y, bitangent.y, normal.y
    half4 tspace2 : TEXCOORD3; // tangent.z, bitangent.z, normal.z

    float3 worldPos : TEXCOORD4;
    float4 GrabUV 	: TEXCOORD5;
    float4 DepthUV	: TEXCOORD6;
    float4 AnimUV	: TEXCOORD7;

    #ifdef UNITY_PASS_FORWARDBASE
    float2 FoamUV	: TEXCOORD8;
    UNITY_FOG_COORDS(9)
    #endif

    #ifdef UNITY_PASS_FORWARDADD
    LIGHTING_COORDS(8,9)  
    #endif

    //LIGHTING_COORDS(10,11)   
    float4 color 	: COLOR;
};

inline float4 AnimateBump(float2 uv){

	float4 coords;

	coords.xy = TRANSFORM_TEX(uv,_BumpMap);
	coords.zw = TRANSFORM_TEX(uv,_BumpMap) * 0.5;

	coords.x += frac(_SpeedX * _Time.x);
	coords.y += frac(_SpeedY * _Time.x);
	coords.z -= frac(_SpeedX * 0.5 * _Time.x);
	coords.w -= frac(_SpeedY * 0.5 * _Time.x);

	return coords;
}


// Vertex portion goes here!
v2f vert (appdata v) {
    v2f o = (v2f)0;
    o.texcoord 	= v.texcoord;
    o.color 	= v.color;

    o.pos 			= mul(UNITY_MATRIX_MVP, v.vertex);
    o.GrabUV 		= ComputeGrabScreenPos(o.pos);
    o.DepthUV 		= ComputeScreenPos(o.pos);
	COMPUTE_EYEDEPTH(o.DepthUV.z);

	//UV Animation
	o.AnimUV = 	AnimateBump(v.texcoord);

	//Foam
	#ifdef UNITY_PASS_FORWARDBASE
	o.FoamUV =	TRANSFORM_TEX(v.texcoord,_FoamTex);
	#endif


	//Normals
	float4 worldPos 	= mul(_Object2World, v.vertex);
	fixed3 worldNormal 	= UnityObjectToWorldNormal(v.normal);
	fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
	fixed tangentSign 	= v.tangent.w * unity_WorldTransformParams.w;
	fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;

	o.tspace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
	o.tspace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
	o.tspace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
	o.worldPos = worldPos;

	#ifdef UNITY_PASS_FORWARDADD
	TRANSFER_VERTEX_TO_FRAGMENT(o);
	#endif


	#ifdef UNITY_PASS_FORWARDBASE
	UNITY_TRANSFER_FOG(o,o.pos);
	#endif

    return o;
}

#ifdef UNITY_CAN_COMPILE_TESSELLATION
    struct TessVertex {
        float4 vertex 	: INTERNALTESSPOS;
        float3 normal 	: NORMAL;
        float4 tangent 	: TANGENT;
        float2 texcoord : TEXCOORD0;
       	float4 color 	: COLOR;
    };

    struct OutputPatchConstant {
        float edge[3]         : SV_TessFactor;
        float inside          : SV_InsideTessFactor;
    };
	TessVertex tessvert (appdata v) {
		TessVertex o;
		o.vertex 	= v.vertex;
		o.normal 	= v.normal;
		o.tangent 	= v.tangent;
		o.texcoord 	= v.texcoord;
		o.color 	= v.color;
		return o;
	}

    void displacement (inout appdata v)
    {

		half3 worldSpaceVertex 	= mul(_Object2World,(v.vertex)).xyz;
		half3 vtxForAni 		= (worldSpaceVertex).xzz;


		// Gerstner Wave
		//half3 nrml;
		half3 offsets;
		half3 nrml;
		
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
		v.color.a = offsets.y;
    }

    float4 Tessellation(TessVertex v, TessVertex v1, TessVertex v2){
        return UnityEdgeLengthBasedTess(v.vertex, v1.vertex, v2.vertex, 32 - _Tess);
    }

    OutputPatchConstant hullconst (InputPatch<TessVertex,3> v) {
        OutputPatchConstant o;
        float4 ts = Tessellation( v[0], v[1], v[2] );
        o.edge[0] = ts.x;
        o.edge[1] = ts.y;
        o.edge[2] = ts.z;
        o.inside = ts.w;
        return o;
    }

    [domain("tri")]
    [partitioning("fractional_odd")]
    [outputtopology("triangle_cw")]
    [patchconstantfunc("hullconst")]
    [outputcontrolpoints(3)]
    TessVertex hull (InputPatch<TessVertex,3> v, uint id : SV_OutputControlPointID) {
        return v[id];
    }

    [domain("tri")]
    v2f domain (OutputPatchConstant tessFactors, const OutputPatch<TessVertex,3> vi, float3 bary : SV_DomainLocation) {
        appdata v = (appdata)0;

        v.vertex 	= vi[0].vertex*bary.x 	+ vi[1].vertex*bary.y 	+ vi[2].vertex*bary.z;
        v.texcoord 	= vi[0].texcoord*bary.x + vi[1].texcoord*bary.y + vi[2].texcoord*bary.z;
        v.color 	= vi[0].color*bary.x 	+ vi[1].color*bary.y 	+ vi[2].color*bary.z;
        v.tangent 	= vi[0].tangent*bary.x 	+ vi[1].tangent*bary.y 	+ vi[2].tangent*bary.z;
        v.normal 	= vi[0].normal*bary.x 	+ vi[1].normal*bary.y  	+ vi[2].normal*bary.z;

        float3 vertNormals = abs(v.normal);

        #if _DISPLACEMENT_ON
        displacement(v);
        #endif
        v2f o = vert(v);


        return o;
    }

#endif


#endif
