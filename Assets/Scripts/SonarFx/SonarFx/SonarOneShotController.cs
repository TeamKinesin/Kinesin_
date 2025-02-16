using UnityEngine;

public class SonarOneShotController : MonoBehaviour
{
    public Material sonarMaterial; // "Custom/SonarOneShotFX" 셰이더를 사용 중인 머티리얼
    private bool isTriggered = false;

    void Update()
    {
        // 예시: 스페이스바를 누르면 파동 트리거
        if(Input.GetKeyDown(KeyCode.Space))
        {
            TriggerSonar();
        }
    }

    public void Start()
    {
        // 머티리얼에 _SonarWaveStartTime 세팅
        sonarMaterial.SetFloat("_SonarWaveStartTime", 999999.0f);
    }
    public void TriggerSonar()
    {
        // 현재 게임 시간
        float currentTime = Time.time;

        // 머티리얼에 _SonarWaveStartTime 세팅
        sonarMaterial.SetFloat("_SonarWaveStartTime", currentTime);

        isTriggered = true;
    }
}
