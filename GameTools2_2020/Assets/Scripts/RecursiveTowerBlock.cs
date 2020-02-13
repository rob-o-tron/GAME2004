using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GRIDCITY
{
	public class RecursiveTowerBlock : MonoBehaviour {

        #region Fields
        public Transform basePrefab;
        public int recursionLevel = 0;
        public int maxLevel = 0;
        public blockType myType = blockType.Arches;
        public int myMaterial = 0;
        private CityManager cityManager;
        private Renderer myRenderer;
        private MeshFilter myMeshFilter;
        #endregion

        #region Properties	
        #endregion

        #region Methods

        public void Initialize(int recLevel, blockType type, int blockMaterial)
        {
            recursionLevel = recLevel;
            myType = type;
            myMaterial = blockMaterial;
        }

        #region Unity Methods

        // Use this for internal initialization
        void Awake ()
        {
			myRenderer = GetComponent<MeshRenderer>();
            myMeshFilter = GetComponent<MeshFilter>();
		}
		
		// Use this for external initialization
		void Start ()
        {
            cityManager = CityManager.Instance;

            myMeshFilter.mesh = cityManager.meshArray[(int)myType];
            myRenderer.material = cityManager.materialArray[myMaterial];
            if (recursionLevel < maxLevel)
            {
                Transform child = Instantiate(basePrefab,transform.position+Vector3.up,Quaternion.identity,this.transform);
                child.GetComponent<RecursiveTowerBlock>().Initialize(recursionLevel+1, (blockType)Random.Range(0, 9), Random.Range(0, 6));
            }
        }
		
		// Update is called once per frame
		void Update ()
        {
			
		}

		#endregion
	#endregion
		
	}
}
