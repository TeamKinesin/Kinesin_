using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Step4TextStart2 : MonoBehaviour
{
    // Start is called before the first 
    [SerializeField] Step3Sonar step3Sonar;
    private bool hasTriggered = false;

    [SerializeField] DialogueSystem dialogueSystem;
   

    // Update is called once per frame
    void Update()
    {
        if (!hasTriggered && step3Sonar.step3EventDone){
            hasTriggered = true;
            dialogueSystem.enabled = true;
        }
    }
}
