using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    public Transform cameraMoveTo;
    public Transform cameraPointTo;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, cameraMoveTo.position, Time.deltaTime * 10.0f);
        transform.LookAt(cameraPointTo.position,cameraPointTo.up);
    }
}
