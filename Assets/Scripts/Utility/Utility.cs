using UnityEngine;
using UnityEngine.AI;

public static class Utility
{
    //NavMeshß¾ªÇ«é«ó«À«àªÊEöÇªòö¢Ôğª¹ªE
    public static Vector3 GetRandomPointOnNavMesh(Vector3 center, float distance, int areaMask)
    {
        var randomPos = Random.insideUnitSphere * distance + center;

        NavMeshHit hit;

        NavMesh.SamplePosition(randomPos, out hit, distance, areaMask);
        if (float.IsInfinity(hit.position.x))
        {
            hit.position = GetRandomPointOnNavMesh(center, distance, areaMask);
        }
        return hit.position;
    }

    /*
    public static float GedRandomNormalDistribution(float mean, float standard)
    {
        var x1 = Random.Range(0f, 0.6f);
        var x2 = Random.Range(0f, 0.6f);
        return mean + standard * (Mathf.Sqrt(-2.0f * Mathf.Log(x1)) * Mathf.Sin(2.0f * Mathf.PI * x2));
    }
    */
}