using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseDialogue : MonoBehaviour
{
    DialogueSystem dialogueSystem;
    // Start is called before the first frame update
    void Start()
    {
        dialogueSystem = GetComponent<DialogueSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (dialogueSystem.dialogueFinished)
        {
            gameObject.SetActive(false);
        }
    }
}
