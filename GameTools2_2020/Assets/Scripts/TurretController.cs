using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    public bool alerted;
    private float interpolator = 1.0f;
    public Transform lookTarget;

    public Transform gunXform;
    public Transform domeXform;

    public float turretSpeedOn = 0.2f;
    public float turretSpeedOff = 0.025f;

    private Vector3 aimVec;

    public GameObject bulletPrefab;

    public LayerMask raycastMask;

    public float gunInterval = 1.0f;
    private float gunTimer = 0.0f;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        if (lookTarget == null)
            lookTarget = GameObject.FindWithTag("Player").transform;

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        gunTimer -= Time.deltaTime;

        aimVec = lookTarget.position - gunXform.position;

        if (alerted)
        {
            interpolator = Mathf.Lerp(interpolator, 1.0f, turretSpeedOn);

            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(gunXform.position, gunXform.forward, out hit, Mathf.Infinity, raycastMask))
            {
                if ((hit.collider.tag=="Player")&&(gunTimer<0.0f))
                {
                    Instantiate(bulletPrefab, gunXform.position, Quaternion.identity);
                    gunTimer = gunInterval;
                    audioSource.Play();

                }

            }

        }
        else
        {
            interpolator = Mathf.Lerp(interpolator, 0.0f, turretSpeedOff);
        }




        Quaternion gunQuat = Quaternion.LookRotation(aimVec);
        gunXform.rotation = Quaternion.Slerp(Quaternion.identity, gunQuat, interpolator);



    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag=="Player")
            alerted = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag=="Player")
            alerted = false;
    }


}
