Shader "Brush/StandardDoubleSided_URP"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _SpecColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
        _Shininess ("Shininess", Range(0.01,1)) = 0.078125
        _MainTex ("Base (RGB) TransGloss (A)", 2D) = "white" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "Queue"="AlphaTest" "RenderType"="TransparentCutout" "IgnoreProjector"="True" }
        LOD 400
        Cull Off

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma target 3.0

            // URP 핵심 및 라이트 관련 헤더
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            // Fog 관련 include 없이 빈 매크로 정의
            #define DECLARE_FOG_COORDS(name)
            #define TRANSFER_FOG_COORDS(o, pos)
            #define APPLY_FOG(coord, col)

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_BumpMap);
            SAMPLER(sampler_BumpMap);

            float4 _Color;
            float4 _SpecColor;
            float _Shininess;
            float _Cutoff;
            float4 _MainTex_ST;
            // _WorldSpaceCameraPos는 URP에서 기본 제공됩니다.

            struct Attributes
            {
                float4 vertex   : POSITION;
                float2 uv       : TEXCOORD0;
                float3 normal   : NORMAL;
                float4 tangent  : TANGENT;
                float4 color    : COLOR;
            };

            struct Varyings
            {
                float4 positionCS  : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float3 worldPos    : TEXCOORD1;
                float3 tspace0     : TEXCOORD2; // world-space tangent
                float3 tspace1     : TEXCOORD3; // world-space bitangent
                float3 tspace2     : TEXCOORD4; // world-space normal
                float4 color       : COLOR;
                DECLARE_FOG_COORDS(FOGCOORDS)
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.vertex);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.worldPos = TransformObjectToWorld(IN.vertex);
                OUT.color = IN.color * _Color;

                float3 worldNormal = normalize(TransformObjectToWorldNormal(IN.normal));
                float3 worldTangent = normalize(TransformObjectToWorldDir(IN.tangent.xyz));
                float tangentSign = IN.tangent.w;
                float3 worldBitangent = cross(worldNormal, worldTangent) * tangentSign;

                OUT.tspace0 = worldTangent;
                OUT.tspace1 = worldBitangent;
                OUT.tspace2 = worldNormal;

                TRANSFER_FOG_COORDS(OUT, OUT.positionCS);
                return OUT;
            }

            // vface를 사용하여 앞면과 뒷면을 구분하여 노말 반전을 처리합니다.
            float4 frag(Varyings IN, float vface : VFACE) : SV_Target
            {
                float4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                if (tex.a < _Cutoff)
                    discard;

                float3 albedo = tex.rgb * IN.color.rgb;
                float3 tnormal = UnpackNormal(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, IN.uv));
                // vface가 -1이면 뒷면이므로 z값을 반전합니다.
                tnormal.z *= vface;
                float3 worldNormal = normalize(
                    tnormal.x * IN.tspace0 +
                    tnormal.y * IN.tspace1 +
                    tnormal.z * IN.tspace2
                );

                Light mainLight = GetMainLight();
                float3 lightDir = normalize(mainLight.direction);
                float NdotL = saturate(dot(worldNormal, lightDir));
                float3 diffuse = albedo * mainLight.color.rgb * NdotL;

                float3 viewDir = normalize(_WorldSpaceCameraPos - IN.worldPos);
                float3 halfDir = normalize(viewDir + lightDir);
                float spec = pow(saturate(dot(worldNormal, halfDir)), _Shininess * 128.0);
                float3 specular = _SpecColor.rgb * spec * mainLight.color.rgb;

                float4 finalColor = float4(diffuse + specular, tex.a * IN.color.a);
                APPLY_FOG(FOGCOORDS, finalColor);
                return finalColor;
            }
            ENDHLSL
        }
    }
    FallBack "Universal Forward"
}
