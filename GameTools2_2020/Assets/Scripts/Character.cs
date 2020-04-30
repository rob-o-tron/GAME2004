using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GRIDCITY
{
    public class Character : MonoBehaviour
    {

        public float charSpeed = 1.0f;
        public Animator charAnimator;
        public NavMeshAgent playerAgent;

        public AnimationCurve aniCurve;
        public Renderer myRend;
        private Material myMaterial;
        public Transform cameramanTransform;
        public Transform cameraTarget;

        public Transform agentTransform;
        public Transform bodyTransform;

        private float turnKeysTimer=0.5f;
        public float turnKeysInterval = 0.5f;
        private Quaternion targetRotation;

        private bool finished = false;


        // Start is called before the first frame update
        void Start()
        {
            myMaterial = myRend.material;
        }

        // Update is called once per frame
        void Update()
        {
            turnKeysTimer += Time.deltaTime;
            if (turnKeysTimer>turnKeysInterval)
            {
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    turnKeysTimer = 0f;
                    targetRotation = bodyTransform.localRotation * Quaternion.Euler(0.0f, 90.0f, 0.0f);
                };
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    turnKeysTimer = 0f;
                    targetRotation = bodyTransform.localRotation * Quaternion.Euler(0.0f, -90.0f, 0.0f);
                };
            }

            if(GameController.Instance.gameState == GameController.GameState.Game)
            {
                float horizontal = Input.GetAxis("Horizontal");
                float vertical = Input.GetAxis("Vertical");

                Vector3 moveVec = 1.25f * (horizontal * bodyTransform.right + vertical * bodyTransform.forward + 0.05f*bodyTransform.up);

                charAnimator.SetBool("forward", (vertical > 0.1f));
                charAnimator.SetBool("backward", (vertical < -0.1f));

                if ((vertical < 0.1f) && (vertical > -0.1f))
                {
                    charAnimator.SetBool("right", (horizontal > 0.1f));
                    charAnimator.SetBool("left", (horizontal < -0.1f));
                }
                else
                {
                    charAnimator.SetBool("right", false);
                    charAnimator.SetBool("left", false);
                }
                bodyTransform.position = Vector3.Lerp(bodyTransform.position, agentTransform.position, 0.1f);
                bodyTransform.rotation = Quaternion.Lerp(bodyTransform.rotation, targetRotation, 0.5f);
                playerAgent.destination = bodyTransform.position + moveVec;
            }

            else if ((GameController.Instance.gameState == GameController.GameState.GameOver)&&(finished==false))
            {
                charAnimator.SetBool("forward", false);
                charAnimator.SetBool("backward", false);
                charAnimator.SetBool("right", false);
                charAnimator.SetBool("left", false);
                if (Random.Range(0, 2) == 0)
                    charAnimator.SetBool("dying1", true);
                else
                    charAnimator.SetBool("dying2", true);
                finished = true;
            }
            else if ((GameController.Instance.gameState == GameController.GameState.Congrats)&&(finished==false))
            {
                charAnimator.SetBool("forward", false);
                charAnimator.SetBool("backward", false);
                charAnimator.SetBool("right", false);
                charAnimator.SetBool("left", false);
                if (Random.Range(0,2)==0)
                    charAnimator.SetBool("win1", true);
                else
                    charAnimator.SetBool("win2", true);
                finished = true;
            }



        }

        private IEnumerator HurtCoroutine()
        {
            float timer = Time.time;
            float starttime = Time.time;
            Color myColor = myMaterial.color;
            while (timer < (starttime + 3.0f))
            {
                timer += Time.deltaTime;
                float value = aniCurve.Evaluate((timer - starttime) / (3.0f));
                myColor.g = value;
                myColor.b = value;
                myMaterial.color = myColor;
                yield return null;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Enemy")
            {
                StartCoroutine(HurtCoroutine());
            }

        }
    }
}

