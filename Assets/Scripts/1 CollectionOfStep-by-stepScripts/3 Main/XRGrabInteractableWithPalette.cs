using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// "잡히는" Interactable 오브젝트에 붙여,
/// 잡고 있는 동안만 왼/오른손 버튼 상태를 폴링하여 팔레트를 보여주거나 숨기는 예시.
/// </summary>
public class XRGrabInteractableWithPalette : XRGrabInteractable
{
    [Header("XR Node Settings")]
    [SerializeField] private XRNode inputSource1;           // 왼손(예: LeftHand)
    [SerializeField] private InputHelpers.Button inputButton1;
    [SerializeField] private XRNode inputSource2;           // 오른손(예: RightHand)
    [SerializeField] private InputHelpers.Button inputButton2;
    [SerializeField] private float inputThreshold = 0.1f;

    [Header("Palette Objects [반대로 넣어주세요.]")]
    [SerializeField] private GameObject leftPalette;
    [SerializeField] private GameObject rightPalette;

    // 이 오브젝트가 현재 "잡혀" 있는지 여부
    private bool isGrabbed = false;

    /// <summary>
    /// 잡혔을 때(Select Entered) 호출되는 콜백
    /// </summary>
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        // 이 오브젝트가 잡힘
        isGrabbed = true;
    }

    /// <summary>
    /// 놓았을 때(Select Exited) 호출되는 콜백
    /// </summary>
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        // 오브젝트가 놓임
        isGrabbed = false;

        // 놓는 순간 팔레트를 모두 꺼준다
        if (leftPalette != null)
            leftPalette.SetActive(false);

        if (rightPalette != null)
            rightPalette.SetActive(false);
    }

    private void Update()
    {
        // 잡고 있지 않다면 버튼 검사할 필요 없음
        if (!isGrabbed)
            return;

        // 왼손/오른손 버튼 상태를 폴링
        bool isPressed1 = false;
        bool isPressed2 = false;

        // 왼손
        InputHelpers.IsPressed(
            InputDevices.GetDeviceAtXRNode(inputSource1),
            inputButton1,
            out isPressed1,
            inputThreshold
        );

        // 오른손
        InputHelpers.IsPressed(
            InputDevices.GetDeviceAtXRNode(inputSource2),
            inputButton2,
            out isPressed2,
            inputThreshold
        );

        // 왼손 버튼 누르는 동안 leftPalette On, 아니면 Off
        if (leftPalette != null)
            leftPalette.SetActive(isPressed1);

        // 오른손 버튼 누르는 동안 rightPalette On, 아니면 Off
        if (rightPalette != null)
            rightPalette.SetActive(isPressed2);
    }
}
