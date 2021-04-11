using System.Collections.Generic;
using UnityEngine;

public class MyMaths
{
    // https://www.habrador.com/tutorials/math/5-line-line-intersection/
    public static bool AreLinesIntersecting(Vector3 l1_p1, Vector3 l1_p2, Vector3 l2_p1, Vector3 l2_p2, bool shouldIncludeEndPoints = true)
    {
        l1_p1 = squashVector(l1_p1);
        l1_p2 = squashVector(l1_p2);
        l2_p1 = squashVector(l2_p1);
        l2_p2 = squashVector(l2_p2);

        //To avoid floating point precision issues we can add a small value
        float epsilon = 0.00001f;

        bool isIntersecting = false;

        float denominator = (l2_p2.y - l2_p1.y) * (l1_p2.x - l1_p1.x) - (l2_p2.x - l2_p1.x) * (l1_p2.y - l1_p1.y);

        //Make sure the denominator is > 0, if not the lines are parallel
        if (denominator != 0f)
        {
            float u_a = ((l2_p2.x - l2_p1.x) * (l1_p1.y - l2_p1.y) - (l2_p2.y - l2_p1.y) * (l1_p1.x - l2_p1.x)) / denominator;
            float u_b = ((l1_p2.x - l1_p1.x) * (l1_p1.y - l2_p1.y) - (l1_p2.y - l1_p1.y) * (l1_p1.x - l2_p1.x)) / denominator;

            //Are the line segments intersecting if the end points are the same
            if (shouldIncludeEndPoints)
            {
                //Is intersecting if u_a and u_b are between 0 and 1 or exactly 0 or 1
                if (u_a >= 0f + epsilon && u_a <= 1f - epsilon && u_b >= 0f + epsilon && u_b <= 1f - epsilon)
                {
                    isIntersecting = true;
                }
            }
            else
            {
                //Is intersecting if u_a and u_b are between 0 and 1
                if (u_a > 0f + epsilon && u_a < 1f - epsilon && u_b > 0f + epsilon && u_b < 1f - epsilon)
                {
                    isIntersecting = true;
                }
            }
        }

        return isIntersecting;
    }

    public static bool IsCBetweenAB(Vector3 A, Vector3 B, Vector3 C)
    {
        return Vector3.Dot((B - A).normalized, (C - B).normalized) < 0f && Vector3.Dot((A - B).normalized, (C - A).normalized) < 0f;
    }

    // From XZ plane uz XY plane
    public static Vector3 squashVector(Vector3 v3)
    {
        return new Vector3(v3.x, v3.z, 0);
    }

    // https://wiki.unity3d.com/index.php/PolyContainsPoint
    public static bool ContainsPoint(List<Vector3> shapePoints, Vector3 p)
    {
        var j = shapePoints.Count - 1;
        var inside = false;
        for (int i = 0; i < shapePoints.Count; j = i++)
        {
            var pi = shapePoints[i];
            var pj = shapePoints[j];
            if (((pi.z <= p.z && p.z < pj.z) || (pj.z <= p.z && p.z < pi.z)) && (p.x < (pj.x - pi.x) * (p.z - pi.z) / (pj.z - pi.z) + pi.x)) inside = !inside;
        }

        return inside;
    }

    // https://stackoverflow.com/a/51906100/207757
    public static Vector2 FindNearestPointOnLine(Vector2 origin, Vector2 end, Vector2 point)
    {
        //Get heading
        Vector2 heading = (end - origin);
        float magnitudeMax = heading.magnitude;
        heading.Normalize();

        //Do projection from the point but clamp it
        Vector2 lhs = point - origin;
        float dotP = Vector2.Dot(lhs, heading);
        dotP = Mathf.Clamp(dotP, 0f, magnitudeMax);
        return origin + heading * dotP;
    }

    // https://stackoverflow.com/a/18472899/207757
    public static bool IsClockwise(IList<Vector3> vertices)
    {
        var sum = 0.0;
        for (var i = 0; i < vertices.Count; i++)
        {
            var v1 = vertices[i];
            var v2 = vertices[(i + 1) % vertices.Count];
            sum += (v2.x - v1.x) * (v2.z + v1.z);
        }

        return sum > 0.0;
    }
    
    public static void InverseTriangles(Mesh mesh)
    {
        var indices = mesh.triangles;
        var triangleCount = indices.Length / 3;
        for (var i = 0; i < triangleCount; i++)
        {
            var tmp = indices[i * 3];
            indices[i * 3] = indices[i * 3 + 1];
            indices[i * 3 + 1] = tmp;
        }

        mesh.triangles = indices;
        mesh.RecalculateNormals();
    }
    
}