using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class TimerManager : MonoBehaviour
{
    [Range(0f, 24.0f * 60.0f * 60.0f)]
    public float duration = 3.0f * 60.0f;

    public TMP_Text timerText;

    public UnityEvent OnTimerFinished;

    private float _timeRemaining;
    private bool _isTimerRunning;

    void Start()
    {
        _timeRemaining = duration;
        _isTimerRunning = true;
    }

    void Update()
    {
        if (!_isTimerRunning) return;

        if (_timeRemaining > 0f)
        {
            _timeRemaining -= Time.deltaTime;
            UpdateDisplay(_timeRemaining);
        }
        else
        {
            Debug.Log("Times Up");
            _timeRemaining = 0f;
            _isTimerRunning = false;
            UpdateDisplay(_timeRemaining);
            OnTimerFinished?.Invoke();
        }
    }

    void UpdateDisplay(float timeToDisplay)
    {
        System.TimeSpan time = System.TimeSpan.FromSeconds(timeToDisplay);
        string timeStr;

        if (time.Hours > 0)
        {
            timeStr = time.ToString(@"hh\:mm\:ss\:ff");
        }
        else if (time.Minutes > 0)
        {
            timeStr = time.ToString(@"mm\:ss\:ff");
        }
        else
        {
            timeStr = time.ToString(@"ss\:ff");
        }

        //Debug.Log(timeStr);
        timerText.text = timeStr;
    }

    public void StartTimer() => _isTimerRunning = true;
    public void StopTimer() => _isTimerRunning = false;
    public void ResetTimer() => _timeRemaining = duration;

    public void FreezeGame()
    {
        Debug.Log("Game Frozen");

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
