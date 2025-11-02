using UnityEngine;
using UnityEngine.SceneManagement; // ✅ Required for loading scenes

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
        Debug.Log("Returning to Main Menu...");
        Time.timeScale = 1f; // ✅ Ensure time resumes normally
        SceneManager.LoadScene(0); // ✅ Load the Main Menu (Scene at index 0)
    }
}
