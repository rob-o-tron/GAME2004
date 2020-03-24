using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GRIDCITY
{
	public class HostileCityManager : MonoBehaviour
    {

        #region Fields
        private static HostileCityManager _instance;
        public Mesh[] meshArray;
        public GameObject buildingPrefab;
        public BuildingProfile[] profileArray;


        private bool[,,] cityArray = new bool [15,15,15];   //increased array size to allow for larger city volume

        public static HostileCityManager Instance
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
                Debug.LogError("Multiple HostileCityManager instances in Scene. Destroying clone!");
            };
        }
		
		// Use this for external initialization
		void Start ()
        {
            //UPDATING PLANNING ARRAY TO ACCOUNT FOR TURRET
            for (int i = -3; i < 4; i += 1)
            {
                for (int j = -3; j < 4; j += 1)
                {
                    SetSlot(i+7,0,j+7,true); 
                }
            }


            //CITY BUILDINGS
            for (int i=-7;i<8;i+=3)
            {
                for (int j=-7; j<8 ;j+=3)
                {
                    int random = Random.Range(0, profileArray.Length);
                    Instantiate(buildingPrefab, new Vector3(i, 0.05f, j), Quaternion.identity).GetComponent<HostileTowerBlock>().SetProfile(profileArray[random]);                 
                }
            }
                      
		}
		
		#endregion

        public bool CheckSlot(int x, int y, int z)
        {
            if (x < 0 || x > 14 || y < 0 || y > 14 || z < 0 || z > 14) return true;
            else
            {
                return cityArray[x, y, z];
            }
        }

        public void SetSlot(int x, int y, int z, bool occupied)
        {
            if (!(x < 0 || x > 14 || y < 0 || y > 14 || z < 0 || z > 14))
            {
                cityArray[x, y, z] = occupied;
            }
        }

        #endregion

    }
}
