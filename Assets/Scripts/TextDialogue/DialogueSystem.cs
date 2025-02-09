using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.Events;

[System.Serializable]
public class Dialogue {
    [TextArea]
    public string dialogue;
}

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txt_Dialogue;
    [SerializeField] private Dialogue[] dialogue;
    public XRNode inputSource;
    public UnityEngine.XR.Interaction.Toolkit.InputHelpers.Button inputButton;
    public float inputThreshold = 0.1f;

    // 자동 진행 간격 (초 단위)
    public float dialogueInterval = 3.0f;
    private float dialogueTimer = 0f;

    private bool isDialogue = false;
    private int count = 0; 

    // 버튼의 상승 에지(버튼을 처음 눌렀을 때) 감지를 위한 변수
    private bool wasPressed = false; 

    public void ShowDialogue() {
        txt_Dialogue.gameObject.SetActive(true);
        count = 0;
        isDialogue = true;
        dialogueTimer = 0f;
        NextDialogue();
    }

    private void HideDialogue(){
        txt_Dialogue.gameObject.SetActive(false);
        isDialogue = false;
    }

    private void NextDialogue(){
        // 만약 아직 남은 대사가 있다면
        if (count < dialogue.Length)
        {
            txt_Dialogue.text = dialogue[count].dialogue;
            count++;
        }
        else
        {
            HideDialogue();
        }
    }

    void Start(){
        // 임시: 시작하자마자 대화창 보이기
        ShowDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        // XR 디바이스의 버튼 입력 상태 확인
        bool isPressed = false;
        UnityEngine.XR.Interaction.Toolkit.InputHelpers.IsPressed(
            InputDevices.GetDeviceAtXRNode(inputSource),
            inputButton,
            out isPressed,
            inputThreshold
        );

        if (isDialogue)
        {
            // 버튼 입력의 상승 에지 (새롭게 눌린 순간) 감지
            bool buttonDown = isPressed && !wasPressed;

            // 타이머 업데이트
            dialogueTimer += Time.deltaTime;

            // 버튼이 새롭게 눌리거나, 일정 시간(dialogueInterval)이 지나면 대사 진행
            if (buttonDown || dialogueTimer >= dialogueInterval)
            {
                // 타이머 초기화
                dialogueTimer = 0f;
                NextDialogue();
            }

            // 현재 프레임의 버튼 상태를 저장 (다음 프레임 비교용)
            wasPressed = isPressed;
        }
    }
}
