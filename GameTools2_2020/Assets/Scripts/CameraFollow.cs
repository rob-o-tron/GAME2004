using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform cameraMoveTo;
    public Transform cameraPointTo;
    private bool targetsSet = false;

    // Update is called once per frame
    void Update()
    {
        if (targetsSet)
        {
            transform.position = Vector3.Lerp(transform.position, cameraMoveTo.position, Time.deltaTime * 10.0f);
            transform.LookAt(cameraPointTo.position,cameraPointTo.up);
        }
    }

    public void SetTargets(Transform player, Transform cameraman)
    {
        cameraMoveTo = cameraman;
        cameraPointTo = player;
        targetsSet = true;
    }
}
