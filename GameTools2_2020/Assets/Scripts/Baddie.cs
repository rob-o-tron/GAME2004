using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GRIDCITY
{
    public class Baddie : MonoBehaviour
    {
        private GridCityManager navCityManager;
        private NavMeshAgent navAgent;
        private Transform playerTransform;
        private Vector3 aimVec;
        private AudioSource audioSource;
        private float sightsTimer = 0.0f;
        public Transform eyeTransform;
        public LayerMask raycastMask;
        public float maxRange = 3f;
        // Start is called before the first frame update
        void Start()
        {
            GameController.Instance.RegisterBaddie();       
            navCityManager = GridCityManager.Instance;
            navAgent = GetComponent<NavMeshAgent>();
            audioSource= GetComponent<AudioSource>();
            playerTransform = GameController.Instance.playerBody;

            StartCoroutine(BaddieLogic());
        }

        private void Update()
        {
            sightsTimer += Time.deltaTime;
            //keep raycasting at player
            if (sightsTimer > 0.5f)
            {
                if (playerTransform != null)
                {
                    aimVec = playerTransform.position - eyeTransform.position;

                    RaycastHit hit;
                    // Does the ray intersect any objects excluding the player layer
                    if (Physics.Raycast(eyeTransform.position, aimVec, out hit, maxRange, raycastMask))
                    {
                        if ((hit.collider.tag == "Player"))
                        {
                            navAgent.destination = playerTransform.position;
                            audioSource.Play();
                            sightsTimer = 0.0f;
                        }
                        else
                        {
                            Transform patrolTransform = GameController.Instance.RequestPatrolTarget();
                            if (patrolTransform!=null)
                                navAgent.destination= patrolTransform.position;
                        }
                    }
                }
            }


        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                GameController.Instance.HurtPlayer();
            }
        }

        IEnumerator BaddieLogic()
        {
            while(true)
            {
                //randomize duration of patrol (5s - 30s)
                float patrolDuration = Random.Range(5f, 30f);
                //get random target from game controller
                Transform target = GameController.Instance.RequestPatrolTarget();
                if (target != null)
                    navAgent.destination = target.position;
                yield return new WaitForSeconds(patrolDuration);

            }
        }
    }

}
