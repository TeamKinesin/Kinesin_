using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Deligate : MonoBehaviour
{
    public UnityEvent onInputSpace;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        onInputSpace.Invoke();
    }
}
