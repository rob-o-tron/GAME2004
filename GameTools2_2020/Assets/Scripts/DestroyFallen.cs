using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyFallen : MonoBehaviour
{
    public float thresholdY = - 10f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < thresholdY)
        {
            Destroy(gameObject);
        }
    }
}
