Shader "Custom/GenshinGrassURP_Transparent"
{
    Properties
    {
        _BaseMap ("Texture", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1,1,1,1)

        _WindStrength ("Wind Strength", Range(0, 1)) = 0.25
        _WindSpeed ("Wind Speed", Range(0, 5)) = 1.5
        _WindDirection ("Wind Direction (X,Z)", Vector) = (1, 0, 1, 0)

        _RimColor ("Rim Color", Color) = (0.6, 0.8, 0.6, 1)
        _RimPower ("Rim Power", Range(1, 8)) = 3
    }

    SubShader
    {
        Tags 
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            sampler2D _BaseMap;
            float4 _BaseMap_ST;
            float4 _BaseColor;

            float _WindStrength;
            float _WindSpeed;
            float4 _WindDirection;

            float4 _RimColor;
            float _RimPower;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 viewDirWS : TEXCOORD2;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };


            // ---- Vertex w/ Wind ----
            Varyings vert(Attributes v)
            {
                Varyings o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                float3 worldPos = TransformObjectToWorld(v.positionOS.xyz);

                float t = _Time.y * _WindSpeed;

                float sway = sin(worldPos.x + t) * cos(worldPos.z + t) * _WindStrength;

                float3 windDir = normalize(float3(_WindDirection.x, 0, _WindDirection.z));
                float heightFactor = saturate(v.positionOS.y);

                worldPos += windDir * sway * heightFactor;

                o.positionHCS = TransformWorldToHClip(worldPos);
                o.uv = TRANSFORM_TEX(v.uv, _BaseMap);
                o.normalWS = TransformObjectToWorldNormal(v.normalOS);

                o.viewDirWS = normalize(_WorldSpaceCameraPos - worldPos);

                return o;
            }


            // ---- Fragment ----
            half4 frag(Varyings i) : SV_Target
            {
                float4 tex = tex2D(_BaseMap, i.uv) * _BaseColor;

                // Transparency is fully respected
                float alpha = tex.a;

                // Toon lighting
                Light mainLight = GetMainLight();
                float NdotL = max(0, dot(i.normalWS, mainLight.direction));

                float toon = NdotL > 0.45 ? 1 : 0.25;

                // Rim light
                float rim = pow(1 - saturate(dot(i.viewDirWS, i.normalWS)), _RimPower);

                float3 color = tex.rgb * mainLight.color * toon;
                color += _RimColor.rgb * rim;

                return float4(color, alpha);
            }

            ENDHLSL
        }
    }

    FallBack Off
}