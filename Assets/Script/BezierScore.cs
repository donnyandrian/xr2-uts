using UnityEditor;
using UnityEngine;

public class BezierScore : MonoBehaviour
{
    public AnimationCurve curve;

    [Header("Round & Score")]
    public int totalRound = 100;
    public float scoreMin = 1f;
    public float scoreMax = 1000f;

    private int _currentRound = 0;
    private float _currentScore = 0f;

    private const float _progressMin = 0.1f;
    private const float _progressMax = 1f;

    private void Start()
    {
        curve.ClearKeys();
        curve.AddKey(new Keyframe(0f, _progressMax, -2f, -2f, 0f, 0.25f));
        curve.AddKey(new Keyframe(1f, _progressMin, 0f, 0f, 0.3f, 0f));

        totalRound = Mathf.Max(totalRound, 1);
        scoreMax = Mathf.Max(scoreMax, scoreMin);
    }

    public float GetScore(bool isCorrect)
    {
        if (!isCorrect) _currentRound = 0;

        // 1 - 0.1
        var progress = curve.Evaluate(Mathf.Clamp01(_currentRound / totalRound));

        // Remap progress to 1000 - 1
        const float progressRange = _progressMax - _progressMin;
        float scoreRange = scoreMax - scoreMin;
        float score = scoreMin + scoreRange * (progress - _progressMin) / progressRange;

        if (!isCorrect) score *= -1f;

        _currentRound++;
        _currentScore += score;

        return score;
    }

    public void ResetScore()
    {
        _currentRound = 0;
        _currentScore = 0f;
    }
}
