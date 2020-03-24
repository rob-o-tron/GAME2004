using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private float vInput;
    private float hInput;
    private float eInput;

    private Rigidbody rb;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    public float speed = 5.0f;
    public float rotSpeed = 180.0f;
    public float rollResetSpeed = 0.5f;

    // Update is called once per frame
    void Update()
    {
        vInput = Input.GetAxis("Vertical") * speed;
        hInput = Input.GetAxis("Horizontal") * rotSpeed;
        eInput = Input.GetAxis("Elevate") * speed;

    }

    private void FixedUpdate()
    {
        if (rb!=null)
        {
            //forward-backward movement and elevation
            rb.MovePosition(transform.position + transform.forward * vInput * Time.fixedDeltaTime + transform.up * eInput * Time.fixedDeltaTime);

            //yaw (left/right heading control)
            Quaternion yawRot = Quaternion.AngleAxis(hInput * Time.fixedDeltaTime,Vector3.up);
            rb.MoveRotation(rb.rotation*yawRot);

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer==11)
        {
            Destroy(other.gameObject);
            audioSource.Play();
        }
    }

}