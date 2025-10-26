using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class QuizManager : MonoBehaviour
{
    [Header("Quiz UI References")]
    public GameObject quizPanel;
    public TextMeshProUGUI questionText;
    public TMP_InputField answerInput;
    public Button submitButton;
    public TextMeshProUGUI feedbackText;

    [Header("Control Panel Reference")]
    public GameObject arrowKeys;

    private GameObject targetFish;
    private string correctAnswer = "";

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

        // Ensure arrow keys panel is visible when the game starts
        if (arrowKeys != null)
            arrowKeys.SetActive(true);
    }

    // === QUIZ 1 ===
    public void ShowQuiz(GameObject fishToRemove)
    {
        SetupQuiz(fishToRemove, "What is 5 + 3?", "8");
    }

    // === QUIZ 2 ===
    public void ShowQuiz2(GameObject fishToRemove)
    {
        SetupQuiz(fishToRemove, "What color do you get when you mix red and blue?", "purple");
    }

    // === QUIZ 3 ===
    public void ShowQuiz3(GameObject fishToRemove)
    {
        SetupQuiz(fishToRemove, "What planet is known as the Red Planet?", "mars");
    }

    // === QUIZ 4 ===
    public void ShowQuiz4(GameObject fishToRemove)
    {
        SetupQuiz(fishToRemove, "How many sides does a triangle have?", "3");
    }

    // === QUIZ 5 ===
    public void ShowQuiz5(GameObject fishToRemove)
    {
        SetupQuiz(fishToRemove, "What is the largest mammal on Earth?", "blue whale");
    }

    // === Common Quiz Setup Function ===
    private void SetupQuiz(GameObject fishToRemove, string question, string answer)
    {
        if (quizPanel != null)
        {
            quizPanel.SetActive(true);
            Time.timeScale = 0f; // Pause game

            if (arrowKeys != null)
                arrowKeys.SetActive(false);

            targetFish = fishToRemove;
            correctAnswer = answer.ToLower();

            // --- Style Setup ---
            questionText.text = question;
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

    // === SUBMIT ANSWER ===
    private void OnSubmitAnswer()
    {
        string userAnswer = answerInput.text.Trim().ToLower();

        if (feedbackText != null)
        {
            if (userAnswer == correctAnswer)
            {
                feedbackText.text = "✅ Correct!";
                feedbackText.color = Color.green;

                // Grow the UserFish only after correct answer
                UserFish user = FindFirstObjectByType<UserFish>();
                if (user == null)
                    Debug.LogWarning("⚠️ UserFish not found in scene!");
                else
                    user.transform.localScale *= 1.2f; // Small growth per quiz

                // Destroy the QuizFish after short delay
                StartCoroutine(CloseQuizAfterDelay(1f));
            }
            else
            {
                feedbackText.text = "❌ Incorrect! Try again.";
                feedbackText.color = Color.red;
            }
        }
    }

    private IEnumerator CloseQuizAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        if (targetFish != null)
            Destroy(targetFish);

        HideQuiz();
    }

    public void HideQuiz()
    {
        if (quizPanel != null)
            quizPanel.SetActive(false);

        Time.timeScale = 1f;
        targetFish = null;

        if (arrowKeys != null)
            arrowKeys.SetActive(true);

        Debug.Log("Quiz closed — arrow keys re-enabled!");
    }
}

