using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GRIDCITY
{
    public class Point : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            //once point is spawned, increment max loot counter 
            GameController.maxLoot++;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                GameController.Instance.CollectPoint();
                Destroy(this.gameObject);
            }
        }
    }

}
