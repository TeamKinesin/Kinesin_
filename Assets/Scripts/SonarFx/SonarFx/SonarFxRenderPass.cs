using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering; // GraphicsFormat 관련

namespace URPGlitch.Runtime.SonarGlitch
{
    /// <summary>
    /// 포스트프로세싱 패스(AfterRenderingPostProcessing)에서 실행되는 Sonar FX Render Pass
    /// AnalogGlitchRenderPass와 유사한 구조입니다.
    /// </summary>
    sealed class SonarFxRenderPass : ScriptableRenderPass, IDisposable
    {
        const string RenderPassName = "SonarFx RenderPass";

        // 쉐이더에서 사용하는 프로퍼티 ID (쉐이더와 이름 일치)
        static readonly int MainTexID         = Shader.PropertyToID("_MainTex");
        static readonly int SonarBaseColorID  = Shader.PropertyToID("_SonarBaseColor");
        static readonly int SonarWaveColorID  = Shader.PropertyToID("_SonarWaveColor");
        static readonly int SonarWaveParamsID = Shader.PropertyToID("_SonarWaveParams"); // (amplitude, exponent, interval, speed)
        static readonly int SonarWaveVectorID = Shader.PropertyToID("_SonarWaveVector"); // direction 또는 origin
        static readonly int SonarAddColorID   = Shader.PropertyToID("_SonarAddColor");

        readonly ProfilingSampler _profilingSampler;
        readonly Material _sonarMaterial;
        readonly SonarFxVolume _volume; // Volume Component에서 파라미터를 받아옴

        // 임시 RT용 RTHandle과 ID
        RTHandle _mainFrame;
        int _mainFrameID;

        // 효과 활성 여부 (Volume Component의 IsActive 플래그 사용)
        bool isActive =>
            _sonarMaterial != null &&
            _volume != null &&
            _volume.IsActive;

        public SonarFxRenderPass(Shader shader)
        {
            try
            {
                // 포스트프로세싱 패스로 지정 (AnalogGlitch와 동일)
                renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
                _profilingSampler = new ProfilingSampler(RenderPassName);
                _sonarMaterial = CoreUtils.CreateEngineMaterial(shader);

                // VolumeManager를 통해 SonarFxVolume Component를 가져옵니다.
                var volumeStack = VolumeManager.instance.stack;
                _volume = volumeStack.GetComponent<SonarFxVolume>();

                // 임시 RT ID 생성 (쉐이더 프로퍼티 이름과 동일)
                _mainFrameID = Shader.PropertyToID("_MainFrame");

                // RTHandle 생성 (전체 해상도, Bilinear 필터, 32비트 색상 포맷)
                _mainFrame = RTHandles.Alloc(
                    scaleFactor: Vector2.one,
                    filterMode: FilterMode.Bilinear,
                    colorFormat: GraphicsFormat.R8G8B8A8_UNorm,
                    useDynamicScale: true,
                    name: "_MainFrame"
                );
            }
            catch (NullReferenceException)
            {
                // VolumeManager나 다른 참조가 null인 경우
                return;
            }
        }

        public void Dispose()
        {
            CoreUtils.Destroy(_sonarMaterial);
            RTHandles.Release(_mainFrame);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // 포스트프로세싱 활성 여부와 Scene 뷰 카메라 제외 처리
            bool isPostProcessEnabled = renderingData.cameraData.postProcessEnabled;
            bool isSceneViewCamera = renderingData.cameraData.isSceneViewCamera;
            if (!isActive || !isPostProcessEnabled || isSceneViewCamera)
            {
                return;
            }

            CommandBuffer cmd = CommandBufferPool.Get(RenderPassName);
            cmd.Clear();

            using (new ProfilingScope(cmd, _profilingSampler))
            {
                // 카메라의 현재 컬러 타깃 핸들 사용
                var source = renderingData.cameraData.renderer.cameraColorTargetHandle;
                var descriptor = renderingData.cameraData.cameraTargetDescriptor;
                descriptor.depthBufferBits = 0;

                // 임시 RT 할당
                cmd.GetTemporaryRT(_mainFrameID, descriptor);
                // 현재 화면을 임시 RT로 복사
                cmd.Blit(source, (RenderTargetIdentifier)_mainFrame);

                // Volume Component에서 Sonar FX 파라미터 읽기  
                // SonarFxVolume에 아래와 같은 필드들이 정의되어 있어야 합니다.
                //   - public ColorParameter baseColor;
                //   - public ColorParameter waveColor;
                //   - public ClampedFloatParameter waveAmplitude;
                //   - public ClampedFloatParameter waveExponent;
                //   - public ClampedFloatParameter waveInterval;
                //   - public ClampedFloatParameter waveSpeed;
                //   - public ColorParameter addColor;
                //   - public EnumParameter mode;         // Directional 또는 Spherical
                //   - public Vector3Parameter direction;
                //   - public Vector3Parameter origin;
                Color baseColor  = _volume.baseColor.value;
                Color waveColor  = _volume.waveColor.value;
                float amplitude  = _volume.waveAmplitude.value;
                float exponent   = _volume.waveExponent.value;
                float interval   = _volume.waveInterval.value;
                float speed      = _volume.waveSpeed.value;
                Color addColor   = _volume.addColor.value;
                bool spherical   = (_volume.mode.value == SonarFxVolume.SonarMode.Spherical);
                Vector3 waveVector = spherical ? _volume.origin.value : _volume.direction.value.normalized;

                // Material에 파라미터 설정
                _sonarMaterial.SetColor(SonarBaseColorID, baseColor);
                _sonarMaterial.SetColor(SonarWaveColorID, waveColor);
                _sonarMaterial.SetColor(SonarAddColorID, addColor);
                _sonarMaterial.SetVector(SonarWaveParamsID, new Vector4(amplitude, exponent, interval, speed));
                _sonarMaterial.SetVector(SonarWaveVectorID, waveVector);

                // 모드에 따라 쉐이더 키워드 활성화/비활성화 (쉐이더에서 분기 처리)
                if (spherical)
                    _sonarMaterial.EnableKeyword("SONAR_SPHERICAL");
                else
                    _sonarMaterial.DisableKeyword("SONAR_SPHERICAL");

                // 임시 RT를 GlobalTexture로 지정한 후, Sonar Material을 사용해 최종 화면에 블릿
                cmd.SetGlobalTexture(MainTexID, (RenderTargetIdentifier)_mainFrame);
                cmd.Blit((RenderTargetIdentifier)_mainFrame, source, _sonarMaterial);

                // 할당한 임시 RT 해제
                cmd.ReleaseTemporaryRT(_mainFrameID);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
