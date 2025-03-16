using System.Collections;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    [Header("이동할 오브젝트 (플레이어 등)")]
    public GameObject objectToMove;   // 인스펙터에서 이동할 게임 오브젝트를 지정

    [Header("도착 지점")]
    public Transform outPortal;       // 출구 포탈 Transform (도착지점으로 사용)

    [Header("이동 시간 (초)")]
    public float moveDuration = 1.0f; // 부드럽게 이동하는 데 걸리는 시간

    private bool isMoving = false;

    /// <summary>
    /// 외부에서 이 함수를 호출하면, 
    /// objectToMove가 outPortal의 위치로 부드럽게 텔레포트(이동) 됩니다.
    /// </summary>
    public void MoveThroughPortal()
    {
        Debug.Log("텔레포트 실행");
        if (!isMoving && outPortal != null && objectToMove != null)
        {
            StartCoroutine(SmoothMove());
        }
    }

    private IEnumerator SmoothMove()
    {
        isMoving = true;
        float elapsedTime = 0f;

        // 시작 위치와 회전
        Vector3 startPos = objectToMove.transform.position;
        Quaternion startRot = objectToMove.transform.rotation;

        // 1) 도착 위치: outPortal의 위치
        // 2) 도착 회전: 현재 회전값 그대로 유지 (startRot)
        Vector3 targetPos = outPortal.position;
        Quaternion targetRot = startRot;

        // 만약 이동할 오브젝트에 카메라가 있다면, 즉시 텔레포트
        if (objectToMove.GetComponent<Camera>() != null ||
            objectToMove.GetComponentInChildren<Camera>() != null)
        {
            objectToMove.transform.position = targetPos;
            objectToMove.transform.rotation = targetRot;
            isMoving = false;
            yield break;
        }

        // 지정한 시간(moveDuration) 동안 부드럽게 보간하며 이동
        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / moveDuration);

            objectToMove.transform.position = Vector3.Lerp(startPos, targetPos, t);
            objectToMove.transform.rotation = Quaternion.Slerp(startRot, targetRot, t);

            yield return null;
        }

        // 보간이 끝난 후, 최종 위치/회전 보정
        objectToMove.transform.position = targetPos;
        objectToMove.transform.rotation = targetRot;

        isMoving = false;
    }
}
