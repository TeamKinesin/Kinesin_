using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    [Tooltip("해당 Mesh3DPenLine 붙이세요.")]
    public Mesh3DPenLine penTool;

    private Collider _collider;

    void Start()
    {
        // Collider 설정
        _collider = GetComponent<Collider>();
        if (_collider != null) _collider.isTrigger = true;
        
            penTool = GetComponent<Mesh3DPenLine>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (penTool == null) return;

        // PaletColor 컴포넌트를 가진 오브젝트와 충돌했는지 확인
        PaletteColor paletteColor = other.GetComponent<PaletteColor>();
        Debug.Log("충돌.");
        if (paletteColor != null)
        {
            // 펜 도구의 색상 인덱스를 업데이트
            penTool.SetColor((int)paletteColor.Pcolor);
            // 색상 변경 로그 (디버깅용)
            Debug.Log("펜 색상이 해당색 " + paletteColor.Pcolor + "으로 변경");
        }
    }
}
