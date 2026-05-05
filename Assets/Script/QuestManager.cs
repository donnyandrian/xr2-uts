using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static Unity.VisualScripting.Member;

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
    public TMP_Text levelText;
    public TMP_Text scoreText;
    public TMP_Text taskText;
    public DroppedSpices droppedSpices;
    public SocketObjectGetter socket;
    public ResetTransform plate;
    public Leaderboard leaderboard;

    [Header("Round Range Group")]
    public RoundRangeGroup[] roundGroups;

    [Header("Events")]
    public UnityEvent OnCorrectAnswer;
    public UnityEvent OnWrongAnswer;

    private SpiceObject[] target;
    private bool _isProcessingScore = false;

    async void Start()
    {
        UpdateLevelScoreText(bezierScore.GetCurrentScore());
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

            target = new SpiceObject[quest.Length];

            string _dbgQuestObjects = "Quest Objects:";
            taskText.text = "";
            for (int i = 0; i < quest.Length; i++)
            {
                var spice = spawner.GetSpice(quest[i]);
                target[i] = spice;
                taskText.text += spice.spiceName + Environment.NewLine;
                _dbgQuestObjects += " (" + spice.spiceName + ")";
            }
            Debug.Log(_dbgQuestObjects);
        }
        catch (System.OperationCanceledException)
        {
            Debug.Log("Spawning was cancelled because the object was destroyed.");
        }
    }

    public async void UpdateScore()
    {
        if (_isProcessingScore) return;
        _isProcessingScore = true;
        try
        {
            bool isCorrect = IsCorrect();
            if (isCorrect) OnCorrectAnswer?.Invoke();
            else OnWrongAnswer?.Invoke();
            Debug.Log("Is correct: " + isCorrect.ToString());

            ClearSelected();

            var currentScore = bezierScore.CalculateTotalScore(isCorrect);
            UpdateLevelScoreText(currentScore);
            await GenerateQuest();
        }
        finally
        {
            _isProcessingScore = false;
        }
    }

    private void UpdateLevelScoreText(float currentScore)
    {
        levelText.text = $"Round {bezierScore.GetCurrentRound() + 1}";
        scoreText.text = "Score: " + Mathf.Round(currentScore).ToString();
    }

    private bool IsCorrect()
    {
        Array.Sort(target, StringComparer.OrdinalIgnoreCase);
        droppedSpices.spices.Sort((a, b) => StringComparer.OrdinalIgnoreCase.Compare(a.spiceName, b.spiceName));

        if (droppedSpices.spices.Count <= 0 || droppedSpices.spices.Count != target.Length)
        {
            Debug.Log("111111111111111111111111111111111111111111111111");
            Debug.Log(droppedSpices.spices.Count);
            Debug.Log(target.Length);
            return false;
        }
        else if (droppedSpices.spices.Count == target.Length)
        {
            for (int i = 0; i < target.Length; i++)
            {
                if (target[i].spiceName != droppedSpices.spices[i].spiceName)
                {
                    Debug.Log("22222222222222222222222222222222222222222222222");
                    return false;
                }
            }
        }

        return true;
    }

    public void ClearSelected()
    {
        droppedSpices.spices.Clear();

        for (int i = socket.interacted.Count - 1; i >= 0; i--)
        {
            Destroy(socket.interacted[i].gameObject);
        }

        socket.interacted.Clear();

        plate.ResetToInitialState();
    }

    public void SaveScore()
    {
        leaderboard.AddEntry(new(bezierScore.GetCurrentScore(), DateTime.Now));
        leaderboard.SaveData();
    }
}
