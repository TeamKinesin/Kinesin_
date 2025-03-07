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
    [SerializeField] public bool startCondition;
    [SerializeField] private bool[] eventHandler;

    [Header("게임오브젝트")]
    [SerializeField] private GameObject brush;
    [SerializeField] private GameObject erase;

    [Header("드로잉 오브젝트 Script")]
    [SerializeField] private Mesh3DPen mesh3DPen;

    [Header("Transform")]
    [SerializeField] private Transform newTransform;


    // 지정된 초만큼 기다린 뒤 코드를 실행하는 코루틴

    private IEnumerator FirstEvent(float waitTime){
        // waitTime(초)만큼 대기
        yield return new WaitForSeconds(waitTime);
        
        brush.transform.position = newTransform.position;
        erase.transform.position = newTransform.position;
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
        startCondition = false;
        for (int i =0 ; i<eventHandler.Length; i++){
            eventHandler[i] = false;
        }   
    }

    void Update()
    {
        if (startCondition){
            StartCoroutine(FirstEvent(2f));
            startCondition = false;
        }

        if (eventHandler[0])
        {
            StartCoroutine(SecondEvent(2f));
            eventHandler[0] = false;
        }
    }
}
