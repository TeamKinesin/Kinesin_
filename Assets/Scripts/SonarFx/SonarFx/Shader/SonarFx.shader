Shader "Custom/SonarFX"
{
    Properties
    {
        // 기본 색상
        _SonarBaseColor("Base Color", Color) = (0.1, 0.1, 0.1, 1.0)
        
        // 파동 색상 + 파라미터
        [HDR]_SonarWaveColor("Wave Color", Color) = (1.0, 0.1, 0.1, 1.0)
        _SonarWaveParams("Wave Params", Vector) = (1, 20, 20, 10)
        //  (X=Amplitude, Y=Exponent, Z=Interval(미사용), W=Speed)

        // 파동 중심 좌표 (월드 좌표)
        _SonarWaveCenter("Wave Center", Vector) = (0, 0, 0, 0)

        // 추가 컬러
        [HDR]_SonarAddColor("Add Color", Color) = (0, 0, 0, 1.0)
        [HDR]_AlbedoColor("Albedo Color", Color) = (0, 0, 0, 1.0)
        
        // 파동 시작 시각
        _SonarWaveStartTime("Wave Start Time", Float) = 99999999.0
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }

        Pass
        {
            Name "ForwardOnly"
            Tags { "LightMode"="UniversalForwardOnly" }

            // 깊이 설정은 필요에 맞게
            ZTest Always
            ZWrite Off

            // 알파 블렌딩
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            ///////////////////////////////////////////////////////
            // 유니폼 변수
            ///////////////////////////////////////////////////////
            CBUFFER_START(UnityPerMaterial)
            float4 _SonarBaseColor;
            float4 _SonarWaveColor;
            float4 _SonarWaveParams;      // x=Amplitude, y=Exponent, z=Interval(미사용), w=Speed
            float4 _SonarWaveCenter;

            float4 _SonarAddColor;
            float4 _AlbedoColor;

            float  _SonarWaveStartTime;   // 파동 시작 시각
            CBUFFER_END

            ///////////////////////////////////////////////////////
            // 정점 구조체
            ///////////////////////////////////////////////////////
            struct Attributes
            {
                float3 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS  : TEXCOORD0;
            };

            ///////////////////////////////////////////////////////
            // Vertex Shader
            ///////////////////////////////////////////////////////
            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                float3 worldPos = TransformObjectToWorld(IN.positionOS);
                OUT.positionHCS = TransformWorldToHClip(worldPos);
                OUT.positionWS  = worldPos;
                return OUT;
            }

            ///////////////////////////////////////////////////////
            // Fragment Shader
            ///////////////////////////////////////////////////////
            half4 frag (Varyings IN) : SV_Target
            {
                float currentTime = _Time.y;
                float elapsed = currentTime - _SonarWaveStartTime;

                // 아직 파동 시작 전 → 전체 검정(혹은 다른 색)
                if(elapsed < 0)
                {
                    return half4(0, 0, 0, 1);
                }

                float dist = distance(IN.positionWS, _SonarWaveCenter.xyz);
                float waveFront = elapsed * _SonarWaveParams.w;

                // 파동 범위 밖이면 → 검정색(투명 X, 완전 불투명)
                if(dist > waveFront)
                {
                    return half4(0, 0, 0, 1);
                }

                // 파동 범위 안: 기존 로직(가장자리 강조)
                float edgeWidth  = 0.2;
                float edgeFactor = 1.0 - smoothstep(0.0, edgeWidth, abs(dist - waveFront));
                float waveFactor = edgeFactor * _SonarWaveParams.x;

                float3 baseColor = _SonarBaseColor.rgb;
                float3 waveColor = _SonarWaveColor.rgb * waveFactor;
                float3 addColor  = _SonarAddColor.rgb;
                float3 finalColor = baseColor + waveColor + addColor;

                // 가장자리에 따라 알파도 줄 수 있지만,
                // 파동 내부를 꽉 채우고 싶다면 alpha = 1 로 고정도 가능
                float alpha = edgeFactor;

                return half4(finalColor, alpha);
            }
            ENDHLSL
        }
    }
    FallBack Off
}
