using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHub : MonoBehaviour
{
    public int homeSceneIndex;
    public int gameSceneIndex;
    public int leaderboardSceneIndex;

    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            Debug.Log("Loading progress: " + (progress * 100) + "%");

            yield return null;
        }
    }

    public void GoToHome()
    {
        StartCoroutine(LoadAsynchronously(homeSceneIndex));
    }

    public void GoToGame()
    {
        StartCoroutine(LoadAsynchronously(gameSceneIndex));
    }

    public void GoToLeaderboard()
    {
        StartCoroutine(LoadAsynchronously(leaderboardSceneIndex));
    }
}
