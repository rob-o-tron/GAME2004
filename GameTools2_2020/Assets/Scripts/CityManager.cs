using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GRIDCITY
{
    public enum blockType { Block, Arches, Columns, Dishpivot, DomeWithBase, HalfDome, SlitDome, Slope, Tile};

	public class CityManager : MonoBehaviour
    {

        #region Fields
        private static CityManager _instance;
        public Mesh[] meshArray;
        public Material[] materialArray;
        public GameObject buildingPrefab;
        public GameObject treePrefab;
        public BuildingProfile[] profileArray;

        public BuildingProfile wallProfile;

        private bool[,,] cityArray = new bool [15,15,15];   //increased array size to allow for larger city volume

        public static CityManager Instance
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
                Debug.LogError("Multiple CityManager instances in Scene. Destroying clone!");
            };
        }
		
		// Use this for external initialization
		void Start ()
        {
            //UPDATING PLANNING ARRAY TO ACCOUNT FOR MANUALLY PLACED|CITY GATE
            for (int ix=-1; ix <2; ix++)
            {
                int iz = -7;
                for (int iy=0;iy<3;iy++)
                {
                    SetSlot(ix + 7, iy, iz + 7, true);
                }
            }

            //BUILD CITY WALLS - add your code below
            
            for (int i=-7 ; i < 8 ; i += 14)
            {
                for (int j = -7; j < 8; j += 1)
                {
                    Instantiate(buildingPrefab, new Vector3(i, 0.05f, j), Quaternion.identity).GetComponent<DeluxeTowerBlock>().SetProfile(wallProfile);
                }
                for (int j = -6; j < 7; j += 1)
                {
                    Instantiate(buildingPrefab, new Vector3(j, 0.05f, i), Quaternion.identity).GetComponent<DeluxeTowerBlock>().SetProfile(wallProfile);
                }
            }
            

            //CITY BUILDINGS:
            
			for (int i=-4;i<5;i+=2)
            {
                for (int j=-4;j<5;j+=2)
                {
                    int random = Random.Range(0, profileArray.Length);
                    Instantiate(buildingPrefab, new Vector3(i, 0.05f, j), Quaternion.identity).GetComponent<DeluxeTowerBlock>().SetProfile(profileArray[random]);                 
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
