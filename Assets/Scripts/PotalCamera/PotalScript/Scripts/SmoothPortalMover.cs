using System.Collections;
using UnityEngine;

public class SmoothPortalMover : MonoBehaviour
{
    [Header("이동할 오브젝트 및 포탈 설정")]
    public GameObject objectToMove;   // 인스펙터에서 이동할 게임 오브젝트를 지정
    public Transform inPortal;        // 입구 포탈 Transform
    public Transform outPortal;       // 출구 포탈 Transform

    [Header("이동 시간 (초)")]
    public float moveDuration = 1.0f; // 포탈을 통과하는 동안 걸리는 시간

    [Header("포탈 충돌 감지용 콜라이더")]
    public BoxCollider portalCollider;  // 인스펙터에서 포탈 충돌용 박스콜라이더를 지정

    private bool isMoving = false;

    // 외부에서 호출하면 부드러운 이동을 시작합니다.
    public void MoveThroughPortal()
    {
        Debug.Log("텔레포트 실행");
        if (!isMoving)
            StartCoroutine(SmoothMove());
    }

    private IEnumerator SmoothMove()
    {
        isMoving = true;
        float elapsedTime = 0f;

        // 시작 위치와 회전
        Vector3 startPos = objectToMove.transform.position;
        Quaternion startRot = objectToMove.transform.rotation;

        // 입구 포탈 좌표계로 변환 후, 출구 포탈 좌표계로 변환 (half turn 없이 그대로 사용)
        Vector3 relativePos = inPortal.InverseTransformPoint(startPos);
        Vector3 targetPos = outPortal.TransformPoint(relativePos);

        Quaternion relativeRot = Quaternion.Inverse(inPortal.rotation) * startRot;
        Quaternion targetRot = outPortal.rotation * relativeRot;

        // objectToMove에 직접 혹은 자식에 Camera가 있다면, 즉시 텔레포트(순간 이동) 처리
        if (objectToMove.GetComponent<Camera>() != null || objectToMove.GetComponentInChildren<Camera>() != null)
        {
            objectToMove.transform.position = targetPos;
            objectToMove.transform.rotation = targetRot;
            isMoving = false;
            yield break;
        }

        // 지정한 시간동안 보간하며 이동
        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / moveDuration);

            objectToMove.transform.position = Vector3.Lerp(startPos, targetPos, t);
            objectToMove.transform.rotation = Quaternion.Slerp(startRot, targetRot, t);

            yield return null;
        }

        // 보간이 끝난 후 최종 위치와 회전 보정
        objectToMove.transform.position = targetPos;
        objectToMove.transform.rotation = targetRot;

        isMoving = false;
    }

    // 박스콜라이더와 충돌하면 자동으로 MoveThroughPortal()을 호출합니다.
    void OnTriggerEnter(Collider other)
    {
        if (other == portalCollider)
        {
            MoveThroughPortal();
        }
    }
}
