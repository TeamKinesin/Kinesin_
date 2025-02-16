Shader "Custom/ScanSphereRing"
{
    Properties
    {
        _ScanColor   ("Scan Color", Color) = (0,1,0,1)
        _ScanCenter  ("Scan Center", Vector) = (0,0,0,0)
        _ScanPos     ("Scan Position", Float) = 0
        _ScanWidth   ("Scan Width", Float) = 0.1
        _ScanDir     ("Scan Direction", Vector) = (1,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Pass
        {
            // 링 효과는 전체 픽셀에 대해 항상 렌더
            ZTest Always
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
            float4 _ScanColor;
            float4 _ScanCenter;
            float  _ScanPos;
            float  _ScanWidth;
            float4 _ScanDir;
            CBUFFER_END

            struct Attributes
            {
                float3 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS  : TEXCOORD0;
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                float3 worldPos = TransformObjectToWorld(IN.positionOS);
                OUT.positionHCS = TransformWorldToHClip(worldPos);
                OUT.positionWS  = worldPos;
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float3 scanDir = normalize(_ScanDir.xyz);
                float d = dot(IN.positionWS - _ScanCenter.xyz, scanDir);
                // 링 효과: 스캔 평면 근처에서만 exp() 함수로 좁은 밴드를 생성
                float ring = exp(-pow((d - _ScanPos) / _ScanWidth, 2.0));
                return half4(_ScanColor.rgb, ring * _ScanColor.a);
            }
            ENDHLSL
        }
    }
    FallBack Off
}
