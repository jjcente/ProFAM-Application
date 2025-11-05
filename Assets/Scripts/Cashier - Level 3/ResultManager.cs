using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ResultManager : MonoBehaviour
{
    public GameObject resultPanel;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI highScoreText;
    public Button continueButton;
    public Button exitButton;

    private bool resultShown = false;
    private int highScore = 0;

    void Start()
    {
        if (resultPanel != null)
            resultPanel.SetActive(false);

        // Load saved high score
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    public void ShowFinalResult(int finalScore)
    {
        if (resultShown) return;
        resultShown = true;

        if (resultPanel != null)
            resultPanel.SetActive(true);

        // Check and update high score
        if (finalScore > highScore)
        {
            highScore = finalScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }

        // Show both scores with an 8-bit style animation
        StartCoroutine(ShowScoreTransition(finalScore));

        // ✅ Continue button: restart current game
        if (continueButton != null)
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(() =>
            {
                ResetGameData();
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            });
        }

        // ✅ Exit button: reset all level data (except main menu & loading), then go to main menu
        if (exitButton != null)
        {
            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(() =>
            {

                ResetAllExceptMenuAndLoading();
                SceneManager.LoadScene(0); // Go back to main menu
            });
        }
    }

    IEnumerator ShowScoreTransition(int finalScore)
    {
        if (resultText != null && highScoreText != null)
        {
            resultText.text = "";
            highScoreText.text = "";

            string finalStr = "FINAL SCORE: " + finalScore.ToString();
            string highStr = "HIGH SCORE: " + highScore.ToString();

            // 8-bit typewriter effect
            foreach (char c in finalStr)
            {
                resultText.text += c;
                yield return new WaitForSeconds(0.03f);
            }

            yield return new WaitForSeconds(0.3f);

            foreach (char c in highStr)
            {
                highScoreText.text += c;
                yield return new WaitForSeconds(0.03f);
            }
        }
    }

    // ✅ Reset current game data but keep high score
    void ResetGameData()
    {
        NPCController.ResetGameData();
    }

    // ✅ Reset all level data except main menu (0) and loading screen (1)
    void ResetAllExceptMenuAndLoading()
    {
        int totalScenes = SceneManager.sceneCountInBuildSettings;

        for (int i = 2; i < totalScenes; i++) // start from index 2 onwards
        {
            string key = "LevelData_" + i;
            if (PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.DeleteKey(key);
                Debug.Log("Reset data for Level (Scene Index): " + i);
            }
        }

        PlayerPrefs.Save();
        NPCController.ResetGameData();
    }
}
