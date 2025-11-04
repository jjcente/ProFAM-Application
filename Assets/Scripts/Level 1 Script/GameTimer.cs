using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public float timeLimit = 600f; // 10 minutes
    private float timeRemaining;
    public TextMeshProUGUI timerText;
    private bool timerRunning = true;

    void Start()
    {
        timeRemaining = timeLimit;
    }

    void Update()
    {
        if (!timerRunning) return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;

            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);

            timerText.text = $"{minutes:00}:{seconds:00}";
        }
        else
        {
            timeRemaining = 0;
            timerRunning = false;
            timerText.text = "00:00";
            OnTimeUp();
        }
    }

    void OnTimeUp()
    {
        Debug.Log("⏰ Time's up! Game Over!");
        Time.timeScale = 0f; // pause game
        // Optionally show a Game Over UI here
    }
}

