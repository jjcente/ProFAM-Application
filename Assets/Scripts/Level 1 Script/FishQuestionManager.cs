using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class FishQuestionManager : MonoBehaviour
{
    public static FishQuestionManager Instance { get; private set; }

    [Header("UI References")]
    public GameObject panel;
    public TMP_Text questionText;
    public Button[] answerButtons;
    public TMP_Text solutionText;

    [Header("Database")]
    public FishQuestionDatabase questionDatabase;

    private FishQuestion currentQuestion;
    private Color defaultColor = Color.white; // button default
    private Color correctColor = Color.green;
    private Color wrongColor = Color.red;

    public static bool IsQuestionActive { get; private set; } = false;

    private FishQuestionHolder currentFish;

    public static event System.Action<FishQuestion> OnQuestionAnswered;

    public static bool IsInCooldown { get; private set; } = false;

    private IEnumerator PostQuestionCooldown()
    {
        IsInCooldown = true;
        yield return new WaitForSeconds(0.5f); // adjust delay as needed
        IsInCooldown = false;
    }

    public static void ForceResetState()
    {
        IsQuestionActive = false;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        panel.SetActive(false);
        solutionText.gameObject.SetActive(false);
    }

    private void Start()
    {
        if (questionDatabase != null)
        {
            questionDatabase.LoadQuestions();
        }
    }

    public void ShowRandomQuestion()
    {
        if (questionDatabase == null || questionDatabase.questions == null || questionDatabase.questions.Length == 0)
        {
            Debug.LogWarning("No questions loaded!");
            return;
        }

        FishQuestion question = questionDatabase.GetRandomQuestion();
        if (question == null)
        {
            Debug.LogWarning("No valid question returned from database!");
            return;
        }

        ShowQuestion(question);
    }

    public void ShowQuestion(FishQuestion question, FishQuestionHolder fish = null)
    {
        if (question == null) return;

        currentQuestion = question;
        currentFish = fish; // save reference

        panel.SetActive(true);
        solutionText.gameObject.SetActive(false);
        IsQuestionActive = true;

        questionText.text = question.question;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i < question.multipleAnswer.Length)
            {
                answerButtons[i].gameObject.SetActive(true);
                int index = i;
                answerButtons[i].GetComponentInChildren<TMP_Text>().text = question.multipleAnswer[i];
                answerButtons[i].image.color = defaultColor;
                answerButtons[i].onClick.RemoveAllListeners();
                answerButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void OnAnswerSelected(int selectedIndex)
    {
        if (currentQuestion == null) return;

        foreach (var btn in answerButtons)
            btn.interactable = false;

        // Highlight correct/wrong answers
        for (int i = 0; i < currentQuestion.multipleAnswer.Length; i++)
        {
            if (i == currentQuestion.correctIndex)
                answerButtons[i].image.color = correctColor;
            else if (i == selectedIndex)
                answerButtons[i].image.color = wrongColor;
            else
                answerButtons[i].image.color = defaultColor;
        }

        // Show the solution
        solutionText.gameObject.SetActive(true);
        solutionText.text = "Solution: " + currentQuestion.solution;

        PlayerGrowth playerGrowth = FindFirstObjectByType<PlayerGrowth>();

        // Correct answer → grow and destroy fish
        if (selectedIndex == currentQuestion.correctIndex)
        {
            if (playerGrowth != null)
                playerGrowth.GrowBy(currentFish.pointValue);

            FishAudioManager.Instance.PlayPlayerBite();

            questionDatabase.MarkQuestionAsAnswered(currentQuestion);
            OnQuestionAnswered?.Invoke(currentQuestion);

            if (currentFish != null)
                Destroy(currentFish.gameObject);
        }
        else
        {
            // Wrong answer → shrink but fish stays alive
            if (playerGrowth != null)
                playerGrowth.Shrink();
        }

        questionDatabase.MarkQuestionAsAnswered(currentQuestion);
        OnQuestionAnswered?.Invoke(currentQuestion);

        // ✅ Wait for player tap instead of auto-hiding
        StartCoroutine(WaitForPlayerTapToClose());
    }

    private IEnumerator WaitForPlayerTapToClose()
    {
        // Small delay so the tap used for answering doesn’t close immediately
        yield return new WaitForSeconds(0.3f);

        // Wait until the player taps or clicks anywhere on the screen
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0) || Input.touchCount > 0);

        // Hide the panel after tap
        panel.SetActive(false);
        IsQuestionActive = false;

        // Reset buttons for next question
        foreach (Button btn in answerButtons)
        {
            btn.image.color = defaultColor;
            btn.interactable = true;
        }

        StartCoroutine(PostQuestionCooldown());
    }

    private IEnumerator HidePanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        panel.SetActive(false);
        IsQuestionActive = false;

        foreach (Button btn in answerButtons)
        {
            btn.image.color = defaultColor;
            btn.interactable = true;
        }

        StartCoroutine(PostQuestionCooldown());
    }

    public void ForceCloseQuestionPanel()
    {
        if (panel != null)
            panel.SetActive(false);

        IsQuestionActive = false;
        IsInCooldown = false;

        PlayerDragController player = FindFirstObjectByType<PlayerDragController>();
        if (player != null)
            player.enabled = true;

        foreach (var fish in FindObjectsByType<FishQuestionHolder>(FindObjectsSortMode.None))
        {
            var ai = fish.GetComponent<FishMovement>();
            if (ai != null)
                ai.enabled = true;
        }

        Debug.Log("🐟 ForceCloseQuestionPanel(): Re-enabled all movement and closed panel.");
    }
}
