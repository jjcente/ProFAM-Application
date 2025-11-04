using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Timer Settings")]
    public float gameDuration = 600f; // 10 minutes
    private float remainingTime;

    [Header("UI")]
    public TMP_Text timerText;

    private bool isGameActive = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;

        StartGame();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reassign the timer UI in the new scene
        timerText = FindFirstObjectByType<TMP_Text>();

        // Reset and restart the timer
        StartGame();
    }

    public void StartGame()
    {
        StopAllCoroutines(); // Stop any old timer
        remainingTime = gameDuration;
        isGameActive = true;

        UpdateTimerUI();
        StartCoroutine(TimerCoroutine());
    }

    private IEnumerator TimerCoroutine()
    {
        while (remainingTime > 0 && isGameActive)
        {
            UpdateTimerUI();
            yield return new WaitForSeconds(1f);
            remainingTime -= 1f;
        }

        if (remainingTime <= 0)
        {
            // Simply reload the scene when time runs out
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(remainingTime / 60f);
            int seconds = Mathf.FloorToInt(remainingTime % 60f);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
