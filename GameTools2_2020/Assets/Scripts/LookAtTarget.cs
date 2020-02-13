using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    public Transform lookTarget;
    public bool horizontalPlaneLock = false;
    // Start is called before the first frame update
    void Start()
    {
        if (lookTarget == null)
            lookTarget = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        //simplest approach:
        //transform.LookAt(lookTarget);

        //quaternion based approach:
        //(needed for the interpolation animation)
        Vector3 lookVec = lookTarget.position - transform.position;
        

        if (horizontalPlaneLock)
        {
            lookVec.y = 0.0f;
        }

        Quaternion lookQuat = Quaternion.LookRotation(lookVec);
        transform.rotation = Quaternion.Slerp(Quaternion.identity,lookQuat,GameController.interpolator);
        
    }
}
