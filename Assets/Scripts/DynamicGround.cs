using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class DynamicGround : MonoBehaviour
{
    [SerializeField, Tooltip("In Unity units")]
    private int xSize, zSize;

    [SerializeField, Tooltip("Density")] private int pointPerUnit;

    private int xPoints, zPoints;
    private Mesh mesh;
    private bool[,,] skippedQuads; // x,y,z => is quad skipped 

    private Vector3[] vertices;

    private void Start()
    {
        xPoints = xSize * pointPerUnit;
        zPoints = zSize * pointPerUnit;
        skippedQuads = new bool[xPoints, 1, zPoints];
        RegenerateMesh();
    }

    public void UpdateGround(List<Vector3> trailPoints = null, int startIndex = 0)
    {
        // Skip cubes intersected by cutting line
        for (var i = startIndex; i < trailPoints.Count; i++)
        {
            var cubeX = (int) Mathf.Floor(trailPoints[i].x * pointPerUnit);
            var cubeZ = (int) Mathf.Floor(trailPoints[i].z * pointPerUnit);
            if (!skippedQuads[cubeX, 0, cubeZ])
            {
                skippedQuads[cubeX, 0, cubeZ] = true;
                Debug.Log($"cut out {cubeX} 0 {cubeZ}");
                Debug.DrawLine(
                    (new Vector3(cubeX, 0, cubeZ) + Vector3.forward * .5f + Vector3.right * .5f) / pointPerUnit,
                    (new Vector3(cubeX, 1, cubeZ) + Vector3.forward * .5f + Vector3.right * .5f) / pointPerUnit,
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
        int maxZ = skippedQuads.GetLength(2);
        int maxX = skippedQuads.GetLength(0);
        for (int z = 1; z < maxZ - 1; z++)
        {
            for (int x = 1; x < maxX - 1; x++)
            {
                if (skippedQuads[x, 0, z] && !skippedQuads[x + 1, 0, z])  //[ ][+]...
                {
                    if ( SomeMaths.IsPointInPolygon(trailPoints, startIndex, (new Vector3(x, 0, z) + Vector3.forward * .5f + Vector3.right * .5f) / pointPerUnit))  
                    {
                            skippedQuads[x, 0, z] = true;
                            Debug.DrawLine(
                                (new Vector3(x, 0, z) + Vector3.forward * .5f + Vector3.right * .5f) / pointPerUnit,
                                (new Vector3(x, 2, z) + Vector3.forward * .5f + Vector3.right * .5f) / pointPerUnit,
                                Color.yellow, 999);
                    }
                }

                // var upMissing = skippedQuads[x, 0, z + 1];
                // var downMissing = skippedQuads[x, 0, z - 1];

                // if (upMissing && downMissing && !skippedQuads[x, 0, z])
                // {
                //     Debug.Log($"additionally skipped: {x} 0 {z}");
                //
                //     Debug.DrawLine(
                //         (new Vector3(x, 0, z) + Vector3.forward * .5f + Vector3.right * .5f) / pointPerUnit,
                //         (new Vector3(x, 2, z) + Vector3.forward * .5f + Vector3.right * .5f) / pointPerUnit,
                //         Color.yellow);
                //     skippedQuads[x, 0, z] = true;
                // }
            }
        }

        RegenerateMesh();
    }

    // find next hole to the right
    private bool IsThisAnIslandX(int xStart, int z)
    {
        int maxX = skippedQuads.GetLength(0);
        for (int x = xStart+1; x < maxX - 1; x++)
        {
            if (skippedQuads[x, 0, z])
            {
                return true;
            }
        }

        return false;
    }
    
    private bool IsThisAnIslandZ(int x, int zStart)
    {
        int maxZ = skippedQuads.GetLength(2);
        for (int z = zStart+1; z < maxZ - 1; z++)
        {
            if (skippedQuads[x, 0, z])
            {
                return true;
            }
        }

        return false;
    }

    private void RegenerateMesh()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Grid";
        vertices = new Vector3[(xPoints + 1) * (zPoints + 1)];
        for (int i = 0, z = 0; z <= zPoints; z++)
        {
            for (int x = 0; x <= xPoints; x++, i++)
            {
                vertices[i] = new Vector3(x / (float) pointPerUnit, 0, z / (float) pointPerUnit);
            }
        }

        mesh.vertices = vertices;

        int[] triangles = new int[xPoints * zPoints * 6];
        for (int ti = 0, vi = 0, z = 0; z < zPoints; z++, vi++)
        {
            for (int x = 0; x < xPoints; x++, ti += 6, vi++)
            {
                if (skippedQuads[x, 0, z]) continue;

                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xPoints + 1;
                triangles[ti + 5] = vi + xPoints + 2;
            }
        }

        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}