using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GRIDCITY
{
    public class GridParametricBlock : MonoBehaviour
    {

        #region Fields
        public Transform basePrefab;
        private GridCityManager cityManager;
        public Transform startPoint, endPoint;

        private int bottomLeftNearIndX, bottomLeftNearIndZ, bottomLeftNearIndY;
        private int topRightFarIndX, topRightFarIndZ, topRightFarIndY;
        private Vector3 midPoint;
        private Vector3 scaleVec;
        #endregion

        #region Properties	
        #endregion

        #region Methods

        public void Initialize(Vector3 startVec, Vector3 endVec)
        {
            int startX = Mathf.RoundToInt(startVec.x + 20.01f);
            int startY = Mathf.RoundToInt(startVec.y);
            int startZ = Mathf.RoundToInt(startVec.z + 20.01f);

            int endX = Mathf.RoundToInt(endVec.x + 20.01f);
            int endY = Mathf.RoundToInt(endVec.y);
            int endZ = Mathf.RoundToInt(endVec.z + 20.01f);

            if (startX<endX)
            {
                bottomLeftNearIndX = startX;
                topRightFarIndX = endX;
            }
            else
            {
                bottomLeftNearIndX = endX;
                topRightFarIndX = startX;
            }

            if (startY < endY)
            {
                bottomLeftNearIndY = startY;
                topRightFarIndY = endY;
            }
            else
            {
                bottomLeftNearIndY = endY;
                topRightFarIndY = startY;
            }

            if (startZ < endZ)
            {
                bottomLeftNearIndZ = startZ;
                topRightFarIndZ = endZ;
            }
            else
            {
                bottomLeftNearIndZ = endZ;
                topRightFarIndZ = startZ;
            }

            for (int x=bottomLeftNearIndX; x<topRightFarIndX; x++)
                for (int y = bottomLeftNearIndY; y < topRightFarIndY; y++)
                    for (int z = bottomLeftNearIndZ; z < topRightFarIndZ; z++)
                    {
                        cityManager.SetSlot(x, y, z, true);
                    }
            int widthx = topRightFarIndX - bottomLeftNearIndX;
            int widthz = topRightFarIndZ - bottomLeftNearIndZ;
            int heighty = topRightFarIndY - bottomLeftNearIndY;
            for (int x = bottomLeftNearIndX+(widthx/5); x < topRightFarIndX-(widthx/5); x++)
                for (int y = bottomLeftNearIndY; y < (bottomLeftNearIndY+heighty*2/3); y++)
                    for (int z = bottomLeftNearIndZ; z < topRightFarIndZ; z++)
                    {
                        cityManager.SetSlot(x, y, z, false);
                    }
            for (int x = bottomLeftNearIndX; x < topRightFarIndX; x++)
                for (int y = bottomLeftNearIndY; y < (bottomLeftNearIndY+heighty * 2 / 3) ; y++)
                    for (int z = bottomLeftNearIndZ + (widthz/5); z < topRightFarIndZ-(widthz/5); z++)
                    {
                        cityManager.SetSlot(x, y, z, false);
                    }

            midPoint = (startVec + endVec) / 2.0f;
            scaleVec = (endVec - midPoint)*2.0f;
            midPoint.y = Mathf.Min(startVec.y, endVec.y);
            Transform newObject = Instantiate(basePrefab, midPoint, Quaternion.identity, cityManager.transform);
            newObject.localScale = scaleVec;


        }

        #region Unity Methods

        // Use this for internal initialization
        void Awake()
        {

        }

        // Use this for external initialization
        void Start()
        {
            cityManager = GridCityManager.Instance;
            Initialize(startPoint.position, endPoint.position);
        }


        #endregion
        #endregion

    }
}

