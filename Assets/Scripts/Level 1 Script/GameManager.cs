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

    [Header("Win State")]
    public GameObject portalPrefab;       // show portal when winning
    public Transform portalSpawnPoint;
    public string nextSceneName = "Level2";
    public float delayBeforeNextScene = 5f; // delay in seconds

    private bool isGameActive = false;
    private bool hasWon = false;

    private PlayerDragController playerController;

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

        if (FishAudioManager.Instance != null)
    {
        FishAudioManager.Instance.RestartBackgroundAudio();
    }
    else
    {
        Debug.LogWarning("⚠️ No FishAudioManager found in scene — background audio won't play!");
    }

        StartGame();    
    }

    public void StartGame()
    {
        StopAllCoroutines(); // stop any old timers
        remainingTime = gameDuration;
        isGameActive = true;
        hasWon = false;


        UpdateTimerUI();
        StartCoroutine(TimerCoroutine());

    }

    private IEnumerator TimerCoroutine()
    {
        while (remainingTime > 0 && isGameActive && !hasWon)
        {
            UpdateTimerUI();
            yield return new WaitForSeconds(1f);
            remainingTime -= 1f;
        }

        if (remainingTime <= 0 && !hasWon)
        {

            if (FishQuestionManager.Instance != null)
{
    FishQuestionManager.Instance.ForceCloseQuestionPanel();
}
            // Time's up → reset questions & reload scene
            if (FishQuestionManager.Instance?.questionDatabase != null)
            {
                FishQuestionManager.Instance.questionDatabase.ResetQuestions();
                Debug.Log("✅ Questions reset for new game.");
            }


    StopAllCoroutines(); // ✅ stops only GameManager coroutines
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

    // Call this from PlayerGrowth after updating weight
    public void CheckWinCondition(float playerWeight)
    {
        if (hasWon) return;

        if (Mathf.Approximately(playerWeight, 10f))
        {
            TriggerWinState();
        }
    }

    private void TriggerWinState()
{
    hasWon = true;

    // Fade background music, bubbles, water
    if (FishAudioManager.Instance != null)
        StartCoroutine(FishAudioManager.Instance.FadeOutAllBackground(1f));

    // Play stage clear SFX
    FishAudioManager.Instance?.PlayStageClear();

    // Disable question manager
    if (FishQuestionManager.Instance != null)
        FishQuestionManager.Instance.enabled = false;

    // Disable player input
    playerController = FindFirstObjectByType<PlayerDragController>();
    if (playerController != null)
        playerController.enabled = false;

    Debug.Log("🎉 Stage Cleared! Spawning portal in 3 seconds...");

    // Delay portal spawn
    StartCoroutine(SpawnPortalWithDelay(3f));

    // Delay then load next scene
    StartCoroutine(LoadNextSceneAfterDelay());
}

    private IEnumerator LoadNextSceneAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeNextScene);

        // Destroy all root objects in the current scene except persistent managers
        Scene currentScene = SceneManager.GetActiveScene();
        GameObject[] rootObjects = currentScene.GetRootGameObjects();
        foreach (GameObject obj in rootObjects)
        {
            // Skip persistent managers
            if (obj.GetComponent<FishAudioManager>() != null) continue;
            if (obj.GetComponent<LoadingScreenManager>() != null) continue;
            if (obj == this.gameObject) continue; // skip for now
            Destroy(obj);
        }

        // Load next scene
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            LoadingScreenManager.LoadSceneByName(nextSceneName);
        }

        // Destroy this GameManager after loading next scene
        Destroy(this.gameObject);
    }

private IEnumerator SpawnPortalWithDelay(float delay)
{
    yield return new WaitForSeconds(delay);

    if (portalPrefab != null && portalSpawnPoint != null)
    {
        GameObject portal = Instantiate(portalPrefab, portalSpawnPoint.position, Quaternion.identity);

        // Assign next scene to the portal if using the Portal script
        Portal portalScript = portal.GetComponent<Portal>();
        if (portalScript != null)
            portalScript.nextSceneName = nextSceneName;
    }
}

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
