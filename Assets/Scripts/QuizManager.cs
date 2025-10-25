using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    [Header("Quiz UI References")]
    public GameObject quizPanel;
    public TextMeshProUGUI questionText;
    public TMP_InputField answerInput;
    public Button submitButton;
    public TextMeshProUGUI feedbackText; 

    private GameObject targetFish;

    private void Start()
    {
        // Hide panel initially
        if (quizPanel != null)
            quizPanel.SetActive(false);

        // Hide feedback initially
        if (feedbackText != null)
            feedbackText.text = "";

        // Assign button listener
        if (submitButton != null)
            submitButton.onClick.AddListener(OnSubmitAnswer);
    }

    // Called by UserFish or QuizFish when a quiz is triggered
    public void ShowQuiz(GameObject fishToRemove)
    {
        if (quizPanel != null)
        {
            quizPanel.SetActive(true);
            Time.timeScale = 0f; // Pause game

            // Store the fish reference
            targetFish = fishToRemove;

            // --- Style Setup ---
            questionText.text = "What is 5 + 3?";
            questionText.fontSize = 36f;
            questionText.alignment = TextAlignmentOptions.Center;
            questionText.color = Color.black;

            answerInput.text = "";
            answerInput.textComponent.fontSize = 28f;

            var placeholder = answerInput.placeholder.GetComponent<TextMeshProUGUI>();
            if (placeholder != null)
            {
                placeholder.fontSize = 26f;
                placeholder.color = new Color(0.5f, 0.5f, 0.5f);
            }

            var buttonText = submitButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = "Submit";
                buttonText.fontSize = 28f;
                buttonText.alignment = TextAlignmentOptions.Center;
            }

            if (feedbackText != null)
                feedbackText.text = "";

            Debug.Log("Quiz popup shown!");
        }
        else
        {
            Debug.LogWarning("Quiz panel not assigned in QuizManager!");
        }
    }

    private void OnSubmitAnswer()
    {
        string userAnswer = answerInput.text.Trim();

        if (feedbackText != null)
        {
            if (userAnswer == "8") // Correct answer
            {
                feedbackText.text = "Correct!";
                feedbackText.color = Color.green;

                // Grow the UserFish only after correct answer
                UserFish user = FindFirstObjectByType<UserFish>();
                if (user != null)
                    user.transform.localScale *= 1.2f;

                // Destroy the QuizFish after short delay
                StartCoroutine(CloseQuizAfterDelay(1f));
            }
            else
            {
                feedbackText.text = "Incorrect! Try again.";
                feedbackText.color = Color.red;
            }
        }
    }

    private System.Collections.IEnumerator CloseQuizAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        // Destroy the fish if it still exists
        if (targetFish != null)
            Destroy(targetFish);

        HideQuiz();
    }

    public void HideQuiz()
    {
        quizPanel.SetActive(false);
        Time.timeScale = 1f; // Resume game
        targetFish = null;
    }
}

