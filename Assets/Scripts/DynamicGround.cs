using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class DynamicGround : MonoBehaviour
{
    [SerializeField, Tooltip("In Unity units")]
    private int xSize, zSize;

    [FormerlySerializedAs("pointsPerUnit")] [SerializeField, Tooltip("Density")]
    private int _pointsPerUnit;

    private int _xPoints, _zPoints;
    private Mesh _mesh;
    private bool[,,] _skippedQuads; // x,y,z => is quad skipped  (coords are for quad's lower bottom corner)

    private Vector3[] _vertices;
    private bool[] _verticesMoved; // To make sure only vertex is moved only once

    private void Start()
    {
        _xPoints = xSize * _pointsPerUnit;
        _zPoints = zSize * _pointsPerUnit;
        _skippedQuads = new bool[_xPoints, 1, _zPoints];
        RegenerateMesh();
    }

    public void UpdateGround(List<Vector3> trailPoints = null, int trailStartIndex = 0)
    {
        // Debug.Log($"update ground num={trailPoints.Count} trailStartIndex={trailStartIndex}");
        // Skip cubes intersected by cutting line
        for (var i = trailStartIndex; i < trailPoints.Count; i++)
        {
            var cubeX = (int) Mathf.Floor(trailPoints[i].x * _pointsPerUnit);
            var cubeZ = (int) Mathf.Floor(trailPoints[i].z * _pointsPerUnit);
            if (!_skippedQuads[cubeX, 0, cubeZ])
            {
                _skippedQuads[cubeX, 0, cubeZ] = true;
                // Debug.DrawLine(
                //     (new Vector3(cubeX, 0, cubeZ) + Vector3.forward * .5f + Vector3.right * .5f) / _pointsPerUnit,
                //     (new Vector3(cubeX, 1, cubeZ) + Vector3.forward * .5f + Vector3.right * .5f) / _pointsPerUnit,
                //     Color.green,999);
            }
        }

        // Find floating islands and drop them
        for (int z = 1; z < _zPoints - 1; z++)
        {
            for (int x = 1; x < _xPoints - 1; x++)
            {
                if (MyMath.ContainsPoint(trailPoints, trailStartIndex, (new Vector3(x, 0, z) + Vector3.forward * .5f + Vector3.right * .5f) / _pointsPerUnit))
                {
                    _skippedQuads[x, 0, z] = true;
                    // Debug.DrawLine(
                    //     (new Vector3(x, 0, z) + Vector3.forward * .5f + Vector3.right * .5f) / _pointsPerUnit,
                    //     (new Vector3(x, 2, z) + Vector3.forward * .5f + Vector3.right * .5f) / _pointsPerUnit,
                    //     Color.yellow, 999);
                }
            }
        }

        RegenerateMesh(trailPoints, trailStartIndex);
    }

    private void RegenerateMesh(List<Vector3> trailPoints = null, int trailStartIndex = 0)
    {
        if (_mesh == null)
        {
            GetComponent<MeshFilter>().mesh = _mesh = new Mesh();
            _mesh.name = "Procedural Grid";
            _vertices = new Vector3[(_xPoints + 1) * (_zPoints + 1)];
            _verticesMoved = new bool[(_xPoints + 1) * (_zPoints + 1)];
            for (int i = 0, z = 0; z <= _zPoints; z++)
            {
                for (int x = 0; x <= _xPoints; x++, i++)
                {
                    _vertices[i] = new Vector3(x / (float) _pointsPerUnit, 0, z / (float) _pointsPerUnit);
                }
            }
        }

        // Mess up generated grid vertices
        for (int i = 0, z = 0; z <= _zPoints; z++)
        {
            for (int x = 0; x <= _xPoints; x++, i++)
            {
                if ((x > 0 && z > 0 && x < _xPoints && z < _zPoints))
                {
                    if (_skippedQuads[x, 0, z] &&
                        (!_skippedQuads[x, 0, z + 1] || !_skippedQuads[x, 0, z - 1] ||
                         !_skippedQuads[x + 1, 0, z] || !_skippedQuads[x - 1, 0, z]
                        )
                    )
                    {
                        DistortPoint(x, z, trailPoints, trailStartIndex);
                        DistortPoint(x + 1, z, trailPoints, trailStartIndex);
                        DistortPoint(x, z + 1, trailPoints, trailStartIndex);
                        DistortPoint(x + 1, z + 1, trailPoints, trailStartIndex);
                    }
                }
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

    // Move mesh point closer to the closest point OF the trail (good enaough not to calculate closest point ON the trail) 
    private void DistortPoint(int x, int z, List<Vector3> trailPoints = null, int trailStartIndex = 0)
    {
        var vertexIndex = z * (_xPoints + 1) + x;
        if (_verticesMoved[vertexIndex]) // Leave already moved ones alone!
        {
            Debug.DrawLine(
                new Vector3(x, 0, z) / _pointsPerUnit,
                new Vector3(x, 2, z) / _pointsPerUnit,
                Color.black, 999);
            return;
        }

        Debug.DrawLine(
            new Vector3(x, 0, z) / _pointsPerUnit,
            new Vector3(x, 2, z) / _pointsPerUnit,
            Color.green, 999);

        var vertexPoint = _vertices[vertexIndex];
        var closestDistance = float.MaxValue;
        var closestTrailPointIndex = -1;
        for (var i = trailStartIndex + 1; i < trailPoints.Count - 1; i++)
        {
            var d = Vector3.Distance(vertexPoint, trailPoints[i]);
            if (d < closestDistance && d < (1.0 / _pointsPerUnit) * 1.6f) // Consider moving only nearby points
            {
                closestDistance = d;
                closestTrailPointIndex = i;
            }
        }

        if (closestTrailPointIndex > 0)
        {
            _vertices[vertexIndex] = trailPoints[closestTrailPointIndex];
            _verticesMoved[vertexIndex] = true;
            // Debug.Log($"d={closestDistance}");
            Debug.DrawLine(
                trailPoints[closestTrailPointIndex],
                trailPoints[closestTrailPointIndex] + Vector3.up * 2,
                Color.yellow, 999);
        }
    }
}