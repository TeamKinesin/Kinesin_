using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Step2FadeOutWhite : MonoBehaviour
{
    //Step 2 는 유니티 이벤트 내에서 작동

    [Header("Fade 설정")]
    public float fadeInDuration = 2f;    // 페이드 인 시간
    public float stayDuration = 1f;        // 유지 시간
    public float fadeOutDuration = 2f;     // 페이드 아웃 시간
    [SerializeField] public bool checkStep2EventDone= false; 

    [Header("대상 머티리얼")]
    // 인스펙터에서 할당할 머티리얼 (URP Unlit 쉐이더 사용, _BaseColor 프로퍼티가 있어야 함)
    public Material targetMaterial;
    
    public GameObject[] structureObject;

    public GameObject[] sonarObjectOn;

    void Start()
    {
        if (targetMaterial == null || !targetMaterial.HasProperty("_BaseColor"))
        {
            Debug.LogWarning("targetMaterial이 할당되지 않았거나 _BaseColor 프로퍼티가 없습니다.");
            return;
        }

        // 머티리얼이 투명 블렌딩을 사용할 수 있도록 설정
        targetMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        targetMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        targetMaterial.SetInt("_ZWrite", 0);
        targetMaterial.renderQueue = 3000;

        StartCoroutine(FadeInAndOut());
    }

    IEnumerator FadeInAndOut()
    {
        // 원래 _BaseColor 값 저장 (알파값은 1로 가정)
        Color origColor = targetMaterial.GetColor("_BaseColor");
        origColor.a = 1f;

        // 초기 상태: 알파 0 (완전 투명)
        Color newColor = origColor;
        newColor.a = 0f;
        targetMaterial.SetColor("_BaseColor", newColor);

        // 페이드 인: 0 -> 1
        float timer = 0f;
        while (timer < fadeInDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / fadeInDuration);
            float alpha = Mathf.Lerp(0f, 1f, t);
            newColor.a = alpha;
            targetMaterial.SetColor("_BaseColor", newColor);
            yield return null;
        }
        newColor.a = 1f;
        targetMaterial.SetColor("_BaseColor", newColor);

        // 유지 시간 대기
        yield return new WaitForSeconds(stayDuration);
        
        // 게임 오브젝트 오프
        for (int i=0; i<structureObject.Length; i++){
            structureObject[i].SetActive(false);
        }

        // 소나 게임 오브젝트 오픈
        for (int i=0; i<sonarObjectOn.Length; i++){
            sonarObjectOn[i].SetActive(true);
        }
        checkStep2EventDone = true;

        // 페이드 아웃: 1 -> 0
        
        timer = 0f;
        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / fadeOutDuration);
            float alpha = Mathf.Lerp(1f, 0f, t);
            newColor.a = alpha;
            targetMaterial.SetColor("_BaseColor", newColor);
            yield return null;
        }
        newColor.a = 0f;
        targetMaterial.SetColor("_BaseColor", newColor);
        gameObject.SetActive(false);
    }
}
