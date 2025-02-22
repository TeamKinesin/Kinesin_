Shader "Custom/SonarFX"
{
    Properties
    {
        // 기본 색상 (오브젝트의 기본 색상)
        _SonarBaseColor("Base Color", Color) = (0.1, 0.1, 0.1, 1.0)
        
        // 파동 색상 및 파라미터 (파동 색상에 HDR 추가)
        [HDR]_SonarWaveColor("Wave Color", Color) = (1.0, 0.1, 0.1, 1.0)
        _SonarWaveParams("Wave Params", Vector) = (1, 20, 20, 10)
        // (X=Amplitude, Y=Exponent, Z=Interval(미사용), W=Speed)

        // 파동 중심 좌표 (월드 좌표)
        _SonarWaveCenter("Wave Center", Vector) = (0, 0, 0, 0)

        // 추가 컬러 (HDR 제거)
        _SonarAddColor("Add Color", Color) = (0, 0, 0, 1.0)
        _AlbedoColor("Albedo Color", Color) = (0, 0, 0, 1.0)
        
        // 파동 시작 시각
        _SonarWaveStartTime("Wave Start Time", Float) = 99999999.0

        // 파동이 도달하기 전(영역 외부) 색상 (포스트프로세싱된 하얀색)
        _PreWaveColor("Before Wave Color", Color) = (1, 1, 1, 1)

        // Post Processing Volume 값 (0: 오브젝트 그대로, 1: 완전 하얀색)
        // ※ 이 값은 스크립트에서 활성화된 포스트 프로세싱 볼륨의 값을 받아 업데이트하세요.
        _PostVolume("Post Process Volume", Range(0,1)) = 1.0
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
            Name "Unlit"
            Tags { "LightMode"="UniversalUnlit" }

            // 깊이 테스트 및 알파 블렌딩 설정
            ZTest Always
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            ///////////////////////////////////////////////////////
            // 유니폼 변수 (Properties에 선언된 변수들은 public입니다)
            ///////////////////////////////////////////////////////
            CBUFFER_START(UnityPerMaterial)
                float4 _SonarBaseColor;
                float4 _SonarWaveColor;
                float4 _SonarWaveParams;      // x=Amplitude, y=Exponent, z=Interval, w=Speed
                float4 _SonarWaveCenter;
                float4 _SonarAddColor;
                float4 _AlbedoColor;
                float  _SonarWaveStartTime;
                float4 _PreWaveColor;
                float  _PostVolume;
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

                // 영역 외부에서는 무조건 _PreWaveColor(하얀색)를 출력
                if(elapsed < 0)
                {
                    return half4(_PreWaveColor.rgb, _PreWaveColor.a);
                }
                
                float dist = distance(IN.positionWS, _SonarWaveCenter.xyz);
                float waveFront = elapsed * _SonarWaveParams.w;

                // 영역 외부 (파동 도달 전)인 경우, 무조건 하얀색 출력
                if(dist > waveFront)
                {
                    return half4(_PreWaveColor.rgb, _PreWaveColor.a);
                }
                
                // 영역 내부 (파동 도달 후): 파동 효과 계산
                float edgeWidth = 0.2;
                float edgeFactor = 1.0 - smoothstep(0.0, edgeWidth, abs(dist - waveFront));
                float waveFactor = edgeFactor * _SonarWaveParams.x;
                
                float3 baseColor = _SonarBaseColor.rgb;
                float3 waveColor = _SonarWaveColor.rgb * waveFactor;
                float3 addColor  = _SonarAddColor.rgb;
                float3 computedColor = baseColor + waveColor + addColor;
                float computedAlpha = edgeFactor;

                // 영역 내부에서는 _PostVolume 값에 따라 하얀색(_PreWaveColor)에서 계산된 색상으로 전환
                float3 finalColor = lerp(_PreWaveColor.rgb, computedColor, _PostVolume);
                float finalAlpha = lerp(_PreWaveColor.a, computedAlpha, _PostVolume);
                return half4(finalColor, finalAlpha);
            }
            ENDHLSL
        }
    }
    FallBack "Unlit/Texture"
}
