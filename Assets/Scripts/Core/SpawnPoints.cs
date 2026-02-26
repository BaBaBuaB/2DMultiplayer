using System.Collections.Generic;
using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
    private static List<SpawnPoints> spawnPoints = new List<SpawnPoints>();

    private void OnEnable()
    {
        spawnPoints.Add(this);
    }
    private void OnDisable()
    {
        spawnPoints.Remove(this);
    }
    public static Vector3 GetRandomSpawnPos()
    {
        if(spawnPoints.Count == 0)
        {
            return Vector3.zero;
        }
        return spawnPoints[Random.Range(0, spawnPoints.Count)].transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 1);
    }
}
