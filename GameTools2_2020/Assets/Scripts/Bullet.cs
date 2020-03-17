using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Transform aimTarget;
    private Vector3 direction;

    public  float speed = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        if (aimTarget == null)
            aimTarget = GameObject.FindWithTag("Player").transform;

        Vector3 targetPosition = aimTarget.position;
        Vector3 initialPosition = transform.position;
        direction = targetPosition - initialPosition;

        Destroy(this.gameObject, 7.0f);
    }

    private void Update()
    {
        transform.position += Time.deltaTime * direction.normalized * speed;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag=="Player")
        {
            Debug.Log("Hit!");
        }
        Destroy(this.gameObject);
    }
}
