using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GRIDCITY
{
    public class RouteTile : MonoBehaviour
    {

        #region Fields

        public int recursionCount = 0;
        private int mySlope; //is current tile a slope up (1), a downslope (-1) or a flat segment
        private int myRotation;
        private int level;
        private Transform tilePrefab;
        private Transform slopeUpPrefab;
        private Transform slopeDownPrefab;

        private int maxLevel = 100;
        private GridCityManager cityManager;
        private GameController gameController;

        private int inc_x, inc_y, inc_z;
        //helper flags for edge cases
        private bool baddieSpawned = false;
        private bool targetSpawned = false;

        #endregion

        #region Properties	
        #endregion

        #region Methods

        public void Initialize(int recCount, int slope, int rotationDeg)
        {
            recursionCount = recCount;
            mySlope = slope;
            myRotation = rotationDeg;
        }

        public void SetIncrements (int degrees,int slope)
        {
            switch (degrees)
            {
                case 0:
                    inc_x = 0;
                    inc_z = -1;
                    break;
                case 90:
                    inc_x = -1;
                    inc_z = 0;
                    break;
                case 180:
                    inc_x = 0;
                    inc_z = 1;
                    break;
                case 270:
                    inc_x = 1;
                    inc_z = 0;
                    break;
                default:
                    break;
            }

            if (slope==-1 || slope==0 || slope==1)
            {
                inc_y = slope;
            }
        }

        #region Unity Methods

        // Use this for internal initialization
        void Awake()
        {
            inc_x = inc_y = inc_z = 0;
            cityManager = GridCityManager.Instance;
            gameController = GameController.Instance;
        }

        private void Update()
        {
            if ((cityManager.navMeshReady) && (mySlope == 0) && gameController.gameState!=GameController.GameState.Game)
            {
                int rando = Random.Range(0, 1000);
                if (rando < 5)
                {
                    Instantiate(cityManager.agentPrefab, this.transform);
                }
            }

            if (gameController.gameState==GameController.GameState.Game)
            {
                if ((cityManager.navMeshReady)&&(GameController.baddiesNeeded)&&(!baddieSpawned)&&(mySlope==0))
                {
                    if ((transform.position-cityManager.startLocation.position).magnitude>5f)
                    {
                        int rando = Random.Range(0, 1000);
                        if (rando < 5)
                        {
                            Instantiate(gameController.baddiePrefab, transform.position, Quaternion.identity, gameController.dummyPivot);
                            baddieSpawned = true;
                        }
                    }
                }

                if ((GameController.patrolTargetsNeeded) && (!targetSpawned) && (mySlope == 0))
                {
                    if ((transform.position - cityManager.startLocation.position).magnitude > 3f)
                    {
                        int rando = Random.Range(0, 1000);
                        if (rando < 5)
                        {
                            Transform target = Instantiate(gameController.targetPrefab, transform.position, Quaternion.identity, gameController.dummyPivot);
                            gameController.RegisterPatrolTarget(target);
                            targetSpawned = true;
                        }
                    }
                }
            }
        }

        // Use this for external initialization
        void Start()
        {       
            int x = Mathf.RoundToInt(transform.position.x + 20.0f);
            int y = Mathf.RoundToInt(transform.position.y);
            if (mySlope==-1)
                y = Mathf.RoundToInt(transform.position.y-1f);

            int z = Mathf.RoundToInt(transform.position.z + 20.0f);

            cityManager = GridCityManager.Instance;
            tilePrefab = cityManager.tilePrefab;
            slopeUpPrefab = cityManager.slopeUpPrefab;
            slopeDownPrefab = cityManager.slopeDownPrefab;

            Transform child;

            if (recursionCount == 0)         //initial tile "crossroads"
            {
                for (int nextRotation = 0; nextRotation < 360; nextRotation += 90)
                {
                    SetIncrements(nextRotation, 0);

                    Vector3 incVector = Vector3.right * inc_x + Vector3.up * inc_y + Vector3.forward * inc_z;
                    Quaternion nextQuat = Quaternion.Euler(0f, nextRotation, 0f);

                    if (!cityManager.CheckSlot(x+inc_x, y+inc_y, z+inc_z))
                    {
                        cityManager.SetSlot(x + inc_x, y + inc_y, z + inc_z,true);
                        child = Instantiate(tilePrefab, transform.position + incVector , nextQuat);
                        child.parent = this.transform;
                        child.GetComponent<RouteTile>().Initialize(recursionCount + 1,0, nextRotation);
                    }
                    else
                    {
                        GameObject.Destroy(gameObject);
                    }
                }

            }
            else if (recursionCount < maxLevel)
            {
                if (mySlope==0)
                {
                    Instantiate(gameController.pointPrefab, transform.position, Quaternion.identity, gameController.dummyPivot);
                }
                int corrector = (maxLevel-recursionCount) / 15;  //biasing randomization in the beginning to avoid extinction
                int random = Random.Range(0, 100);
                if (random<(80+corrector))                //move forward (most likely):
                {
                    int nextRotation = myRotation;
                    
                    random = Random.Range(0, 100);
                    if ((random>80)&&(mySlope == 0)) //going up
                    {
                        SetIncrements(nextRotation, mySlope);

                        Vector3 incVector = Vector3.right * inc_x + Vector3.up * inc_y + Vector3.forward * inc_z;
                        Quaternion nextQuat = Quaternion.Euler(0f, nextRotation, 0f);

                        if ((!cityManager.CheckSlot(x + inc_x, y, z + inc_z)) && (!cityManager.CheckSlot(x + inc_x, y + 1, z + inc_z)))
                            {
                            cityManager.SetSlot(x + inc_x, y, z + inc_z, true);
                            cityManager.SetSlot(x + inc_x, y + 1, z + inc_z, true);
                            child = Instantiate(slopeUpPrefab, transform.position + incVector, nextQuat);
                            child.parent = this.transform;
                            child.GetComponent<RouteTile>().Initialize(recursionCount + 1, 1, nextRotation);
                        }

                    }
                    else if ((random <20)&&(mySlope==0)) //going down
                    {
                        SetIncrements(nextRotation, mySlope);

                        Vector3 incVector = Vector3.right * inc_x + Vector3.up * inc_y + Vector3.forward * inc_z;
                        Quaternion nextQuat = Quaternion.Euler(0f, nextRotation, 0f);

                        if ((!cityManager.CheckSlot(x + inc_x, y, z + inc_z))&& (!cityManager.CheckSlot(x + inc_x, y-1, z + inc_z)))
                        {
                            cityManager.SetSlot(x + inc_x, y , z + inc_z, true);
                            cityManager.SetSlot(x + inc_x, y - 1, z + inc_z, true);
                            child = Instantiate(slopeDownPrefab, transform.position + incVector, nextQuat);
                            child.parent = this.transform;
                            child.GetComponent<RouteTile>().Initialize(recursionCount + 1, -1, nextRotation);
                        }

                    }
                    else //flat path
                    {
                        SetIncrements(nextRotation, mySlope);

                        Vector3 incVector = Vector3.right * inc_x + Vector3.up * inc_y + Vector3.forward * inc_z;
                        Quaternion nextQuat = Quaternion.Euler(0f, nextRotation, 0f);
                        if (mySlope==-1)
                        {
                            if (!cityManager.CheckSlot(x + inc_x, y + inc_y + 1, z + inc_z))
                            {
                                cityManager.SetSlot(x + inc_x, y + inc_y + 1, z + inc_z, true);
                                child = Instantiate(tilePrefab, transform.position + incVector, nextQuat);
                                child.parent = this.transform;
                                child.GetComponent<RouteTile>().Initialize(recursionCount + 1, 0, nextRotation);
                            }
                        }
                        else
                        {
                            if (!cityManager.CheckSlot(x + inc_x, y + inc_y, z + inc_z))
                            {
                                cityManager.SetSlot(x + inc_x, y + inc_y, z + inc_z, true);
                                child = Instantiate(tilePrefab, transform.position + incVector, nextQuat);
                                child.parent = this.transform;
                                child.GetComponent<RouteTile>().Initialize(recursionCount + 1, 0, nextRotation);
                            }
                        }


                    }
                }
                //end move forward
                //if tile is not sloped, we can make a left or right split every once in a while. let's keep it as simple as possible:
                random = Random.Range(0, 100);
                if (((random+corrector)>70)&&(mySlope==0))
                {
                    int nextRotation = myRotation;
                    random = Random.Range(0, 100);
                    if (random>50) //left turn
                    {
                        nextRotation = (int)Mathf.Repeat(nextRotation-90,360);
                    }
                    else  //right turn
                    {
                        nextRotation = (int)Mathf.Repeat(nextRotation+90, 360);
                    }
                    SetIncrements(nextRotation, 0);

                    Vector3 incVector = Vector3.right * inc_x + Vector3.up * inc_y + Vector3.forward * inc_z;
                    Quaternion nextQuat = Quaternion.Euler(0f, nextRotation, 0f);

                    if (!cityManager.CheckSlot(x + inc_x, y + inc_y, z + inc_z))
                    {
                        cityManager.SetSlot(x + inc_x, y + inc_y, z + inc_z, true);
                        child = Instantiate(tilePrefab, transform.position + incVector, nextQuat);
                        child.parent = this.transform;
                        child.GetComponent<RouteTile>().Initialize(recursionCount + 1, 0, nextRotation);
                    }

                }

            }
        }

        #endregion

        #endregion

    }
}

