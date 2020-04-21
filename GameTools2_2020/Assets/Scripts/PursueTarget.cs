// MoveTo.cs
using UnityEngine;
using UnityEngine.AI;

namespace GRIDCITY
{
    public class PursueTarget : MonoBehaviour
    {
        private NavMeshAgent agent;
        public Transform goal;
        private NavigationCityManager navCityManager;

        public void SetColor(Color col)
        {
            Renderer rend = GetComponent<Renderer>();
            Material mat = rend.material;
            mat.color = col;           
        }

        void Start()
        {
            SetColor(Random.ColorHSV());
            navCityManager = NavigationCityManager.Instance;
            agent = GetComponent<NavMeshAgent>();
            agent.destination = navCityManager.startLocation.position;
            Destroy(this.gameObject, 10.0f);
        }
    }
}

