using System.Collections.Generic;
using UnityEngine;

public class MyMath
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

    // From XZ plane uz XY plane
    public static Vector3 squashVector(Vector3 v3)
    {
        return new Vector3(v3.x, v3.z, 0);
    }

    // https://wiki.unity3d.com/index.php/PolyContainsPoint
    public static bool ContainsPoint(List<Vector3> trailPoints, int startIndex, Vector3 p)
    {
        var j = trailPoints.Count - 1;
        var inside = false;
        for (int i = startIndex; i < trailPoints.Count; j = i++)
        {
            var pi = trailPoints[i];
            var pj = trailPoints[j];
            if (((pi.z <= p.z && p.z < pj.z) || (pj.z <= p.z && p.z < pi.z)) && (p.x < (pj.x - pi.x) * (p.z - pi.z) / (pj.z - pi.z) + pi.x)) inside = !inside;
        }

        return inside;
    }
}