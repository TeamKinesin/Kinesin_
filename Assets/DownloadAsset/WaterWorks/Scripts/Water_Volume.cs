using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
// ↓ 추가
using UnityEngine.Experimental.Rendering; // 일부 Unity 버전에서 GraphicsFormat을 이 네임스페이스에서 제공


public class Water_Volume : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        public RTHandle source;
        private Material _material;
        private RTHandle tempRenderTarget;
        private RTHandle tempRenderTarget2;

        public CustomRenderPass(Material mat)
        {
            _material = mat;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            // cameraTextureDescriptor의 필요한 정보를 추출해서 RTHandles.Alloc에 넘깁니다.
            int width  = cameraTextureDescriptor.width;
            int height = cameraTextureDescriptor.height;
            // 그래픽 포맷 (URP에서 색상 텍스처가 어떤 포맷으로 렌더링되는지)
            GraphicsFormat colorFormat = cameraTextureDescriptor.graphicsFormat;
            // 3D나 큐브맵 같은 경우가 아니라면 일반적으로 Tex2D
            TextureDimension dimension = (TextureDimension)cameraTextureDescriptor.dimension;

            // 1) 컬러 텍스처용 RTHandle 생성
            tempRenderTarget = RTHandles.Alloc(
                width: width,
                height: height,
                slices: 1,                      // 일반적으로 1 (VR 등 특별 케이스 제외)
                colorFormat: colorFormat,
                depthBufferBits: DepthBits.None, // 컬러 텍스처이므로 Depth 없음
                dimension: dimension,
                filterMode: FilterMode.Bilinear,
                wrapMode: TextureWrapMode.Clamp,
                name: "_TemporaryColourTexture"
            );

            // 2) Depth 텍스처용 RTHandle 생성
            tempRenderTarget2 = RTHandles.Alloc(
                width: width,
                height: height,
                slices: 1,
                colorFormat: colorFormat,         // 원하는 경우 그래픽 포맷을 다르게 해도 무방
                depthBufferBits: DepthBits.Depth32,
                dimension: dimension,
                filterMode: FilterMode.Point,     // 일반적으로 딥스 텍스처는 Point 필터를 많이 사용
                wrapMode: TextureWrapMode.Clamp,
                name: "_TemporaryDepthTexture"
            );

            // 이 패스가 tempRenderTarget 에 렌더링하도록 설정 (필요 시)
            ConfigureTarget(tempRenderTarget);
            ConfigureClear(ClearFlag.All, Color.clear);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType != CameraType.Reflection)
            {
                CommandBuffer cmd = CommandBufferPool.Get("WaterVolumePass");

                // URP 최신 API에 맞게 RTHandle을 사용하는 Blit 오버로드
                Blit(cmd, source, tempRenderTarget, _material);
                Blit(cmd, tempRenderTarget, source);

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            if (tempRenderTarget != null)
            {
                RTHandles.Release(tempRenderTarget);
                tempRenderTarget = null;
            }
            if (tempRenderTarget2 != null)
            {
                RTHandles.Release(tempRenderTarget2);
                tempRenderTarget2 = null;
            }
        }
    }

    [System.Serializable]
    public class _Settings
    {
        public Material material = null;
        public RenderPassEvent renderPass = RenderPassEvent.AfterRenderingSkybox;
    }

    public _Settings settings = new _Settings();
    CustomRenderPass m_ScriptablePass;

    public override void Create()
    {
        if (settings.material == null)
        {
            settings.material = Resources.Load<Material>("Water_Volume");
        }
        m_ScriptablePass = new CustomRenderPass(settings.material);
        m_ScriptablePass.renderPassEvent = settings.renderPass;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // 최신 URP에서 cameraColorTargetHandle은 RTHandle 타입
        m_ScriptablePass.source = renderer.cameraColorTargetHandle;
        renderer.EnqueuePass(m_ScriptablePass);
    }
}
