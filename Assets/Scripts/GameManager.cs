using UnityEngine;
using TMPro; // Required for TextMeshProUGUI
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Timer Settings")]
    public float timeLimit = 600f; // 10 minutes
    private float timeRemaining;
    private bool timerRunning = false;

    [Header("UI References")]
    public TextMeshProUGUI timerText;
    // public GameObject gameOverPanel; // Commented out for now

    [Header("Game References")]
    public QuizFish[] quizFishes; // All QuizFish in the scene

    private void Start()
    {
        if (timerText != null)
          timerText.text = "Timer Active!";

        timeRemaining = timeLimit;
        timerRunning = true;
        StartCoroutine(TimerCoroutine());
    }

    private IEnumerator TimerCoroutine()
    {
        while (timerRunning && timeRemaining > 0f)
        {
            // Use unscaledDeltaTime to keep timer running even if Time.timeScale = 0
            timeRemaining -= Time.unscaledDeltaTime;

            // Update timer UI
            if (timerText != null)
            {
                int minutes = Mathf.FloorToInt(timeRemaining / 60f);
                int seconds = Mathf.FloorToInt(timeRemaining % 60f);
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }

            // Check if all fishes are cleared
            if (AllFishesCleared())
            {
                timerRunning = false;
                Debug.Log("🎉 All fishes cleared! You win!");
                yield break;
            }

            yield return null;
        }

        if (timeRemaining <= 0f)
        {
            GameOver();
        }
    }

    private bool AllFishesCleared()
    {
        foreach (QuizFish fish in quizFishes)
        {
            if (fish != null && fish.gameObject.activeInHierarchy)
                return false;
        }
        return true;
    }

    private void GameOver()
    {
        timerRunning = false;
        // if (gameOverPanel != null)
        //     gameOverPanel.SetActive(true); // Commented out for now

        Debug.Log("⏰ Time's up! Game Over.");
        Time.timeScale = 0f; // Optional: pause the game
    }
}

