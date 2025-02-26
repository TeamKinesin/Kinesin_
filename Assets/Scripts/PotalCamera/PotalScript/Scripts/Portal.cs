using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Portal : MonoBehaviour
{
    [field: SerializeField]
    public Portal OtherPortal { get; private set; }

    [SerializeField]
    private Renderer outlineRenderer;

    [field: SerializeField]
    public Color PortalColour { get; private set; }

    // 벽에 붙이기 위한 마스크(벽 판정)를 더 이상 사용하지 않는다면 주석 처리
    //[SerializeField]
    //private LayerMask placementMask;

    [SerializeField]
    private Transform testTransform;

    private List<PortalableObject> portalObjects = new List<PortalableObject>();
    public bool IsPlaced { get; private set; } = false;

    // 벽 콜라이더를 따로 추적할 필요가 없다면 주석 처리
    // private Collider wallCollider;

    // Components.
    public Renderer Renderer { get; private set; }
    private new BoxCollider collider;

    private void Awake()
    {
        collider = GetComponent<BoxCollider>();
        Renderer = GetComponent<Renderer>();
    }

    private void Start()
    {
        outlineRenderer.material.SetColor("_OutlineColour", PortalColour);
        // 필요하다면 최초에 비활성화
        // gameObject.SetActive(false);
    }

    private void Update()
    {
        // 상대 포탈이 유효(설치됨)일 때만 렌더러를 보이게 할 것인지 결정
        Renderer.enabled = OtherPortal != null && OtherPortal.IsPlaced;

        for (int i = 0; i < portalObjects.Count; ++i)
        {
            Vector3 objPos = transform.InverseTransformPoint(portalObjects[i].transform.position);
            // z축 양수로 넘어가면 워프
            if (objPos.z > 0.0f)
            {
                portalObjects[i].Warp();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var obj = other.GetComponent<PortalableObject>();
        if (obj != null)
        {
            portalObjects.Add(obj);
            // 벽 콜라이더 사용 부분도 제거하거나 null로 전달
            obj.SetIsInPortal(this, OtherPortal, null);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var obj = other.GetComponent<PortalableObject>();
        if (obj != null && portalObjects.Contains(obj))
        {
            portalObjects.Remove(obj);
            // 마찬가지로 wallCollider 제거
            obj.ExitPortal(null);
        }
    }

    /// <summary>
    /// "어디에나" 포탈을 배치할 수 있도록 간소화.
    /// </summary>
    /// <param name="pos">배치할 위치</param>
    /// <param name="rot">배치할 회전</param>
    /// <returns>배치 성공 여부</returns>
    public bool PlacePortal(Vector3 pos, Quaternion rot)
    {
        // 굳이 testTransform을 쓰지 않고 바로 배치
        transform.position = pos;
        transform.rotation = rot;

        // 만약 “포탈 앞면이 조금 뒤로 밀리는 것”이 필요하다면
        //transform.position -= transform.forward * 0.001f;

        // 기존 벽 판정 로직(Overhang / Intersect / Overlap 등)은 제거

        // 포탈 활성화
        gameObject.SetActive(true);
        IsPlaced = true;

        // 언제나 true를 반환
        return true;
    }

    public void RemovePortal()
    {
        gameObject.SetActive(false);
        IsPlaced = false;
    }
}
