Shader "Sheep"
{

    Properties
    {
        _BaseMap("Texture", 2D) = "white" {}
        _BaseColor("Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _Cutoff ("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
        [Toggle(_CLIPPING)] _Clipping ("Alpha Clipping", Float) = 0
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Src Blend", Float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Dst Blend", Float) = 0
        [Enum(Off, 0, On, 1)] _ZWrite ("Z Write", Float) = 1
    }

    SubShader
    {

        Pass
        {
            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]

            HLSLPROGRAM
            #pragma multi_compile_instancing

            #pragma vertex SheepVertex
            #pragma fragment SheepFragment

            #include "./SheepForwardPass.hlsl"

            Varyings SheepVertex(Attributes input)
            {
                Varyings output;
                output.instanceID = input.instanceID;
                float hash = Hash(input.instanceID);

                float4 positionOS = VertexBuffer[input.vertexId].positionOS;
                float2 texcoord = VertexBuffer[input.vertexId].texcoord;
                float3 normalOS = VertexBuffer[input.vertexId].normalOS;

                // Sheep scale
                float4x4 localToWorld = InstanceMatrixBuffer[input.instanceID];
                positionOS.xyz *= 1.0f + (hash * 2.0f - 1.0f) * 0.3f;
                float3 positionWS = mul(localToWorld, positionOS).xyz;

                // Sheep animation
                float x = hash + _Time.z;
                float jump = sin((x * 2.0 - 0.5) * PI) * 0.5 + 0.5;
                // TODO: Improve this.
                positionWS.y += jump * smoothstep(0.0f, 1.0f, saturate(InstanceResourcesBuffer[input.instanceID].velocity / 4.0f));

                output.positionCS = TransformWorldToHClip(positionWS);

                float4 baseST = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _BaseMap_ST);
                output.uv = texcoord * baseST.xy + baseST.zw;

                output.normalWS = SafeNormalize(mul((float3x3)localToWorld, normalOS));

                return output;
            }

            float4 SheepFragment(Varyings input) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(input);
                float hash = Hash(input.instanceID);
                
                float4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
                float4 baseColor = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _BaseColor);
                float4 albedo = baseMap * baseColor;

                float3 albedoHSV = RgbToHsv(albedo.rgb);
                albedoHSV.b = (albedoHSV.b + (hash * 2.0f - 1.0f) * 0.3f) % 1.0f;
                albedo.rgb = HsvToRgb(albedoHSV);

                // Compute ambient lighting.
                half3 irradiance = albedo.rgb * SampleSheepSH(input.normalWS);

                // Compute direct lighting.
                Light mainLight = GetMainLight();
                half directTerm = saturate(dot(input.normalWS, mainLight.direction) / 0.3f)* mainLight.shadowAttenuation;
                half3 radiance = albedo.rgb * mainLight.color * directTerm;

                return half4(irradiance + radiance, 1.0f);
            }
            ENDHLSL
        }
    }
}