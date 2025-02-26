using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CameraMove))]
public class PortalPlacement : MonoBehaviour
{
    [SerializeField]
    private PortalPair portals; // 포탈 페어 (Portals[0], Portals[1])

    [SerializeField]
    private LayerMask layerMask;

    private CameraMove cameraMove;

    [Header("Portal 0 전용 - 배치할 위치/회전")]
    [SerializeField]
    private Transform portal0Placement;  // Portal 0이 놓일 Transform

    [Header("Portal 1 전용 - 배치할 위치/회전")]
    [SerializeField]
    private Transform portal1Placement;  // Portal 1이 놓일 Transform

    // 이제 Portal 0은 Raycast 로직 대신 Inspector에서 지정한 위치로 배치됨
    [Header("Portal 0 Ray 쏠 조건 (미사용)")]
    [SerializeField]
    private bool onRay = false; // 기존 Raycast 조건 (현재는 사용하지 않음)

    public bool PotalOn = false;

    private void Awake()
    {
        cameraMove = GetComponent<CameraMove>();
    }

    private void Start()
    {
        // 게임 시작 시 Portal 0과 Portal 1 모두 지정한 Transform 위치에 배치
        FirePortal(0, transform.position, transform.forward, 250.0f);
        FirePortal(1, transform.position, transform.forward, 250.0f);
    }

    private void Update()
    {
        // 만약 onRay 조건을 사용하고 싶다면 아래 로직으로 한 번만 Portal 0을 배치할 수 있음
        if (onRay && !PotalOn)
        {
            FirePortal(0, transform.position, transform.forward, 250.0f);
            PotalOn = true;
        }
    }

    /// <summary>
    /// portalID에 따라 포탈을 배치하는 로직.
    /// 이제 Portal 0과 Portal 1 모두 Inspector에 지정한 위치/회전으로 배치합니다.
    /// </summary>
    /// <param name="portalID">0 또는 1</param>
    /// <param name="pos">레이 시작 위치 (미사용)</param>
    /// <param name="dir">레이 방향 (미사용)</param>
    /// <param name="distance">레이 사거리 (미사용)</param>
    private void FirePortal(int portalID, Vector3 pos, Vector3 dir, float distance)
    {
        if (portalID == 0)
        {
            if (portal0Placement == null)
            {
                Debug.LogWarning("[Portal 0] portal0Placement가 지정되지 않았습니다!");
                return;
            }
            bool wasPlaced = portals.Portals[0].PlacePortal(
                portal0Placement.position,
                portal0Placement.rotation
            );
            if (wasPlaced)
            {
                Debug.Log("[Portal 0] 지정된 Transform 위치에 포탈 배치 성공");
            }
        }
        else if (portalID == 1)
        {
            if (portal1Placement == null)
            {
                Debug.LogWarning("[Portal 1] portal1Placement가 지정되지 않았습니다!");
                return;
            }
            bool wasPlaced = portals.Portals[1].PlacePortal(
                portal1Placement.position,
                portal1Placement.rotation
            );
            if (wasPlaced)
            {
                Debug.Log("[Portal 1] 지정된 Transform 위치에 포탈 배치 성공");
            }
            else
            {
                Debug.LogWarning("[Portal 1] 배치 실패");
            }
        }
    }
}
