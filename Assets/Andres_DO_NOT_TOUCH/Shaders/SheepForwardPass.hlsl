#ifndef SHEEP_FORWARD_PASS_INCLUDED
#define SHEEP_FORWARD_PASS_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Random.hlsl"

UNITY_INSTANCING_BUFFER_START(UnityPerMaterial)
UNITY_DEFINE_INSTANCED_PROP(float4, _BaseMap_ST)
UNITY_DEFINE_INSTANCED_PROP(float4, _BaseColor)
UNITY_DEFINE_INSTANCED_PROP(float, _Cutoff)
UNITY_INSTANCING_BUFFER_END(UnityPerMaterial)

struct MeshBufferAttributes {
	float4 positionOS;
	float2 texcoord;
	float3 normalOS;
};
StructuredBuffer<MeshBufferAttributes> VertexBuffer;

StructuredBuffer<float4x4> InstanceMatrixBuffer;
struct InstanceResources {
	float velocity;
};
StructuredBuffer<InstanceResources> InstanceResourcesBuffer;

// Ambient lighting.
real4 sheep_SHAr;
real4 sheep_SHAg;
real4 sheep_SHAb;
real4 sheep_SHBr;
real4 sheep_SHBg;
real4 sheep_SHBb;
real4 sheep_SHC;

half3 SampleSheepSH(half3 normalWS)
{
	// LPPV is not supported in Ligthweight Pipeline
	real4 SHCoefficients[7];
	SHCoefficients[0] = sheep_SHAr;
	SHCoefficients[1] = sheep_SHAg;
	SHCoefficients[2] = sheep_SHAb;
	SHCoefficients[3] = sheep_SHBr;
	SHCoefficients[4] = sheep_SHBg;
	SHCoefficients[5] = sheep_SHBb;
	SHCoefficients[6] = sheep_SHC;

	return max(half3(0, 0, 0), SampleSH9(SHCoefficients, normalWS));
}

struct Attributes {
	uint vertexId : SV_VertexID;
	uint instanceID : SV_InstanceID;
};

struct Varyings {
	float4 positionCS : SV_POSITION;
	float2 uv : TEXCOORD0;
	float3 normalWS : TEXCOORD1;
	float4 shadowCoord : TEXCOORD2;
	uint instanceID : SV_InstanceID;
};

#endif