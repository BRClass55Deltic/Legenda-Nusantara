// Genshin-style Lake Water — Unity Standard Surface Shader (CG/HLSL)
// Built-in Render Pipeline — Fully Fixed Version

Shader "Custom/GenshinLakeWater_Surface"
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
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:fade
        #pragma target 3.0
        #pragma vertex vert

        sampler2D _NormalMap;

        struct Input
        {
            float2 uv_NormalMap;
            float4 screenPos;
            float3 worldPos;
            float3 viewDir;
        };

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

        half _Glossiness;
        half _Metallic;

        float _FresnelPower;
        float _FresnelIntensity;

        UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

        // Vertex modifier
        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.screenPos = UnityObjectToClipPos(v.vertex);
            o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
            o.uv_NormalMap = v.texcoord;
            o.viewDir = normalize(_WorldSpaceCameraPos - o.worldPos);
        }

        // Correct depth sampling (Surface Shader safe)
        inline float SampleLinearCameraDepth(float4 screenPos)
        {
            float raw = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(screenPos));
            return Linear01Depth(raw);
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            // Animated normal map
            float time = _Time.y;
            float2 flowUV =
                IN.uv_NormalMap * _WaveScale +
                float2(time * _WaveSpeed, -time * _WaveSpeed * 0.6);

            float3 normalT = UnpackNormal(tex2D(_NormalMap, flowUV));
            normalT.xy *= _NormalStrength;
            o.Normal = normalize(normalT);

            // Depth-based shading
            float sceneDepth = SampleLinearCameraDepth(IN.screenPos);

            float rawPixel = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(IN.screenPos));
            float pixelDepth = Linear01Depth(rawPixel);

            float depthDiff = saturate((sceneDepth - pixelDepth) / max(0.0001, _DepthMax));
            float shallowFactor = depthDiff;

            float3 waterColor = lerp(_DeepColor.rgb, _ShallowColor.rgb, shallowFactor);
            waterColor = lerp(waterColor, _BaseColor.rgb, 0.28);

            // Fresnel highlight
            float NdotV = saturate(dot(o.Normal, IN.viewDir));
            float fresnel = pow(1.0 - NdotV, _FresnelPower) * _FresnelIntensity;

            // Foam (shoreline)
            float foamMask = smoothstep(_FoamThreshold - 0.05, _FoamThreshold + 0.05, shallowFactor);
            float3 foam = _FoamColor.rgb * foamMask * _FoamIntensity;

            // Final output
            o.Albedo = waterColor + foam * 0.25;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Emission = fresnel * 0.25;

            o.Alpha = saturate(0.85 + foamMask * 0.15);
        }
        ENDCG
    }

    FallBack "Transparent/VertexLit"
}
