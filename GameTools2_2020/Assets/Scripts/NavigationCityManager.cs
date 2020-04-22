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

        }

        private void Update()
        {

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
