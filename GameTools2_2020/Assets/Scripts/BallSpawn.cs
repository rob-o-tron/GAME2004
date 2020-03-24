using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawn : MonoBehaviour
{
    public Transform ballPrefab;
    private Rigidbody rb;
    private Vector3 posVector;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            posVector = new Vector3(Random.Range(-5f, 5f), Random.Range(1f, 10f), Random.Range(-5f, 5f));
            Transform spawnedObjectTransform = Instantiate(ballPrefab,posVector,Quaternion.identity);
            rb = spawnedObjectTransform.GetComponent<Rigidbody>();
        }
    }

    private void FixedUpdate()
    {
        if (rb!=null)
        {
            rb.AddForce(posVector);
        }
    }
}
