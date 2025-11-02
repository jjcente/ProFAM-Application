using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    public GameObject resultPanel;
    public TextMeshProUGUI resultText;
    public Button continueButton;
    public Button exitButton;

    private bool resultShown = false;

    void Start()
    {
        if (resultPanel != null)
            resultPanel.SetActive(false);
    }

    public void ShowFinalResult(int finalScore)
    {
        if (resultShown) return; // ✅ prevent multiple calls
        resultShown = true;

        if (resultPanel != null)
            resultPanel.SetActive(true);

        if (resultText != null)
            resultText.text = "Final Score: " + finalScore.ToString();

        if (continueButton != null)
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            });
        }

        if (exitButton != null)
        {
            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(() =>
            {
                Application.Quit();
            });
        }

        // Reset NPC score count for next game
        NPCController.npcCount = 0;
        NPCController.totalScore = 0;
    }
}
