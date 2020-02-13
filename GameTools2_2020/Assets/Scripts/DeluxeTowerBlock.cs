using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GRIDCITY
{
    public class DeluxeTowerBlock : MonoBehaviour
    {

        #region Fields
        public BuildingProfile myProfile;
        public Transform basePrefab;
        public int recursionLevel = 0;
        private int maxLevel = 3;
        private CityManager cityManager;
        private Renderer myRenderer;
        private MeshFilter myMeshFilter;
        private Mesh myMesh;
        private Material myMaterial;
        #endregion

        #region Properties	
        #endregion

        #region Methods

        public void SetProfile(BuildingProfile profile)
        {
            myProfile = profile;
        }

        public void Initialize(int recLevel, Material mat, Mesh mesh)
        {
            recursionLevel = recLevel;
            myMesh = mesh;
            myMaterial = mat;
            maxLevel = myProfile.maxHeight;
            
        }

        #region Unity Methods

        // Use this for internal initialization
        void Awake()
        {
            myRenderer = GetComponent<MeshRenderer>();
            myMeshFilter = GetComponent<MeshFilter>();
        }

        // Use this for external initialization
        void Start()
        {
            cityManager = CityManager.Instance;
            Transform child;
            if (recursionLevel==0)
            {
                int meshNum = myProfile.groundBlocks.Length;
                int matNum = myProfile.groundMaterials.Length;
                myMesh=myProfile.groundBlocks[Random.Range(0, meshNum)];
                myMaterial = myProfile.groundMaterials[Random.Range(0, matNum)];
            }

            myMeshFilter.mesh = myMesh;
            myRenderer.material = myMaterial;

            if (recursionLevel < maxLevel)
            {
                if (recursionLevel == maxLevel-1)
                {
                    child = Instantiate(basePrefab, transform.position + Vector3.up, Quaternion.identity, this.transform);
                    int meshNum = myProfile.roofBlocks.Length;
                    int matNum = myProfile.roofMaterials.Length;
                    child.GetComponent<DeluxeTowerBlock>().Initialize(recursionLevel + 1, myProfile.roofMaterials[Random.Range(0, matNum)], myProfile.roofBlocks[Random.Range(0, meshNum)]);
                }
                else
                {
                    child = Instantiate(basePrefab, transform.position + Vector3.up, Quaternion.identity, this.transform);
                    int meshNum = myProfile.mainBlocks.Length;
                    int matNum = myProfile.mainMaterials.Length;
                    child.GetComponent<DeluxeTowerBlock>().Initialize(recursionLevel + 1, myProfile.mainMaterials[Random.Range(0, matNum)], myProfile.mainBlocks[Random.Range(0, meshNum)]);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        #endregion
        #endregion

    }
}

