using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePhysicsController : MonoBehaviour
{

    private float vInput;
    private float hInput;
    private float eInput;

    private Rigidbody rb;

    public GameObject nuke;
    private bool nuked = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public float speed = 5.0f;
    public float rotSpeed = 180.0f;
    public float rollResetSpeed = 0.5f;

    // Update is called once per frame
    void Update()
    {
        vInput = Input.GetAxis("Vertical") * speed;
        hInput = Input.GetAxis("Horizontal") * rotSpeed;
        eInput = -Input.GetAxis("Elevate") * rotSpeed;

        if (transform.position.y>20)
        {
            if (!nuked)
            {
                Instantiate(nuke, Vector3.zero, Quaternion.identity);
                nuked = true;
            }

        }

    }

    private void FixedUpdate()
    {
        if (rb!=null)
        {
            //forward and backward movement
            rb.MovePosition(transform.position + transform.forward * vInput * Time.fixedDeltaTime);

            //yaw (left/right heading control)
            Quaternion yawRot = Quaternion.AngleAxis(hInput * Time.fixedDeltaTime,Vector3.up);
            rb.MoveRotation(rb.rotation*yawRot);

            //pitch (nose up/down control)
            Quaternion pitchRot = Quaternion.AngleAxis(eInput * Time.fixedDeltaTime, Vector3.right);
            rb.MoveRotation(rb.rotation * pitchRot);


            //the code below stabilises the vehicle roll after a collision or a side turn when pitching, but only when player lets go of controls
            if ((vInput==0f)&&(hInput == 0f)&&(eInput == 0f))
            {
                //first we look at the local "right" vector of the vehicle
                //if that vector is in the horizontal plane (parallel to the ground) then we don't need to do anything
                Vector3 currentRight=transform.right;
                //we only test the y component of the "right" vector:
                float y = currentRight.y;
                //Uncomment the line and watch the console to convince yourself that when y is 0 then we don't need to correct rotation
                //Debug.Log(y.ToString());

                //we create a Quaternion corresponding to a small rotation along the vehicle's "forward" axis:
                Quaternion rollRot = Quaternion.AngleAxis(- y * rollResetSpeed * Time.fixedDeltaTime, Vector3.forward);
                //we provide target rotation by multiplying the existing rigidbody rotation by the Quaternion:
                rb.MoveRotation(rb.rotation * rollRot);
            }

        }
    }

}