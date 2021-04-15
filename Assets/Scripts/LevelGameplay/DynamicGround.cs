using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using deVoid.Utils;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class DynamicGround : MonoBehaviour
{
    [SerializeField, Tooltip("In Unity units")]
    private int xSize, zSize;

    [SerializeField, Tooltip("Density")] private int _pointsPerUnit;
    [SerializeField] private Transform _dynamicHolePrefab;
    [SerializeField] private Transform _dynamicHoleWallPrefab;
    [SerializeField] private Transform _wallBottom;
    [SerializeField] private Transform _wallTop;
    [SerializeField] private Transform _wallLeft;
    [SerializeField] private Transform _wallRight;
    [SerializeField] private Transform _finish;

    private int _xPoints, _zPoints;
    private Mesh _mesh;
    private bool[,,] _skippedQuads; // x,y,z => is quad skipped  (coords are for quad's lower bottom corner)
    private Vector3[] _vertices;
    private Vector2[] _uv;
    private bool[] _verticesMoved; // To make sure the vertex is moved only once
    private int _holeId;

    private void Start()
    {

        zSize = GameDataManager.I.LevelNumber * 3 + 13;
                
        Debug.Log($"DynamicGround zSize={zSize} L={GameDataManager.I.LevelNumber}");
        
        _xPoints = xSize * _pointsPerUnit;
        _zPoints = zSize * _pointsPerUnit;
        _skippedQuads = new bool[_xPoints, 1, _zPoints];
        _dynamicHolePrefab.gameObject.SetActive(false);
        _dynamicHoleWallPrefab.gameObject.SetActive(false);
        RegenerateMesh();
        Signals.Get<LevelGeneratedSignal>().Dispatch(xSize, zSize);
        
        _wallBottom.localPosition = new Vector3(xSize / 2f, 0, -.5f);
        _wallBottom.localScale = new Vector3(xSize + 1, 1, 1);

        _wallTop.localPosition = new Vector3(xSize / 2f, 0, zSize + .5f);
        _wallTop.localScale = new Vector3(xSize * 1.1f, 1, 1);

        _wallLeft.localPosition = new Vector3(-.5f, 0, zSize / 2f);
        _wallLeft.localScale = new Vector3(1, 1, zSize + 1);

        _wallRight.localPosition = new Vector3(xSize + .5f, 0, zSize / 2f);
        _wallRight.localScale = new Vector3(1, 1, zSize + 1);

        var h = 2;
        _finish.localPosition = new Vector3(xSize / 2f, -(h/2) + .17f, zSize - 2f);
        // _finish.localScale = new Vector3(5, h, 2);
    }

    public void UpdateGround(List<Vector3> shapePoints = null)
    {
        // Skip cubes intersected by cutting line
        for (var i = 0; i < shapePoints.Count; i++)
        {
            var cubeX = (int) Mathf.Floor(shapePoints[i].x * _pointsPerUnit);
            var cubeZ = (int) Mathf.Floor(shapePoints[i].z * _pointsPerUnit);
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
                if (MyMaths.ContainsPoint(shapePoints, (new Vector3(x, 0, z) + Vector3.forward * .5f + Vector3.right * .5f) / _pointsPerUnit))
                {
                    _skippedQuads[x, 0, z] = true;
                    // Debug.DrawLine(
                    //     (new Vector3(x, 0, z) + Vector3.forward * .5f + Vector3.right * .5f) / _pointsPerUnit,
                    //     (new Vector3(x, 2, z) + Vector3.forward * .5f + Vector3.right * .5f) / _pointsPerUnit,
                    //     Color.yellow, 999);
                }
            }
        }

        RegenerateMesh(shapePoints);

        var hole = CreateHoleObject(shapePoints);
        var walls = CreateHoleWallObject(shapePoints);
        GiveHoleACollider(hole);

        Signals.Get<HoleGeneratedSignal>().Dispatch(hole, walls);
        Debug.Log($"HoleGeneratedSignal F={Time.frameCount}", gameObject);

        _holeId++;
    }

    private void RegenerateMesh(List<Vector3> shapePoints = null)
    {
        if (_mesh == null)
        {
            GetComponent<MeshFilter>().mesh = _mesh = new Mesh();
            _mesh.name = "Procedural Grid";
            _vertices = new Vector3[(_xPoints + 1) * (_zPoints + 1)];
            _uv = new Vector2[(_xPoints + 1) * (_zPoints + 1)];
            _verticesMoved = new bool[(_xPoints + 1) * (_zPoints + 1)];
            for (int i = 0, z = 0; z <= _zPoints; z++)
            {
                for (int x = 0; x <= _xPoints; x++, i++)
                {
                    _vertices[i] = new Vector3(x / (float) _pointsPerUnit, 0, z / (float) _pointsPerUnit);
                    _uv[i] = new Vector2(x / (float) _xPoints, z / (float) _zPoints);
                }
            }
        }

        // Mess up generated grid vertices
        for (int i = 0, z = 0; z <= _zPoints; z++)
        {
            for (int x = 0; x <= _xPoints; x++, i++)
            {
                if ((x > 0 && z > 0 && x < _xPoints-1 && z < _zPoints-1))
                {
                
                    if (_skippedQuads[x, 0, z] &&
                        (!_skippedQuads[x, 0, z + 1] || !_skippedQuads[x, 0, z - 1] ||
                         !_skippedQuads[x + 1, 0, z] || !_skippedQuads[x - 1, 0, z]
                        )
                    )
                    {
                        DistortPoint(x, z, shapePoints);
                        DistortPoint(x + 1, z, shapePoints);
                        DistortPoint(x, z + 1, shapePoints);
                        DistortPoint(x + 1, z + 1, shapePoints);
                    }
                }

                // recalculate all UV's, some points are moved 
                var vertexIndex = z * (_xPoints + 1) + x;
                _uv[vertexIndex] = MyMaths.squashVector(_vertices[vertexIndex]);
            }
        }

        _mesh.vertices = _vertices;
        _mesh.uv = _uv;

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
    private void DistortPoint(int x, int z, List<Vector3> shapePoints)
    {
        var vertexIndex = z * (_xPoints + 1) + x;
        if (_verticesMoved[vertexIndex]) // Leave already moved ones alone!
        {
            // Debug.DrawLine(
            //     new Vector3(x, 0, z) / _pointsPerUnit,
            //     new Vector3(x, 2, z) / _pointsPerUnit,
            //     Color.black, 999);
            return;
        }

        // Debug.DrawLine(
        //     new Vector3(x, 0, z) / _pointsPerUnit,
        //     new Vector3(x, 2, z) / _pointsPerUnit,
        //     Color.green, 999);

        var vertexPoint = _vertices[vertexIndex];
        var closestDistance = float.MaxValue;
        var closestTrailPointIndex = -1;
        for (var i = 1; i < shapePoints.Count - 1; i++)
        {
            var d = Vector3.Distance(vertexPoint, shapePoints[i]);
            if (d < closestDistance && d < (1.0 / _pointsPerUnit) * 1.7f) // Consider moving only nearby points
            {
                closestDistance = d;
                closestTrailPointIndex = i;
            }
        }

        if (closestTrailPointIndex > 0)
        {
            _vertices[vertexIndex] = shapePoints[closestTrailPointIndex];
            _verticesMoved[vertexIndex] = true;
            // Debug.Log($"d={closestDistance}");
            // Debug.DrawLine(
            //     trailPoints[closestTrailPointIndex],
            //     trailPoints[closestTrailPointIndex] + Vector3.up * 2,
            //     Color.yellow, 999);
        }
    }

    private Transform CreateHoleObject(List<Vector3> shapePoints)
    {
        var startIndex = 1;
        var endIndex = shapePoints.Count - 1;
        Vector3[] vertices;
        Vector2[] uv;
        Mesh mesh;

        var hole = Instantiate(_dynamicHolePrefab, Vector3.zero, Quaternion.identity, transform);
        hole.GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        hole.gameObject.name = mesh.name = $"Hole{_holeId}";
        var num = endIndex - startIndex;
        vertices = new Vector3[num];
        uv = new Vector2[num];

        for (int i = startIndex, k = 0; i < endIndex; i++, k++)
        {
            vertices[k] = shapePoints[i];
            uv[k] = MyMaths.squashVector(vertices[k]);
        }

        mesh.vertices = vertices;
        mesh.uv = uv;

        var triangulator = new Triangulator(vertices);
        mesh.triangles = triangulator.Triangulate();
        mesh.RecalculateNormals();

        var normals = mesh.normals;
        for (var i = 0; i < normals.Length; i++)
        {
            if (normals[i].normalized != Vector3.up)
            {
                // HAXXX: if you retrace your steps and finish a shape, it comes out all messed up, this kind of helps
                // Debug.Log("Fixed inverse hole");
                MyMaths.InverseTriangles(mesh);
                break;
            }
        }

        return hole;
    }

    private Transform CreateHoleWallObject(List<Vector3> shapePoints = null)
    {
        var centerOfMass = MyMaths.CenterOfVectors(shapePoints);
        for (var i = 0; i < shapePoints.Count; i++)
        {
            // Set points relative to the shape center
            shapePoints[i] = shapePoints[i] - centerOfMass;
        }

        var startIndex = 1;
        var endIndex = shapePoints.Count - 1;
        Vector3[] vertices;
        Vector2[] uv;
        Mesh mesh;

        var holeWalls = Instantiate(_dynamicHoleWallPrefab, centerOfMass, Quaternion.identity, transform);
        holeWalls.GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        holeWalls.gameObject.name = mesh.name = $"HoleWall{_holeId}";
        var num = endIndex - startIndex;
        vertices = new Vector3[num * 2];
        uv = new Vector2[num * 2];

        int[] triangles = new int[(num + 1) * 6 * 2];

        // Triangulate band
        var bandLen = Mathf.Sqrt(shapePoints.Sum(x => x.sqrMagnitude));
        var t = 0;
        for (int i = startIndex, k = 0; i < endIndex; i++, k++)
        {
            vertices[k] = shapePoints[i] + Vector3.up;
            vertices[k + num] = shapePoints[i]; // + Vector3.down;

            if (i < endIndex - 1)
            {
                // CW
                triangles[t++] = k;
                triangles[t++] = k + 1;
                triangles[t++] = k + num;

                triangles[t++] = k + 1;
                triangles[t++] = k + 1 + num;
                triangles[t++] = k + num;
            }
            else // Connecting start to end
            {
                triangles[t++] = k;
                triangles[t++] = 0;
                triangles[t++] = k + num;

                triangles[t++] = 0;
                triangles[t++] = num;
                triangles[t++] = k + num;
            }

            // wallpapering the wall
            var percentageOfWall = k / (float) num;
            var c = bandLen * percentageOfWall;
            uv[k] = new Vector2(c, 0);
            uv[k + num] = new Vector2(c, 1);
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return holeWalls;
    }

    private void GiveHoleACollider(Transform hole)
    {
        var mc = hole.GetComponent<MeshCollider>();
        var mesh = hole.GetComponent<MeshFilter>().mesh;

        var newMesh = new Mesh
        {
            vertices = mesh.vertices,
            triangles = mesh.triangles,
        };
        newMesh.RecalculateBounds();
        mc.sharedMesh = newMesh;
    }
}