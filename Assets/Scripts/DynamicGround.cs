using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class DynamicGround : MonoBehaviour
{
    [SerializeField, Tooltip("In Unity units")]
    private int xSize, zSize;

    [FormerlySerializedAs("pointsPerUnit")] [SerializeField, Tooltip("Density")] private int _pointsPerUnit;

    private int _xPoints, _zPoints;
    private Mesh _mesh;
    private bool[,,] _skippedQuads; // x,y,z => is quad skipped 

    private Vector3[] _vertices;

    private void Start()
    {
        _xPoints = xSize * _pointsPerUnit;
        _zPoints = zSize * _pointsPerUnit;
        _skippedQuads = new bool[_xPoints, 1, _zPoints];
        RegenerateMesh();
    }

    public void UpdateGround(List<Vector3> trailPoints = null, int startIndex = 0)
    {
        // Skip cubes intersected by cutting line
        for (var i = startIndex; i < trailPoints.Count; i++)
        {
            var cubeX = (int) Mathf.Floor(trailPoints[i].x * _pointsPerUnit);
            var cubeZ = (int) Mathf.Floor(trailPoints[i].z * _pointsPerUnit);
            if (!_skippedQuads[cubeX, 0, cubeZ])
            {
                _skippedQuads[cubeX, 0, cubeZ] = true;
                Debug.Log($"cut out {cubeX} 0 {cubeZ}");
                Debug.DrawLine(
                    (new Vector3(cubeX, 0, cubeZ) + Vector3.forward * .5f + Vector3.right * .5f) / _pointsPerUnit,
                    (new Vector3(cubeX, 1, cubeZ) + Vector3.forward * .5f + Vector3.right * .5f) / _pointsPerUnit,
                    Color.green,999);
            }
        }
        
        // ielikt tikko izgrieztot klucīšus citā DS un tad pēc tās noskaidrot kurus vēl jānodropo, lai nelido gaisā

        // var xx = (int) Mathf.Floor(trailPoints[trailPoints.Count - 1].x * pointPerUnit);
        // var zz = (int) Mathf.Floor(trailPoints[trailPoints.Count - 1].z * pointPerUnit);
        // Debug.DrawLine(
        //     (new Vector3(xx, 0, zz) + Vector3.forward * .5f + Vector3.right * .5f) / pointPerUnit,
        //     (new Vector3(xx, 3, zz) + Vector3.forward * .5f + Vector3.right * .5f) / pointPerUnit,
        //     Color.blue);

        // Find floating islands and drop them
        int maxZ = _skippedQuads.GetLength(2);
        int maxX = _skippedQuads.GetLength(0);
        for (int z = 1; z < maxZ - 1; z++)
        {
            for (int x = 1; x < maxX - 1; x++)
            {
                if ( MyMath.ContainsPoint(trailPoints, startIndex, (new Vector3(x, 0, z) + Vector3.forward * .5f + Vector3.right * .5f) / _pointsPerUnit))  
                {
                        _skippedQuads[x, 0, z] = true;
                        Debug.DrawLine(
                            (new Vector3(x, 0, z) + Vector3.forward * .5f + Vector3.right * .5f) / _pointsPerUnit,
                            (new Vector3(x, 2, z) + Vector3.forward * .5f + Vector3.right * .5f) / _pointsPerUnit,
                            Color.yellow, 999);
                }
            }
        }

        RegenerateMesh();
    }

    // find next hole to the right
    private bool IsThisAnIslandX(int xStart, int z)
    {
        int maxX = _skippedQuads.GetLength(0);
        for (int x = xStart+1; x < maxX - 1; x++)
        {
            if (_skippedQuads[x, 0, z])
            {
                return true;
            }
        }

        return false;
    }
    
    private bool IsThisAnIslandZ(int x, int zStart)
    {
        int maxZ = _skippedQuads.GetLength(2);
        for (int z = zStart+1; z < maxZ - 1; z++)
        {
            if (_skippedQuads[x, 0, z])
            {
                return true;
            }
        }

        return false;
    }

    private void RegenerateMesh()
    {
        GetComponent<MeshFilter>().mesh = _mesh = new Mesh();
        _mesh.name = "Procedural Grid";
        _vertices = new Vector3[(_xPoints + 1) * (_zPoints + 1)];
        for (int i = 0, z = 0; z <= _zPoints; z++)
        {
            for (int x = 0; x <= _xPoints; x++, i++)
            {
                _vertices[i] = new Vector3(x / (float) _pointsPerUnit, 0, z / (float) _pointsPerUnit);
            }
        }

        _mesh.vertices = _vertices;

        int[] triangles = new int[_xPoints * _zPoints * 6];
        for (int ti = 0, vi = 0, z = 0; z < _zPoints; z++, vi++)
        {
            for (int x = 0; x < _xPoints; x++, ti += 6, vi++)
            {
                if (_skippedQuads[x, 0, z]) continue;

                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + _xPoints + 1;
                triangles[ti + 5] = vi + _xPoints + 2;
            }
        }

        _mesh.triangles = triangles;
        _mesh.RecalculateNormals();
    }
}