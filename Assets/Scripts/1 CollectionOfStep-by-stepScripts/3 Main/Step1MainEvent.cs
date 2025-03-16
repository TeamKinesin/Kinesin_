using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class Step1MainEvent : MonoBehaviour
{
    /*
    void Start()
    {
        // 예시: 3초 뒤에 다음 코드를 실행
        StartCoroutine(WaitAndExecute(3f));
    }
    */
    [Header("조건들 전부 false")]
    [SerializeField] public bool brushTransform;
    [SerializeField] public bool mesh3DPenOff;
    [SerializeField] public bool playerTransform;

    [Header("게임오브젝트")]
    [SerializeField] private GameObject brush;

    [Header("드로잉 오브젝트 Script")]
    [SerializeField] private Mesh3DPen mesh3DPen;

    [Header("Transform")]
    [SerializeField] private Transform newTransform;

    [Header("첫 이벤트 설정")]
    [SerializeField] private float noDrawingTime = 10.0f;
    [SerializeField] private float DrawingTime = 20.0f;
    public bool firstFinished = false; 

    [Header("두 번쨰 이벤트 설정")]
    //타임라인 실행되게.
    public PlayableDirector playableDirector;
    public bool secondFinished = false;

    

    [Header("세 번쨰 이벤트 설정")]
    public Teleport teleport1;
  
    // 지정된 초만큼 기다린 뒤 코드를 실행하는 코루틴
    private IEnumerator FirstEvent(float waitTime){
        // waitTime(초)만큼 대기
        yield return new WaitForSeconds(waitTime);
        mesh3DPen.drawing = false;
        brush.SetActive(false);
        brush.transform.position = newTransform.position;
        brush.transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
        yield return new WaitForSeconds(waitTime);
        mesh3DPen.drawing = true;
        brush.SetActive(true);
        firstFinished = true;
    }

    private IEnumerator SecondEvent(){
        secondFinished = false;
        // waitTime(초)만큼 대기
        yield return new WaitForSeconds(DrawingTime);

        playableDirector.Play();
        mesh3DPen.drawing = false;

        yield return new WaitForSeconds(noDrawingTime);

        mesh3DPen.drawing = true;
        secondFinished = true;
    }

    private IEnumerator ThirdEvent(){
        yield return new WaitForSeconds(1f);
        teleport1.MoveThroughPortal();
    }

    void Start(){
        brushTransform = false;
        mesh3DPenOff = false;
        playerTransform = false;
    }

    void Update()
    {
        //붓 트랜스폼 바꾸기
        if (brushTransform){
            StartCoroutine(FirstEvent(2f));
            brushTransform = false;
        }
        //붓 안그려짐.
        if (mesh3DPenOff){
            StartCoroutine(SecondEvent());
            mesh3DPenOff = false;
        }

        if (playerTransform){
            StartCoroutine(ThirdEvent());
            playerTransform = false;
        }
    }
}
