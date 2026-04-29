using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class LeaderboardPanel : MonoBehaviour
{
    public Leaderboard leaderboard;
    public TMP_Text[] scores;

    public void WriteScores()
    {
        var entries = leaderboard.Entries();
        for (int i = 0; i < scores.Length; i++)
        {
            var entry = entries.ElementAtOrDefault(i);
            string scoreText;
            if (entry != null)
            {
                scoreText = entry.score.ToString();
            }
            else scoreText = "0.0";

            scores[i].text = scoreText;
        }
    }
}
