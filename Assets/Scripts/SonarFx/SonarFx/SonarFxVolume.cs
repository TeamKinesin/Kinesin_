using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable]
public class SonarFxVolume : VolumeComponent
{
    public enum SonarMode
    {
        Directional,
        Spherical
    }

    // enum 전용 VolumeParameter 클래스를 정의합니다.
    [Serializable]
    public sealed class SonarModeParameter : VolumeParameter<SonarMode>
    {
        public SonarModeParameter(SonarMode value, bool overrideState = false)
            : base(value, overrideState) { }
    }

    // 기본 색상 (albedo)
    public ColorParameter baseColor = new ColorParameter(new Color(0.2f, 0.2f, 0.2f, 0));

    // 파형 색상
    public ColorParameter waveColor = new ColorParameter(new Color(1.0f, 0.2f, 0.2f, 0));

    // 파형 색상 진폭
    public ClampedFloatParameter waveAmplitude = new ClampedFloatParameter(2.0f, 0.0f, 10.0f);

    // 파형 색상 지수
    public ClampedFloatParameter waveExponent = new ClampedFloatParameter(22.0f, 0.0f, 100.0f);

    // 파형 간격
    public ClampedFloatParameter waveInterval = new ClampedFloatParameter(20.0f, 0.0f, 100.0f);

    // 파형 속도
    public ClampedFloatParameter waveSpeed = new ClampedFloatParameter(10.0f, 0.0f, 100.0f);

    // 추가 색상 (Emission 등)
    public ColorParameter addColor = new ColorParameter(Color.black);

    // Sonar 효과 모드 (Directional 또는 Spherical)
    public SonarModeParameter mode = new SonarModeParameter(SonarMode.Directional);

    // 파형 진행 방향 (Directional 모드에서 사용)
    public Vector3Parameter direction = new Vector3Parameter(Vector3.forward);

    // 파형 원점 (Spherical 모드에서 사용)
    public Vector3Parameter origin = new Vector3Parameter(Vector3.zero);

    // 효과 활성화 여부 (필요 시 조건 추가 가능)
    public bool IsActive => true;
}
