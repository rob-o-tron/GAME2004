using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    public bool alerted = false;

    public Transform gunXform;
    public Transform domeXform;
    public Transform aimTarget;
    public GameObject bulletPrefab;
    public float gunInterval = 0.1f;
    private float gunTimer = 0.0f;

    public LayerMask raycastMask;

    private float interpolator = 1.0f;

    private void Start()
    {
        if (aimTarget == null)
            aimTarget = GameObject.FindWithTag("Player").transform;
    }

    private void Update()
    {
        gunTimer -= Time.deltaTime;

        if (alerted)
        {
            interpolator = Mathf.Lerp(interpolator, 1.0f, 0.1f);

            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(gunXform.position, gunXform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, raycastMask))
            {
                if ((hit.collider.gameObject.tag=="Player")&&(gunTimer<0.0f))
                {
                    Instantiate(bulletPrefab, gunXform.position, Quaternion.identity);
                    gunTimer = gunInterval;
                }

            }
        }
        else
        {
            interpolator = Mathf.Lerp(interpolator, 0.0f, 0.1f);
        }

        //if (interpolator>0.9f)
        //{
        //    Instantiate(bulletPrefab, gunXform.position, Quaternion.identity);
        //}

        Vector3 aimVec = aimTarget.position - gunXform.position;
        Quaternion gunQuat = Quaternion.LookRotation(aimVec);
        gunXform.rotation = Quaternion.Slerp(Quaternion.identity, gunQuat, interpolator);
        aimVec.y = 0.0f;
        Quaternion domeQuat = Quaternion.LookRotation(aimVec);
        domeXform.rotation = Quaternion.Slerp(Quaternion.identity, domeQuat, interpolator);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag=="Player")
            alerted = true;
    }

    private void OnTriggerExit(Collider other)
    {
        alerted = false;
    }
}
