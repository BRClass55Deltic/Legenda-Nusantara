Shader "Custom/URP_GenshinLakeWater"
{
    Properties
    {
        _BaseColor ("Base Tint", Color) = (0.22,0.6,0.78,1)
        _ShallowColor ("Shallow Color", Color) = (0.45,0.85,1,1)
        _DeepColor ("Deep Color", Color) = (0.03,0.22,0.45,1)
        _DepthMax ("Depth Max", Float) = 6.0

        _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalStrength ("Normal Strength", Range(0,2)) = 1.0
        _WaveScale ("Wave Scale", Float) = 0.8
        _WaveSpeed ("Wave Speed", Float) = 0.12

        _FoamColor ("Foam Color", Color) = (1,1,1,1)
        _FoamThreshold ("Foam Threshold", Range(0,1)) = 0.18
        _FoamIntensity ("Foam Intensity", Range(0,1)) = 0.9

        _Glossiness ("Smoothness", Range(0,1)) = 0.6
        _Metallic ("Metallic", Range(0,1)) = 0.0

        _FresnelPower ("Fresnel Power", Range(0.1,6)) = 2.0
        _FresnelIntensity ("Fresnel Intensity", Range(0,2)) = 1.2
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "RenderPipeline"="UniversalRenderPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_fog
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);

            float4 _BaseColor;
            float4 _ShallowColor;
            float4 _DeepColor;
            float _DepthMax;

            float _NormalStrength;
            float _WaveScale;
            float _WaveSpeed;

            float4 _FoamColor;
            float _FoamThreshold;
            float _FoamIntensity;

            float _Glossiness;
            float _Metallic;

            float _FresnelPower;
            float _FresnelIntensity;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float3 normalWS    : TEXCOORD1;
                float3 posWS       : TEXCOORD2;
                float3 viewDirWS   : TEXCOORD3;
                float4 screenPos   : TEXCOORD4;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.posWS       = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.normalWS    = TransformObjectToWorldNormal(IN.normalOS);
                OUT.uv          = IN.uv;
                OUT.viewDirWS   = GetWorldSpaceViewDir(OUT.posWS);

                OUT.screenPos   = ComputeScreenPos(OUT.positionHCS);

                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float time = _Time.y;

                // Scrolling wave normalmap (Genshin-style)
                float2 flowUV =
                    IN.uv * _WaveScale +
                    float2(time * _WaveSpeed, -time * _WaveSpeed * 0.6);

                float3 normal = UnpackNormal(
                    SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, flowUV)
                );
                normal.xy *= _NormalStrength;
                normal = normalize(normal);

                float3 normalWS = normalize(IN.normalWS);
                float3 viewDir  = normalize(IN.viewDirWS);

                //---------------------------------------------------------
                // Depth sampling (correct URP version)
                //---------------------------------------------------------
                float2 screenUV = IN.screenPos.xy / IN.screenPos.w;

                float rawSceneDepth = SampleSceneDepth(screenUV);
                float sceneDepth01  = Linear01Depth(rawSceneDepth, _ZBufferParams);

                float pixelRawDepth = IN.positionHCS.z / IN.positionHCS.w;
                float pixelDepth01  = Linear01Depth(pixelRawDepth, _ZBufferParams);

                float depthDiff = saturate((sceneDepth01 - pixelDepth01) / max(_DepthMax, 0.001));

                //---------------------------------------------------------
                // Depth → Color gradient (deep → shallow → base tint)
                //---------------------------------------------------------
                float3 waterColor = lerp(_DeepColor.rgb, _ShallowColor.rgb, depthDiff);
                waterColor = lerp(waterColor, _BaseColor.rgb, 0.28);

                //---------------------------------------------------------
                // Fresnel
                //---------------------------------------------------------
                float NdotV = saturate(dot(normalWS, viewDir));
                float fresnel = pow(1 - NdotV, _FresnelPower) * _FresnelIntensity;

                //---------------------------------------------------------
                // Shoreline foam
                //---------------------------------------------------------
                float foamMask = smoothstep(
                    _FoamThreshold - 0.05,
                    _FoamThreshold + 0.05,
                    depthDiff
                );

                float3 foam = _FoamColor.rgb * foamMask * _FoamIntensity;

                //---------------------------------------------------------
                // Lighting (basic URP main light)
                //---------------------------------------------------------
                Light mainLight = GetMainLight();
                float3 L = mainLight.direction;
                float NdotL = saturate(dot(normalWS, L));

                float3 litColor = waterColor * (0.3 + NdotL * 0.7);

                //---------------------------------------------------------
                // Final Color
                //---------------------------------------------------------
                float3 finalCol = litColor + foam * 0.25 + fresnel * 0.25;
                float alpha = saturate(0.85 + foamMask * 0.15);

                return float4(finalCol, alpha);
            }

            ENDHLSL
        }
    }
}
