using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowDialogue : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float distance = 3.0f;
    private bool isCentered = false;
    
    // 오브젝트가 화면에서 사라지면 isCentered를 false로 설정
    private void OnBecameInvisible(){
        isCentered = false;
    }
    
    private void Update()
    {
        if (!isCentered)
        {
            Vector3 targetPosition = FindTargetPosition();
            MoveTowards(targetPosition);
            if (ReachedPosition(targetPosition))
            {
                isCentered = true;
            }
        }
        
        // 오브젝트의 앞면이 카메라를 향하도록 회전
        if(cameraTransform != null)
        {
            // LookAt는 기본적으로 오브젝트의 forward (Z축)가 타겟을 향하게 합니다.
            transform.LookAt(cameraTransform);
        }
    }

    private Vector3 FindTargetPosition()
    {
        return cameraTransform.position + (cameraTransform.forward * distance);
    }

    private void MoveTowards(Vector3 targetPosition)
    {
        transform.position += (targetPosition - transform.position) * 0.025f;
    }

    private bool ReachedPosition(Vector3 targetPosition)
    {
        return Vector3.Distance(targetPosition, transform.position) < 0.1f;
    }
}
