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
        public bool regularPatrol = true;
        public AudioClip alertFX;
        public AudioClip stopChaseFX;
        public Animator charAnimator;
        private Coroutine baddieLogic;
        // Start is called before the first frame update
        void Start()
        {
            GameController.Instance.RegisterBaddie();       
            navCityManager = GridCityManager.Instance;
            navAgent = GetComponent<NavMeshAgent>();
            audioSource= GetComponent<AudioSource>();
            playerTransform = GameController.Instance.playerBody;

            baddieLogic=StartCoroutine(BaddieLogic());
        }

        private void Update()
        {
            if (regularPatrol)
            {
                if (navAgent.remainingDistance < navAgent.stoppingDistance)
                    charAnimator.SetBool("running", false);
                else
                    charAnimator.SetBool("running", true);
            }
            if (baddieLogic==null)
            {
                baddieLogic = StartCoroutine(BaddieLogic());
            }

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
                        if (hit.collider.tag == "Player")
                        {
                            navAgent.destination = playerTransform.position;
                            audioSource.Stop();
                            audioSource.pitch = Random.Range(0.8f, 1.2f);
                            audioSource.clip = alertFX;
                            audioSource.Play();
                            sightsTimer = 0.0f;
                            regularPatrol = false;
                            charAnimator.SetBool("running", true);
                        }
                        else if (!regularPatrol)
                        {
                            audioSource.Stop();
                            audioSource.pitch = Random.Range(0.8f, 1.2f);
                            audioSource.clip = stopChaseFX;
                            audioSource.Play();
                            Transform patrolTransform = GameController.Instance.RequestPatrolTarget();
                            if (patrolTransform!=null)
                                navAgent.destination= patrolTransform.position;  
                            regularPatrol = true;
                            charAnimator.SetBool("running", true);
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
                regularPatrol = true;
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
