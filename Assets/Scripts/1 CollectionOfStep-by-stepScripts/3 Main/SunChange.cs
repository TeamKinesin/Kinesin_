using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunChange : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject sun1;
    public GameObject sun2;

    

    // Update is called once per frame
    public void ChangeSun(){
        sun1.SetActive(false);
        sun2.SetActive(true);
        
    }
}
