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
        transform.LookAt(lookTarget);
    }
}
