using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class FadeSettings {
    // 페이드 아웃 지속 시간 (초 단위)
    public float fadeDuration = 1.5f;
}

public class Step1SonarEvent : MonoBehaviour
{
    // 이것이 켜지면 실행.
    public DialogueSystem checkDialogueFinish;
    public UnityEvent[] SonarEvent;

    [Header("Fade Out 설정")]
    // 페이드 아웃 효과를 적용할 게임오브젝트 배열 (Stylized Toon 쉐이더 사용)
    public GameObject[] fadeOutObjects;
    // Fade 효과 관련 설정 자료형
    public FadeSettings fadeSettings;

    // 이미 페이드 아웃과 SonarEvent가 실행되었는지 여부 (중복 실행 방지)
    private bool hasTriggered = false;

    void Update()
    {
        // DialogueSystem이 종료되면 한 번만 실행
        if (!hasTriggered && checkDialogueFinish.dialogueFinished)
        {
            hasTriggered = true;
            StartCoroutine(FadeOutAndTrigger());
        }
    }

    IEnumerator FadeOutAndTrigger()
    {
        // 각 오브젝트의 Renderer와 원래 _Color 값을 저장할 딕셔너리 생성
        List<Renderer> rendererList = new List<Renderer>();
        Dictionary<Material, Color> originalColors = new Dictionary<Material, Color>();

        // 대상 오브젝트들의 머티리얼 인스턴스를 확보하고, 투명 렌더링 설정 및 원래 _Color 저장
        foreach (GameObject obj in fadeOutObjects)
        {
            if (obj == null)
                continue;

            Renderer rend = obj.GetComponent<Renderer>();
            if (rend != null)
            {
                // rend.materials 반환 시 인스턴스가 생성됩니다.
                Material[] mats = rend.materials;
                // 재할당하여 각 오브젝트가 개별 머티리얼을 갖도록 함
                rend.materials = mats;
                rendererList.Add(rend);

                foreach (Material mat in mats)
                {
                    if (mat.HasProperty("_Color"))
                    {
                        // 투명 블렌딩 설정 (쉐이더가 이를 지원해야 합니다)
                        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        mat.SetInt("_ZWrite", 0);
                        mat.DisableKeyword("_ALPHATEST_ON");
                        mat.EnableKeyword("_ALPHABLEND_ON");
                        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        mat.renderQueue = 3000;

                        // 원래 _Color 값을 저장 (알파값 1로 초기화)
                        Color orig = mat.GetColor("_Color");
                        orig.a = 1f;
                        mat.SetColor("_Color", orig);
                        originalColors[mat] = orig;
                    }
                }
            }
        }

        float timer = 0f;
        // fadeDuration 동안 매 프레임 _Color의 알파값을 선형 보간(Lerp)로 1에서 0으로 변경
        while (timer < fadeSettings.fadeDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / fadeSettings.fadeDuration);
            float newAlpha = Mathf.Lerp(1f, 0f, t);

            foreach (Renderer rend in rendererList)
            {
                Material[] mats = rend.materials;
                foreach (Material mat in mats)
                {
                    if (mat.HasProperty("_Color") && originalColors.ContainsKey(mat))
                    {
                        Color orig = originalColors[mat];
                        Color newColor = orig;
                        newColor.a = newAlpha;
                        mat.SetColor("_Color", newColor);
                    }
                }
            }
            yield return null;
        }

        // 모든 오브젝트의 _Color 알파값을 확실히 0으로 설정
        foreach (Renderer rend in rendererList)
        {
            Material[] mats = rend.materials;
            foreach (Material mat in mats)
            {
                if (mat.HasProperty("_Color"))
                {
                    Color newColor = mat.GetColor("_Color");
                    newColor.a = 0f;
                    mat.SetColor("_Color", newColor);
                }
            }
        }

        // 페이드 아웃 효과가 완료된 후 SonarEvent 실행
        for (int i = 0; i < SonarEvent.Length; i++)
        {
            SonarEvent[i].Invoke();
        }
    }
}
