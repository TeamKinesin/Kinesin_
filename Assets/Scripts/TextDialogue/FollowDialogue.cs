using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowDialogue : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float distance = 3.0f;

    private void Update()
    {
        // 카메라 기준 목표 위치 계산
        Vector3 targetPosition = FindTargetPosition();
        // 목표 위치로 부드럽게 이동
        MoveTowards(targetPosition);

        // 오브젝트의 앞면이 카메라를 향하도록 회전
        if (cameraTransform != null)
        {
            transform.LookAt(cameraTransform);
        }
    }

    private Vector3 FindTargetPosition()
    {
        return cameraTransform.position + (cameraTransform.forward * distance);
    }

    private void MoveTowards(Vector3 targetPosition)
    {
        // 현재 위치와 목표 위치 사이를 선형 보간(Lerp)으로 부드럽게 이동
        transform.position = Vector3.Lerp(transform.position, targetPosition, 0.025f);
    }
}
