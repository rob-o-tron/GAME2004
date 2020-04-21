using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GRIDCITY
{
	public class NavigationCityManager : MonoBehaviour
    {

        #region Fields
        private static NavigationCityManager _instance;

        public NavMeshSurface navMesh;
        public Transform tilePrefab;
        public Transform slopeUpPrefab;
        public Transform slopeDownPrefab;
        public Transform agentPrefab;

        public BuildingProfile[] profileArray;
        public GameObject buildingPrefab;
        public GameObject roadPrefab;
        public Transform startLocation;
        public bool navMeshReady = false;


        private bool[,,] cityArray = new bool [40,40,40];   //increased array size to allow for larger city volume

        public static NavigationCityManager Instance
        {
            get
            {
                return _instance;
            }
        }
        #endregion

        #region Properties	
        #endregion

        #region Methods
        #region Unity Methods

        // Use this for internal initialization
        void Awake () {
            if (_instance == null)
            {
                _instance = this;
            }

            else
            {
                Destroy(gameObject);
                Debug.LogError("Multiple NavigationCityManager instances in Scene. Destroying clone!");
            };
        }
		
		// Use this for external initialization
		void Start ()
        {
            StartCoroutine(TimedBuildAndBake());
        }

        private void Update()
        {

        }

        IEnumerator TimedBuildAndBake()
        {
            for (int i = 0; i < 20; i++)
            {
                int random = Random.Range(0, profileArray.Length);
                int randomX = Random.Range(-15, 16);
                int randomZ = Random.Range(-15, 16);
                if (!(((randomX>-3)&&(randomX<3))&&((randomZ>-3)&&(randomZ<3))))
                    Instantiate(buildingPrefab, new Vector3(randomX, 0.05f, randomZ), Quaternion.identity).GetComponent<NavTowerBlock>().SetProfile(profileArray[random]);
            }

            yield return new WaitForSeconds(3);

            Instantiate(roadPrefab, startLocation.position, Quaternion.identity);

            yield return new WaitForSeconds(3);

            navMesh.BuildNavMesh();

            yield return new WaitForSeconds(1);

            navMeshReady = true;
        }

        #endregion

        public bool CheckSlot(int x, int y, int z)
        {
            if (x < 0 || x > 39 || y < 0 || y > 39 || z < 0 || z > 39) return true;
            else
            {
                return cityArray[x, y, z];
            }

        }

        public void SetSlot(int x, int y, int z, bool occupied)
        {
            if (!(x < 0 || x > 39 || y < 0 || y > 39 || z < 0 || z > 39))
            {
                cityArray[x, y, z] = occupied;
            }

        }

        #endregion

    }
}
