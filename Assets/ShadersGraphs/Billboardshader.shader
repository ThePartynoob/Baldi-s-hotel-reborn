Shader "Custom/Billboardshader"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "RenderPipeline"="UniversalPipeline"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        ZWrite Off

        // Premultiplied alpha
        Blend One OneMinusSrcAlpha

        Pass
        {
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float3 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                float4 color      : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float4 color       : COLOR;
                float3 positionWS  : TEXCOORD1;
                float3 normalWS    : TEXCOORD2;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _Color;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;

                float3 centerWS = TransformObjectToWorld(float3(0,0,0));

                // Camera forward (FULL 3D)
                float3 camForward = normalize(-UNITY_MATRIX_V[2].xyz);

                // Object scale
                float3 objRight = UNITY_MATRIX_M._m00_m01_m02;
                float3 objUp    = UNITY_MATRIX_M._m10_m11_m12;

                float scaleX = length(objRight);
                float scaleY = length(objUp);

                // Billboard axes
                float3 right = normalize(cross(objUp, camForward));
                float3 up    = normalize(cross(camForward, right));

                float3 worldPos =
                    centerWS +
                    right * IN.positionOS.x * scaleX +
                    up    * IN.positionOS.y * scaleY;

                OUT.positionWS  = worldPos;
                OUT.positionHCS = TransformWorldToHClip(worldPos);
                OUT.uv          = IN.uv;
                OUT.color       = IN.color * _Color;

                // Normal faces the camera
                OUT.normalWS = -camForward;

                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                // Sample sprite
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                col *= IN.color;

                // Premultiply alpha
                col.rgb *= col.a;

                // --- Lighting ---
                InputData inputData;
                inputData.positionWS = IN.positionWS;
                inputData.normalWS   = normalize(IN.normalWS);
                inputData.viewDirectionWS = normalize(_WorldSpaceCameraPos - IN.positionWS);
                inputData.shadowCoord = TransformWorldToShadowCoord(IN.positionWS);
                inputData.fogCoord = 0;
                inputData.vertexLighting = half3(0,0,0);
                inputData.bakedGI = half3(0,0,0);

                // Main light
                Light mainLight = GetMainLight(inputData.shadowCoord);
                half NdotL = saturate(dot(inputData.normalWS, mainLight.direction));

                half3 lighting =
                    mainLight.color * NdotL * mainLight.shadowAttenuation;

                col.rgb *= lighting;

                return col;
            }
            ENDHLSL
        }
    }
}
