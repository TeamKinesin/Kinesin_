using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Step3Sonar : MonoBehaviour
{
    [SerializeField] Step2FadeOutWhite checkFadeOutWhite;
    [SerializeField] SonarOneShotController sonarOneShotController;
    [SerializeField] public bool step3EventDone = false;

    // 이미 소나를 실행했는지 여부를 추적할 플래그
    private bool hasTriggered = false;

    void Update()
    {
        // 아직 소나가 실행된 적이 없고, Step2 이벤트가 완료되었으면...
        if (!hasTriggered && checkFadeOutWhite.checkStep2EventDone)
        {
            // 소나 1회 실행
            sonarOneShotController.TriggerSonar();
            //step3 끝
            step3EventDone = true;
            // 이후로는 다시 실행되지 않도록 갱신
            hasTriggered = true;
        }
    }
}
