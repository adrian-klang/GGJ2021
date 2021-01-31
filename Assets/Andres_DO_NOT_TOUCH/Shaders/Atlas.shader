Shader "Atlas"
{
    Properties
    {
        _MainTex ("Albedo", 2D) = "white" {}
        _Shading("Shading", Range(0.0, 1.0)) = 1.0
    }
    SubShader
    {
        Tags{"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True"}
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma multi_compile_instancing

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            #pragma vertex vert
            #pragma fragment frag
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 texcoord : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float4 shadowCoord : TEXCOORD2; 
            };

            sampler2D _MainTex;
            float _Shading;

            Varyings vert (Attributes input)
            {
                Varyings output = (Varyings)0;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.texcoord;

                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                output.normalWS = normalInput.normalWS;

                output.shadowCoord = TransformWorldToShadowCoord(TransformObjectToWorld(input.positionOS.xyz));;
                return output;
            }

            half4 frag (Varyings input) : SV_Target
            {
                half4 albedo = tex2D(_MainTex, input.uv);

                // Compute ambient lighting.
                half3 irradiance = albedo.rgb * SampleSH(input.normalWS);

                // Compute direct lighting.
                Light mainLight = GetMainLight(input.shadowCoord);             
                half directTerm = saturate(dot(input.normalWS, mainLight.direction) / _Shading) * mainLight.shadowAttenuation;
                half3 radiance = albedo.rgb * mainLight.color * directTerm;

                return half4(irradiance + radiance, 1.0f);
            }
            ENDHLSL
        }
    }
}
