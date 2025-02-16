using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace URPGlitch.Runtime.SonarGlitch
{
    /// <summary>
    /// Inspector에서 설정할 수 있는 파라미터들을 포함합니다.
    /// 원하는 머티리얼(소나 효과용)과 스캔할 오브젝트가 있는 레이어를 지정하세요.
    /// </summary>
    [System.Serializable]
    public class SonarFxSettings
    {
        // 소나 효과를 나타내는 머티리얼 (소나 효과 셰이더가 적용된 머티리얼)
        public Material sonarMaterial = null;

        // 소나 효과를 적용할 오브젝트들이 속한 레이어 마스크
        public LayerMask layerMask = 0;

        // Render Pass가 실행될 시점을 지정합니다.
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    /// <summary>
    /// SonarFxFeature : 특정 레이어의 오브젝트에만 소나 스캔 효과(override material)를 적용합니다.
    /// </summary>
    public sealed class SonarFxFeature : ScriptableRendererFeature
    {
        public SonarFxSettings settings = new SonarFxSettings();

        SonarFxRenderPass _scriptablePass;

        public override void Create()
        {
            // 설정된 머티리얼과 레이어 마스크를 이용하여 Render Pass를 생성합니다.
            _scriptablePass = new SonarFxRenderPass(settings.sonarMaterial, settings.layerMask);
            _scriptablePass.renderPassEvent = settings.renderPassEvent;
        }

        // 각 카메라의 렌더링 설정 시 호출되어 Render Pass를 추가합니다.
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (settings.sonarMaterial == null)
            {
                Debug.LogWarning("SonarFxFeature: sonarMaterial이 할당되지 않았습니다.");
                return;
            }
            renderer.EnqueuePass(_scriptablePass);
        }

        protected override void Dispose(bool disposing)
        {
            // 필요한 경우 여기서 리소스를 해제합니다.
        }

        /// <summary>
        /// SonarFxRenderPass : 지정한 레이어에 속한 오브젝트들을 override material을 사용하여 렌더링합니다.
        /// </summary>
        class SonarFxRenderPass : ScriptableRenderPass
        {
            private Material _sonarMaterial;
            private FilteringSettings _filteringSettings;
            private ProfilingSampler _profilingSampler = new ProfilingSampler("SonarFxRenderPass");

            public SonarFxRenderPass(Material sonarMaterial, LayerMask layerMask)
            {
                _sonarMaterial = sonarMaterial;
                // FilteringSettings를 사용하여 특정 레이어의 오브젝트만 선택합니다.
                _filteringSettings = new FilteringSettings(RenderQueueRange.all, layerMask);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                // CommandBuffer 생성
                CommandBuffer cmd = CommandBufferPool.Get("SonarFxRenderPass");

                using (new ProfilingScope(cmd, _profilingSampler))
                {
                    // 커맨드 버퍼 초기화 후 실행
                    context.ExecuteCommandBuffer(cmd);
                    cmd.Clear();

                    // DrawingSettings 생성 (기본 ShaderTag "UniversalForward" 사용)
                    DrawingSettings drawingSettings = CreateDrawingSettings(
                        new ShaderTagId("UniversalForward"), ref renderingData, SortingCriteria.CommonOpaque);

                    // overrideMaterial을 지정하여 해당 오브젝트들을 소나 효과 머티리얼로 렌더링합니다.
                    drawingSettings.overrideMaterial = _sonarMaterial;

                    // FilteringSettings를 이용하여 지정한 레이어의 오브젝트들만 그립니다.
                    context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref _filteringSettings);
                }

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }
        }
    }
}
