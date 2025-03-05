using UnityEngine;
using UnityEngine.Events;

public class TriggerEventArray : MonoBehaviour
{
    [Header("충돌 체크할 대상 콜라이더 [Consumer를 넣자]")]
    public Collider targetCollider;

    [Header("트리거 진입 시 실행할 이벤트 리스트")]
    public UnityEvent[] onTriggerEnterEvents;

    [Header("이벤트가 모두 실행되었는지 여부")]
    public bool isFinished = false;

    private void OnTriggerEnter(Collider other)
    {
        // 지정한 targetCollider와 충돌했는지 확인
        if (other == targetCollider)
        {
            // 모든 이벤트를 순서대로 실행
            foreach (UnityEvent evt in onTriggerEnterEvents)
            {
                evt.Invoke();
            }

            // 모든 이벤트를 실행한 후 isFinished = true로 설정
            isFinished = true;
            gameObject.SetActive(false);
        }
    }
}
