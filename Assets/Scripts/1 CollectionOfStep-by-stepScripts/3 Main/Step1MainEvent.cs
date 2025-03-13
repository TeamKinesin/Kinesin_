using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
    [SerializeField] private bool mesh3DPenOff;

    [Header("게임오브젝트")]
    [SerializeField] private GameObject brush;

    [Header("드로잉 오브젝트 Script")]
    [SerializeField] private Mesh3DPen mesh3DPen;

    [Header("Transform")]
    [SerializeField] private Transform newTransform;


    // 지정된 초만큼 기다린 뒤 코드를 실행하는 코루틴
    private IEnumerator FirstEvent(float waitTime){
        // waitTime(초)만큼 대기
        yield return new WaitForSeconds(waitTime);
        
        brush.SetActive(false);
        brush.transform.position = newTransform.position;
        mesh3DPen.drawing = false;
        yield return new WaitForSeconds(waitTime);
        brush.SetActive(true);
        mesh3DPen.drawing = true;
    }

    private IEnumerator SecondEvent(float waitTime){
        // waitTime(초)만큼 대기
        yield return new WaitForSeconds(waitTime);
        
        mesh3DPen.drawing = false;

        yield return new WaitForSeconds(10f);

        mesh3DPen.drawing = true;
    }

    void Start()
    {
        brushTransform = false;
        mesh3DPenOff = false;
    }

    void Update()
    {
        //
        if (brushTransform){
            StartCoroutine(FirstEvent(2f));
            brushTransform = false;
        }
        //붓사라지게하기 
        if (mesh3DPenOff)
        {
            StartCoroutine(SecondEvent(2f));
            mesh3DPenOff = false;
        }
    }
}
