using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoundRangeGroup
{
    public int from = 0;
    public int objectKindToSpawn = 1;
}

public class FromComparer : IComparer<RoundRangeGroup>
{
    public int Compare(RoundRangeGroup x, RoundRangeGroup y)
    {
        return x.from.CompareTo(y.from);
    }
}

public class QuestManager : MonoBehaviour
{
    [Header("Setup")]
    public ObjectSpawner spawner;
    public BezierScore bezierScore;

    [Header("Round Range Group")]
    public RoundRangeGroup[] roundGroups;

    async void Start()
    {
        await GenerateQuest();
    }

    int[] RandomRetrive(int[] arr, int n)
    {
        var _rand = new Unity.Mathematics.Random((uint)System.DateTime.Now.Ticks);

        int[] result = new int[n];
        for (int i = 0; i < n; i++)
        {
            int randomIndex = _rand.NextInt(i, arr.Length);

            (arr[randomIndex], arr[i]) = (arr[i], arr[randomIndex]);
            result[i] = arr[i];
        }
        return result;
    }

    RoundRangeGroup FindGroup(int targetValue)
    {
        var searchItem = new RoundRangeGroup { from = targetValue };
        int index = Array.BinarySearch(roundGroups, searchItem, new FromComparer());

        // Exact match found
        if (index >= 0)
        {
            return roundGroups[index];
        }

        // No exact match. Get the bitwise complement (~index)
        // This gives us the index of the first element GREATER than the target.
        int nextLargerIndex = ~index;

        // The target belongs to the element immediately BEFORE the next larger one.
        int targetIndex = nextLargerIndex - 1;
        if (targetIndex >= 0)
        {
            return roundGroups[targetIndex];
        }

        // Target is smaller than the very first "from" value
        return null;
    }

    int GetObjectKindCount()
    {
        int currentRound = bezierScore.GetCurrentRound();
        if (roundGroups.Length < 1 || currentRound < roundGroups[0].from) return 0;

        var toSpawn = FindGroup(currentRound)?.objectKindToSpawn ?? 0;
        Debug.Log("Round " + currentRound.ToString() + " asks for " + toSpawn.ToString() + " kind(s)");
        return toSpawn;
    }

    public async Awaitable GenerateQuest()
    {
        try
        {
            // If this GameObject is destroyed during the nth second of spawning,
            // the code stops immediately and doesn't cause errors.
            int[] objectIndicies = await spawner.SpawnObjects(destroyCancellationToken);
            int[] quest = RandomRetrive(objectIndicies, GetObjectKindCount());

            string _dbgQuestObjects = "Quest Objects:";
            foreach (int i in quest)
            {
                _dbgQuestObjects += " (" + spawner.GetSpice(i).spiceName + ")";
            }
            Debug.Log(_dbgQuestObjects);
        }
        catch (System.OperationCanceledException)
        {
            Debug.Log("Spawning was cancelled because the object was destroyed.");
        }
    }
}
