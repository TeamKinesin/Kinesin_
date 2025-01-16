using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using PDollarGestureRecognizer;
using System.IO;
using UnityEngine.Events;

public class MovementRecognizer : MonoBehaviour
{
    public XRNode inputSource;
    public UnityEngine.XR.Interaction.Toolkit.InputHelpers.Button inputButton;
    public float inputThreshold = 0.1f;
    public Transform movementSource;

    public float newPositionThresholdDistance = 0.05f;
    public GameObject debugCubePrefab;
    public bool creationMode = true;
    public string newGestureName;

    public float recognitionThreshold = 0.9f;
   
   [System.Serializable]
    public class UnityStringEvent : UnityEvent<string> { }
    public UnityStringEvent OnRecognized;

    private List<Gesture> trainingSet = new List<Gesture>();
    private bool isMoving = false;
    private List<Vector3> positionList = new List<Vector3>();

   
    void Start()
    {
        string[] gestureFiles = Directory.GetFiles(Application.persistentDataPath, "*.xml");
        foreach (var item in gestureFiles )
        {
            trainingSet.Add(GestureIO.ReadGestureFromFile(item));
        }
    }

    
    void Update()
    {
        UnityEngine.XR.Interaction.Toolkit.InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(inputSource), inputButton, out bool isPressed, inputThreshold);
        
        //start The Movement
        if(!isMoving && isPressed)
        {
            StartMovement();
        }
        //Ending The Movement
        else if(isMoving && !isPressed)
        {
            EndMovement();
        }
        //updating The Movement
        else if(isMoving && isPressed)
        {
            UpdateMovement();
        }
    }

    void StartMovement()
    {
        isMoving = true;
        positionList.Clear();
        positionList.Add(movementSource.position);
        if(debugCubePrefab) 
            Destroy( Instantiate(debugCubePrefab,movementSource.position,Quaternion.identity),3);
    }

    void EndMovement()
    {
        isMoving = false;

        //걍 한국어로 해야겠다. 포지션 리스트로 부터 제스쳐 크리에이팅 
        Point[] pointArray = new Point[positionList.Count];

        for (int i = 0; i< positionList.Count; i++)
        {
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(positionList[i]);
            pointArray[i] = new Point(screenPoint.x,screenPoint.y,0);
        }

        Gesture newGesture = new Gesture(pointArray);

        //새로운 제스쳐의 트레이닝 셋

        if(creationMode)
        {
            newGesture.Name = newGestureName;
            trainingSet.Add(newGesture);

            string fileName = Application.persistentDataPath + "/" + newGestureName + ".xml";
            GestureIO.WriteGesture(pointArray,newGestureName, fileName);
        }
        //recognize
        else
        {
            Result result = PointCloudRecognizer.Classify(newGesture, trainingSet.ToArray());

            if(result.Score > recognitionThreshold)
            {
                Debug.Log(result.GestureClass + result.Score);
                OnRecognized.Invoke(result.GestureClass);
            }
        }
    }

    void UpdateMovement()
    {
        Vector3 lastPosition = positionList[positionList.Count - 1];
        if(Vector3.Distance(movementSource.position,lastPosition) > newPositionThresholdDistance) 
        {
            positionList.Add(movementSource.position);
            if(debugCubePrefab) 
                Destroy( Instantiate(debugCubePrefab,movementSource.position,Quaternion.identity),3);
        }
    }    
}
