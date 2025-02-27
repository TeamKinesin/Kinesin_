using System.Collections;
using System.Collections.Generic;
using PDollarGestureRecognizer;
using UnityEngine;

public class Step5DoorGestureOn : MonoBehaviour
{
    [SerializeField] GameObject gestureOn;
    [SerializeField] DialogueSystem dialogueSystem;
    [SerializeField] MovementRecognizer movementRecognizer;
    [SerializeField] bool hasTrigger = false;

    [Header("문 이펙트")]
    [SerializeField] GameObject memoVFX;
    [SerializeField] GameObject potal;

    [SerializeField] GameObject playerCollider;

    [Header("문 생성 딜레이 (초)")]
    public float doorAppearDelay = 2f;  // 인스펙터에서 설정 가능

    void Start()
    {
        memoVFX.SetActive(false);
        potal.SetActive(false);
        gestureOn.SetActive(false);
    }

    void Update()
    {
        if (dialogueSystem.dialogueFinished)
        {
            gestureOn.SetActive(true);
            if (movementRecognizer.result.GestureClass == "D" && !hasTrigger)
            {
                playerCollider.SetActive(false);    
                memoVFX.SetActive(true);
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
