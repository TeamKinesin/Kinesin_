using System.Collections;
using System.Collections.Generic;
using PDollarGestureRecognizer;
using UnityEngine;

public class Step5DoorGestureOn : MonoBehaviour
{
    [SerializeField] GameObject gesture;
    [SerializeField] DialogueSystem dialogueSystem;
    [SerializeField] MovementRecognizer movementRecognizer;
    [SerializeField] bool hasTrigger = false;

    [Header("문 이펙트")]
    [SerializeField] GameObject memoVFX;
    [SerializeField] GameObject potal;

    [SerializeField] GameObject playerCollider;

    [SerializeField] GameObject followCamera;
    [SerializeField] DialogueSystem dialogueScript1;
    [SerializeField] GameObject drawingMeshPenOn;

    [Header("문 생성 딜레이 (초)")]
    public float doorAppearDelay = 2f;  // 인스펙터에서 설정 가능

    void Start()
    {
        memoVFX.SetActive(false);
        potal.SetActive(false);
        gesture.SetActive(false);
    }

    void Update()
    {
        if (dialogueSystem.dialogueFinished && !hasTrigger)
        {
            followCamera.SetActive(true);
            gesture.SetActive(true);
            dialogueScript1.enabled = true;
            if (movementRecognizer.result.GestureClass == "D" && !hasTrigger)
            {
                //collider 없애기
                playerCollider.SetActive(false);
                //VFX 키기    
                memoVFX.SetActive(true);
                //Gesture 끄기
                gesture.SetActive(false);
                //해당 오브젝의 다이얼로그 스크립트 끄기
                dialogueScript1.enabled = false;
                //팔로우 카메라 끄기;
                followCamera.SetActive(false);
                dialogueScript1.NextDialogue();
                drawingMeshPenOn.SetActive(true);
                //
                hasTrigger = true; // 중복 트리거 방지
                StartCoroutine(ShowDoorAfterDelay());
            }
        }
    }

    IEnumerator ShowDoorAfterDelay()
    {
        yield return new WaitForSeconds(doorAppearDelay);
        potal.SetActive(true);
    }
}
