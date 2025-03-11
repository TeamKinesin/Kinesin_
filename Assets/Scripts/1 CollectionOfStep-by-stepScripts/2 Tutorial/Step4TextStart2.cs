using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Step4TextStart2 : MonoBehaviour
{
    // Start is called before the first 
    [SerializeField] Step3Sonar step3Sonar;
    private bool hasTriggered = false;

    [SerializeField] DialogueSystem dialogueSystem;

    [Header("1번이 끄는거 2번이 키는거")]
    [SerializeField] GameObject DirectLight1;
    [SerializeField] GameObject DirectLight2; 
   

    // Update is called once per frame
    void Update()
    {
        if (!hasTriggered && step3Sonar.step3EventDone){
            DirectLight1.SetActive(false);
            DirectLight2.SetActive(true);
            hasTriggered = true;
            dialogueSystem.enabled = true;
        }
    }
}
