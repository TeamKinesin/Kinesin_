using System.Collections;
using UnityEngine;
using TMPro;

public class IntroText : MonoBehaviour
{
    private TMP_Text textMesh;

    [Header("페이드 효과 설정")]
    [SerializeField] private float fadeInDuration = 5f;    // 페이드 인에 걸리는 시간
    [SerializeField] private float stayDuration = 2f;        // 텍스트가 완전히 보인 상태로 유지되는 시간
    [SerializeField] private float fadeOutDuration = 1.3f;     // 페이드 아웃에 걸리는 시간
    
     [Header("인트로 진행후 코드")] 
    [SerializeField] GameObject introOff;
    [SerializeField] GameObject TutorialOn; 
    [SerializeField] GameObject OffFollowText;
    [SerializeField] DialogueSystem OffFollowDialogueText;

    void Start()
    {
        textMesh = GetComponent<TMP_Text>();
        // 초기 투명도 0으로 설정
        Color c = textMesh.color;
        c.a = 0f;
        textMesh.color = c;

        StartCoroutine(FadeInAndOut());
    }

    IEnumerator FadeInAndOut()
    {
        // 페이드 인
        float timer = 0f;
        while (timer < fadeInDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Clamp01(timer / fadeInDuration);
            SetAlpha(alpha);
            yield return null;
        }
        // 완전히 보이도록 설정
        SetAlpha(1f);

        // 일정 시간 텍스트 유지
        yield return new WaitForSeconds(stayDuration);

        // 페이드 아웃
        timer = 0f;
        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;
            float alpha = 1f - Mathf.Clamp01(timer / fadeOutDuration);
            SetAlpha(alpha);
            yield return null;
        }
        // 완전히 안보이도록 설정
        SetAlpha(0f);

        //-> 진행코드를 여기다 적을게요.
         // 대기 시간 추가 (예: 2초)
        OffFollowDialogueText.enabled = false;
        OffFollowText.SetActive(false);
         
        yield return new WaitForSeconds(2f);

        // 진행 코드
        gameObject.SetActive(false);
        introOff.SetActive(false);
        TutorialOn.SetActive(true);
        OffFollowDialogueText.enabled = false;
        OffFollowText.SetActive(false);
    }

    // 텍스트의 알파값을 설정하는 헬퍼 함수
    private void SetAlpha(float alpha)
    {
        Color c = textMesh.color;
        c.a = alpha;
        textMesh.color = c;
    }
}
