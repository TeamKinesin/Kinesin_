Shader "Custom/ScanSphereLit"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _Metallic  ("Metallic", Range(0,1)) = 0.0
        _Smoothness("Smoothness", Range(0,1)) = 0.5

        _ScanCenter("Scan Center", Vector) = (0,0,0,0)
        _ScanPos   ("Scan Position", Float) = 0.0
        _ScanWidth ("Scan Width", Float) = 0.1
        _ScanDir   ("Scan Direction", Vector) = (1,0,0,0)
    }
    SubShader
    {
        // 스캔되지 않은 영역은 투명하게 처리되므로 Transparent Queue 사용
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Pass
        {
            Name "ForwardLit"
            // ZTest: LEqual, ZWrite Off, 알파 블렌딩
            ZTest LEqual
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            // URP 코어와 라이트 관련 정의 포함
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            // fog 관련 매크로 기본 정의 (정의되지 않은 경우)
            #ifndef UNITY_FOG_COORDS
            #define UNITY_FOG_COORDS(var)
            #endif
            #ifndef UNITY_TRANSFER_FOG
            #define UNITY_TRANSFER_FOG(fog, pos)
            #endif
            #ifndef UNITY_APPLY_FOG
            #define UNITY_APPLY_FOG(fog, col) (col)
            #endif

            // 유니폼 변수 (URP Lit 스타일 조명 계산용)
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float  _Metallic;
                float  _Smoothness;
                float4 _ScanCenter;
                float  _ScanPos;
                float  _ScanWidth;
                float4 _ScanDir;
            CBUFFER_END

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS  : TEXCOORD0;
                float3 normalWS    : TEXCOORD1;
                UNITY_FOG_COORDS(2)
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 worldPos = TransformObjectToWorld(IN.positionOS);
                OUT.positionHCS = TransformWorldToHClip(worldPos);
                OUT.positionWS  = worldPos;
                OUT.normalWS    = normalize(TransformObjectToWorldNormal(IN.normalOS));
                UNITY_TRANSFER_FOG(OUT, OUT.positionHCS);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // 스캔 효과 계산: 스캔 중심(_ScanCenter)에서 스캔 방향(_ScanDir)을 따라 
                // 현재 스캔 평면(_ScanPos, _ScanWidth) 기준으로 스캔된(내부) 영역는 불투명하게,
                // 스캔되지 않은 영역은 투명하게 처리
                float3 scanDir = normalize(_ScanDir.xyz);
                float d = dot(IN.positionWS - _ScanCenter.xyz, scanDir);
                float alpha = 1.0 - smoothstep(_ScanPos, _ScanPos + _ScanWidth, d);
                clip(alpha - 0.01); // 알파값이 낮은 픽셀은 잘라냄

                // 간단한 PBR 스타일 조명 계산 (URP Lit 느낌 근사)
                float3 N = normalize(IN.normalWS);
                float3 V = normalize(_WorldSpaceCameraPos - IN.positionWS);
                // URP의 메인 라이트 변수 사용
                float3 L = normalize(_MainLightPosition.xyz);
                float3 H = normalize(L + V);

                float NdotL = saturate(dot(N, L));
                float3 diffuse = _BaseColor.rgb * NdotL;

                float NdotH = saturate(dot(N, H));
                float specPower = max(1.0, _Smoothness * 128.0);
                float specularTerm = pow(NdotH, specPower);
                float3 specular = specularTerm * _MainLightColor.rgb * _Metallic;

                float3 color = diffuse + specular + (0.1 * _BaseColor.rgb);
                half4 finalColor = half4(color, alpha);
                UNITY_APPLY_FOG(IN.fogCoord, finalColor);
                return finalColor;
            }
            ENDHLSL
        }
    }
    FallBack "Universal Forward"
}
