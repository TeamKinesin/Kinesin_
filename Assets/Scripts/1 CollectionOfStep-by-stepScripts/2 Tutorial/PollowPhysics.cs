using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PollowPhysics : MonoBehaviour
{
    public Transform target;
    Rigidbody _rigidbody;
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        _rigidbody.MovePosition(target.transform.position);
    }
}
