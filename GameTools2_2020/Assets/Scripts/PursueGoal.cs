using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GRIDCITY
{
    public class PursueGoal : MonoBehaviour
    {
        private GridCityManager navCityManager;
        private NavMeshAgent navAgent;

        // Start is called before the first frame update
        void Start()
        {
            GetComponent<Renderer>().material.color = Random.ColorHSV();         
            navCityManager = GridCityManager.Instance;
            navAgent = GetComponent<NavMeshAgent>();
            Destroy(this.gameObject, 10f);
            navAgent.destination = navCityManager.startLocation.position;
        }

        // Update is called once per frame
        void Update()
        {
        }
    }

}
