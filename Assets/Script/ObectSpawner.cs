using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObectSpawner : MonoBehaviour
{
    public int minObjectToSpawn = 10;
    public int maxObjectToSpawn = 10;

    public int minObjectKindToSpawn = 1;
    public int maxObjectKindToSpawn = 1;

    public int minPointToSpawn = 1;
    public int maxPointToSpawn = 1;

    public float spawnSpeed = 8.0f;

    public GameObject[] objects;
    public Transform[] points;

    void Start()
    {
        var objectsLength = objects.Length;
        var pointsLength = points.Length;
        if (objects == null || objectsLength <= 0)
        {
            Debug.LogError("The objects to be spawned have not yet been defined");
            return;
        }
        if (points == null || pointsLength <= 0)
        {
            Debug.LogError("The spawn points have not yet been defined");
            return;
        }

        minObjectToSpawn = Mathf.Max(minObjectToSpawn, 1);
        maxObjectToSpawn = Mathf.Max(maxObjectToSpawn, minObjectToSpawn);

        minObjectKindToSpawn = Mathf.Clamp(minObjectKindToSpawn, 1, objectsLength);
        maxObjectKindToSpawn = Mathf.Clamp(maxObjectKindToSpawn, minObjectKindToSpawn, objectsLength);

        minPointToSpawn = Mathf.Clamp(minPointToSpawn, 1, pointsLength);
        maxPointToSpawn = Mathf.Clamp(maxPointToSpawn, minPointToSpawn, pointsLength);

        StartCoroutine(SpawnObjects());
    }

    // Fisher-Yates Shuffle
    int[] GetUniquesRandom(int n, int min, int max)
    {
        var _rand = new Unity.Mathematics.Random((uint)System.DateTime.Now.Ticks);
        
        int[] possibilities = Enumerable.Range(min, max - min).ToArray();
        int[] result = new int[n];

        for (int i = 0; i < n; i++)
        {
            int randomIndex = _rand.NextInt(i, possibilities.Length);

            (possibilities[randomIndex], possibilities[i]) = (possibilities[i], possibilities[randomIndex]);
            result[i] = possibilities[i];
        }

        return result;
    }

    int[] RandomEliminate(int[] arr)
    {
        var _rand = new Unity.Mathematics.Random((uint)System.DateTime.Now.Ticks);

        var max = arr.Length - 1;
        int n = _rand.NextInt(Mathf.Min(_rand.NextInt(3, 5), max), max);
        int[] result = new int[n];
        for (int i = 0; i < n; i++)
        {
            int randomIndex = _rand.NextInt(i, arr.Length);

            (arr[randomIndex], arr[i]) = (arr[i], arr[randomIndex]);
            result[i] = arr[i];
        }
        return result;
    }

    IEnumerator SpawnObjects()
    {
        Debug.Log("Start Spawning");

        var _rand = new Unity.Mathematics.Random((uint)System.DateTime.Now.Ticks);

        int[] pointIndicies = GetUniquesRandom(_rand.NextInt(minPointToSpawn, maxPointToSpawn), 0, points.Length);
        int[] objectIndicies = GetUniquesRandom(_rand.NextInt(minObjectKindToSpawn, maxObjectKindToSpawn), 0, objects.Length);

        Debug.Log("ObjectIndicies: " + objectIndicies.Length.ToString());
        Debug.Log("PointIndicies: " + pointIndicies.Length.ToString());

        // A dictionary where each point index holds a Queue of objects it needs to spawn
        Dictionary<int, Queue<int>> pointSpawningQueues = new();
        foreach (int objectIndex in objectIndicies)
        {
            int[] spawnPointIndicies = RandomEliminate(pointIndicies);
            foreach (int pointIndex in spawnPointIndicies)
            {
                if (!pointSpawningQueues.ContainsKey(pointIndex))
                    pointSpawningQueues[pointIndex] = new();

                int objectCount = _rand.NextInt(minObjectToSpawn, maxObjectToSpawn);
                for (int i = 0; i < objectCount; i++)
                {
                    pointSpawningQueues[pointIndex].Enqueue(objectIndex);
                }
            }
        }

        var spawnPauseDur = 1.0f / spawnSpeed;
        bool itemsRemaining = true;
        while (itemsRemaining)
        {
            itemsRemaining = false; // Assume we are done unless we find an object to spawn

            // Iterate through every possible point in one "cycle"
            foreach (int pointIndex in pointIndicies)
            {
                // Check if this specific point has anything left in its queue
                if (pointSpawningQueues.ContainsKey(pointIndex) && pointSpawningQueues[pointIndex].Count > 0)
                {
                    int objectIndex = pointSpawningQueues[pointIndex].Dequeue();

                    // Spawn the object
                    var obj = Instantiate(objects[objectIndex]);
                    obj.transform.SetParent(points[pointIndex]);
                    obj.transform.localPosition = Vector3.zero;
                    obj.transform.localRotation = Quaternion.identity;

                    itemsRemaining = true; // We spawned something, so we need to check the points again
                }
            }

            // After EVERY point has had a chance to spawn ONE object, wait.
            // This creates the "all at once" visual effect.
            if (itemsRemaining)
            {
                yield return new WaitForSeconds(spawnPauseDur);
            }
        }

        Debug.Log("Done");
    }
}
