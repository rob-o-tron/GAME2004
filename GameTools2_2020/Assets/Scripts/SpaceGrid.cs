using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SpaceGrid : MonoBehaviour
{
    #region Fields
    private bool _renderActive = true;
    private Mesh _lineMesh;
    private Mesh _pointMesh;
 
    private Vector3[,,] _points;
    private Vector2[,,] _uvs;
    private int[,,] _indices;
    public int _divisions;

    public GameObject _objLines;
    public GameObject _objPoints;
    private Renderer _rendLines,_rendPoints;
    private MeshFilter _filtLines, _filtPoints;
    public Material _matLines,_matPoints;
    #endregion

    #region Properties
    public bool RenderActive
    {
        get
        {
            return _renderActive;
        }
        set
        {
            _renderActive = value;
            if (_objLines != null)
                _objLines.SetActive(_renderActive);
            if (_objPoints != null)
                _objPoints.SetActive(_renderActive);
        }
    }
    #endregion

    // Use this for initialization
    void Awake()
    {
        _rendLines = _objLines.GetComponent<Renderer>();
        _rendPoints = _objPoints.GetComponent<Renderer>();
        _filtLines = _objLines.GetComponent<MeshFilter>();
        _filtPoints = _objPoints.GetComponent<MeshFilter>();
        _matLines = _rendLines.material;
        _matPoints = _rendPoints.material;
        CreateMesh(_filtLines,_filtPoints,_divisions);
    }

    public void CreateMesh(MeshFilter filtLines, MeshFilter filtPoints, int divisions)
    {
        int countLinePoints= (divisions+1)*(divisions+1)*6;
        int countGridPoints = (divisions+1) * (divisions + 1) * (divisions + 1);

        Mesh meshLines = new Mesh();
        Mesh meshPoints = new Mesh();
        Vector2[] uvLines = new Vector2[countLinePoints];
        Vector2[] uvPoints = new Vector2[countGridPoints];
        Vector3[] vertLines = new Vector3[countLinePoints];
        Vector3[] vertPoints = new Vector3[countGridPoints];
        _points = new Vector3[divisions+1,divisions+1,divisions+1];
        _uvs= new Vector2[divisions+1,divisions+1, divisions+1];
        float sqrt3 = Mathf.Pow(3.0f, 1.0f / 3.0f);
        float step = 2.0f / (float)divisions;
        int[] pointIndices = new int[countGridPoints];
        _indices = new int[divisions + 1, divisions + 1, divisions + 1];
        int count = 0;
        for (int ix=0;ix<divisions+1;ix++)
        {
            for (int iy=0;iy<divisions+1;iy++)
            {
                for (int iz=0;iz<divisions+1;iz++)
                {
                    float px = -1.0f + ix * step;
                    float py = -1.0f + iy * step;
                    float pz = -1.0f + iz * step;
                    float u = (new Vector3(px, py, pz)).magnitude / sqrt3;
                    float v = Mathf.Max(Mathf.Abs(px+0.5f), Mathf.Abs(py+0.5f), Mathf.Abs(pz + 0.5f));
                    _points[ix, iy, iz] = new Vector3(px,py,pz);
                    _uvs[ix, iy, iz] = new Vector2(u,v);
                    _indices[ix, iy, iz] = count;
                    pointIndices[count]=count;
                    vertPoints[count]=_points[ix, iy, iz];
                    uvPoints[count]=_uvs[ix, iy, iz];
                    count++;
                }
            }
        }
        meshPoints.vertices = vertPoints;
        meshPoints.uv = uvPoints;
        meshPoints.SetIndices(pointIndices, MeshTopology.Points, 0);

        count = 0;
        int[] lineIndices = new int[countLinePoints];
        for (int ix = 0; ix < divisions + 1; ix++)
        {
            for (int iy = 0; iy < divisions + 1; iy++)
            {
                int iz = 0;
                vertLines[count]=_points[ix, iy, iz];
                uvLines[count]=_uvs[ix, iy, iz];
                lineIndices[count] = count;
                count++;
                iz = divisions;
                vertLines[count] = _points[ix, iy, iz];
                uvLines[count] = _uvs[ix, iy, iz];
                lineIndices[count] = count;
                count++;

            }
        }
        for (int ix = 0; ix < divisions + 1; ix++)
        {
            for (int iz = 0; iz < divisions + 1; iz++)
            {
                int iy = 0;
                vertLines[count] = _points[ix, iy, iz];
                uvLines[count] = _uvs[ix, iy, iz];
                lineIndices[count] = count;
                count++;
                iy = divisions;
                vertLines[count] = _points[ix, iy, iz];
                uvLines[count] = _uvs[ix, iy, iz];
                lineIndices[count] = count;
                count++;
            }
        }

        for (int iy = 0; iy < divisions + 1; iy++)
        {
            for (int iz = 0; iz < divisions + 1; iz++)
            {
                int ix = 0;
                vertLines[count] = _points[ix, iy, iz];
                uvLines[count] = _uvs[ix, iy, iz];
                lineIndices[count] = count;
                count++;
                ix = divisions;
                vertLines[count] = _points[ix, iy, iz];
                uvLines[count] = _uvs[ix, iy, iz];
                lineIndices[count] = count;
                count++;
            }
        }

        meshLines.vertices = vertLines;
        meshLines.uv = uvLines;
        meshLines.SetIndices(lineIndices, MeshTopology.Lines, 0);

        float boundwidth = 500.0f;
        meshLines.bounds = new UnityEngine.Bounds(new Vector3(0, 0, 0), new Vector3(1, 1, 1) * boundwidth);
        meshPoints.bounds = new UnityEngine.Bounds(new Vector3(0, 0, 0), new Vector3(1, 1, 1) * boundwidth);
        filtLines.mesh = meshLines;
        filtPoints.mesh = meshPoints;
    }

    public void Enable(int ind, bool show)
    {
        _rendLines.enabled = show;
        _rendPoints.enabled = show;
    }

}

