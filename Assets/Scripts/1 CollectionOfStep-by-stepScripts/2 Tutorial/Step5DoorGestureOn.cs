using System.Collections;
using System.Collections.Generic;
using PDollarGestureRecognizer;
using UnityEngine;

public class Step5DoorGestureOn : MonoBehaviour
{

    [SerializeField] GameObject gestureOn;

    [SerializeField] DialogueSystem dialogueSystem;
    
    [SerializeField] MovementRecognizer movementRecognizer;
    [SerializeField] bool hasTrigger = false;

    // Update is called once per frame
    void Update()
    {
        if  (dialogueSystem.dialogueFinished){
            gestureOn.SetActive(true);
            if (movementRecognizer.result.GestureClass == "D" && !hasTrigger){
                Debug.Log("히히 일단 되는 것 같네요?");
                hasTrigger = true;
            }
        }
    }
}
