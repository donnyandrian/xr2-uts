using System;
using UnityEngine;

public class ObectSpawner : MonoBehaviour
{
    public int minObjectToSpawn = 10;
    public int maxObjectToSpawn;
    public int minObjectKindToSpawn = 1;
    public GameObject[] objects;
    public GameObject[] points;

    bool HasAny<T>(T[] arr, T val)
    {
        foreach (var item in arr)
        {
            if (item.Equals(val)) return true;
        }

        return false;
    }

    void Start()
    {
        int spawnPointCounts = UnityEngine.Random.Range(1, points.Length);
        int[] spawnPointIndicies = new int[spawnPointCounts];

        for (int i = 0; i < spawnPointCounts; i++)
        {
            int pointIndex;
            do
            {
                pointIndex = UnityEngine.Random.Range(0, points.Length - 1);
            }
            while (HasAny(spawnPointIndicies, pointIndex));

            spawnPointIndicies[i] = pointIndex;
        }

        int objectKindToSpawnCount = UnityEngine.Random.Range(minObjectKindToSpawn, objects.Length);
        int[] objectIndicies = new int[objectKindToSpawnCount];

        for (int i = 0; i < objectKindToSpawnCount; i++)
        {
            int objectIndex;
            do
            {
                objectIndex = UnityEngine.Random.Range(0, objects.Length - 1);
            }
            while (HasAny(objectIndicies, objectIndex));

            objectIndicies[i] = objectIndex;
        }

        int objectCount = UnityEngine.Random.Range(minObjectToSpawn, maxObjectToSpawn);
        foreach (int spawnIndex in spawnPointIndicies)
        {
            for (int i = 0; i < objectCount; i++)
            {
                foreach (int objectIndex in objectIndicies)
                {
                    Instantiate(objects[objectIndex], points[spawnIndex].transform);
                }
            }
        }
    }

    void Update()
    {
        
    }
}
