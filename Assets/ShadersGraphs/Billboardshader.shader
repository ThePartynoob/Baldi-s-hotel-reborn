Shader "Custom/Billboard/LitFixed"
{
    Properties
    {
        _MainTex ("Albedo", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.01
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="TransparentCutout" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        CGPROGRAM
        #pragma surface surf Standard alpha:fade addshadow

        sampler2D _MainTex;
        fixed4 _Color;
        float _Cutoff;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            clip(c.a - _Cutoff);

            o.Albedo = c.rgb;
            o.Alpha = c.a;
            o.Normal = float3(0,0,1); // billboard normal
        }
        ENDCG
    }
}
