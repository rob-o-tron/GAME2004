using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10.0f;
    private Vector3 aimVec;
    private Transform aimTarget;


    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, 5.0f);
        aimTarget = GameObject.FindWithTag("Player").transform;
        aimVec = aimTarget.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += aimVec * speed * Time.deltaTime;

    }

    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //Debug.Log("Hit");
            Destroy(this.gameObject);
        }
    }
    */

}
