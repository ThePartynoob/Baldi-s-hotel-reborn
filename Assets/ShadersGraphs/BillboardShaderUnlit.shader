Shader "Custom/Billboard/UnlitFixed"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.01
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Cutoff;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;

                // Extract object scale
                float3 scale;
                scale.x = length(unity_ObjectToWorld._m00_m10_m20);
                scale.y = length(unity_ObjectToWorld._m01_m11_m21);
                scale.z = length(unity_ObjectToWorld._m02_m12_m22);

                // Camera-facing axes
                float3 right = UNITY_MATRIX_V._m00_m01_m02;
                float3 up    = UNITY_MATRIX_V._m10_m11_m12;

                // Billboard offset (keeps scale!)
                float3 offset =
                    right * v.vertex.x * scale.x +
                    up    * v.vertex.y * scale.y;

                float3 worldPos = mul(unity_ObjectToWorld, float4(0,0,0,1)).xyz + offset;
                o.pos = UnityWorldToClipPos(worldPos);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                clip(col.a - _Cutoff);
                return col;
            }
            ENDCG
        }
    }
}
