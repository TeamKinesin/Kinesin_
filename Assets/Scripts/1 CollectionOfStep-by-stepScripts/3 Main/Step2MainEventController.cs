using System.Collections;
using UnityEngine;

public class Step2MainEventController : MonoBehaviour
{
    [SerializeField] private Step1MainEvent step1MainEvent;

    private void StartMainEvent()
    {
        // Start()에서 한 번만 코루틴 시작
        StartCoroutine(EventSequence());
    }

    private IEnumerator EventSequence()
    {
        // 1) 첫 번째 이벤트(붓 트랜스폼)
        //    brushTransform = true를 통해 Step1MainEvent의 FirstEvent 코루틴이 동작하도록 설정
        step1MainEvent.brushTransform = true;

        // 1-1) 첫 번째 이벤트가 끝날 때까지 대기
        yield return new WaitUntil(() => step1MainEvent.firstFinished);

        // 필요하다면 여기서 몇 초 대기 (ex. 10초)
        yield return new WaitForSeconds(10f);

        // 2) 두 번째 이벤트(mesh3DPenOff)
        step1MainEvent.mesh3DPenOff = true;

        // 2-1) 두 번째 이벤트 끝날 때까지 대기
        yield return new WaitUntil(() => step1MainEvent.secondFinished);

        step1MainEvent.mesh3DPenOff = true;

        yield return new WaitUntil(() => step1MainEvent.secondFinished);

        step1MainEvent.mesh3DPenOff = true;

        yield return new WaitUntil(() => step1MainEvent.secondFinished);

        // 필요하다면 여기서 몇 초 대기
        yield return new WaitForSeconds(20f);

        // 3) 세 번째 이벤트(playerTransform)
        step1MainEvent.playerTransform = true;

        // 이후 로직(teleport가 끝나는 것까지 기다린다든지)을 이어서 작성
        // ex) yield return new WaitForSeconds(3f);
        // 등등
    }
}
