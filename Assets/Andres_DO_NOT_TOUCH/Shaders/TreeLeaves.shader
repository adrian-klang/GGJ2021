Shader "TreeLeaves"
{
    Properties
    {
        _MainTex ("Albedo", 2D) = "white" {}
        _CutoffTex ("Cutoff", 2D) = "white" {}
        _Shading("Shading", Range(0.0, 1.0)) = 1.0
    }
    SubShader
    {
        Tags{"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True"}
        LOD 100

        Pass
        {
            Cull Off

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
                float4 shadowcoord : TEXCOORD2;
            };

            UNITY_INSTANCING_BUFFER_START(UnityPerMaterial)
            UNITY_DEFINE_INSTANCED_PROP(float4, _TreeInstanceColor)
            UNITY_INSTANCING_BUFFER_END(UnityPerMaterial)

            sampler2D _MainTex;
            sampler2D _CutoffTex;
            float _Shading;

            Varyings vert (Attributes input)
            {
                Varyings output = (Varyings)0;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.texcoord;

                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                output.normalWS = normalInput.normalWS;

                output.shadowcoord = TransformWorldToShadowCoord(TransformObjectToWorld(input.positionOS.xyz));;
                return output;
            }

            half4 frag (Varyings input) : SV_Target
            {
                half4 albedo = tex2D(_MainTex, input.uv);
                half alpha = tex2D(_CutoffTex, input.uv);

                float treeColorVariation = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _TreeInstanceColor);
                if (treeColorVariation > 0.0f)
                {
                    half3 hsv = RgbToHsv(albedo);
                    hsv.b = treeColorVariation / 4.0f;
                    albedo.rgb = HsvToRgb(hsv);
                }

                // Compute ambient lighting.
                half3 irradiance = albedo.rgb * SampleSH(input.normalWS);

                // Compute direct lighting.
                Light mainLight = GetMainLight(input.shadowcoord);
                half directTerm = saturate(dot(input.normalWS, mainLight.direction) / _Shading) * mainLight.shadowAttenuation;
                half3 radiance = albedo.rgb * mainLight.color * directTerm;

                clip(alpha - 0.1f);
                return half4(irradiance + radiance, 1.0f);
            }
            ENDHLSL
        }
    }
}
