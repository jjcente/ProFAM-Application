using UnityEngine;
using UnityEngine.SceneManagement; // ✅ Required for loading scenes
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // Assign your Pause Menu Canvas here
    public static bool isPaused = false;


    void Update()
    {
        // Press Escape (or any key you like) to toggle pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true); // Show the pause menu
        Time.timeScale = 0f;         // Freeze the game
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false); // Hide the pause menu
        Time.timeScale = 1f;          // Resume normal time
        isPaused = false;
    }


    public void QuitGame()
    {
        StartCoroutine(QuitGameCoroutine());
    }

    private IEnumerator QuitGameCoroutine()
    {
        Debug.Log("Returning to Main Menu...");

        // Ensure time resumes
        Time.timeScale = 1f;
        yield return new WaitForSeconds(0.5f);

        // Fade out any playing background sounds
        if (FishAudioManager.Instance != null)
        {
            Debug.Log("Fading out Level 1 audio...");
            yield return StartCoroutine(FishAudioManager.Instance.FadeOutAllBackground(1f));
        }

        // Reset all question systems
        if (FishQuestionManager.Instance?.questionDatabase != null)
        {
            FishQuestionManager.Instance.questionDatabase.ResetQuestions();
            Debug.Log("✅ Questions reset for new game.");
        }

        // Reset other static flags
        PauseMenu.isPaused = false;
        FishQuestionManager.ForceResetState();
        FeaturePanelManager.ForceResetState();

        // Destroy the GameManager
        GameManager gameManager = GameObject.FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            Debug.Log("🗑️ Destroying GameManager completely.");
            Destroy(gameManager.gameObject);
        }

        // Destroy all root objects except persistent managers
        Scene currentScene = SceneManager.GetActiveScene();
        foreach (GameObject obj in currentScene.GetRootGameObjects())
        {
            if (obj.GetComponent<FishAudioManager>() != null) continue;
            if (obj.GetComponent<LoadingScreenManager>() != null) continue;
            if (obj == this.gameObject) continue; // skip this PauseMenu for now

            Destroy(obj);
        }

        //Level3

        NPCController.ResetGameData();

        // Load Main Menu
        SceneManager.LoadScene(0);

        // Finally destroy this PauseMenu
        Debug.Log("🗑️ PauseMenu destroyed for full reset.");
        Destroy(this.gameObject);
    }

}
