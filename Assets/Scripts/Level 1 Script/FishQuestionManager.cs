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

    for (int i = 0; i < currentQuestion.multipleAnswer.Length; i++)
    {
        if (i == currentQuestion.correctIndex)
            answerButtons[i].image.color = correctColor;
        else if (i == selectedIndex)
            answerButtons[i].image.color = wrongColor;
        else
            answerButtons[i].image.color = defaultColor;
    }

    solutionText.gameObject.SetActive(true);
        solutionText.text = "Solution: " + currentQuestion.solution;
    
    PlayerGrowth playerGrowth = FindFirstObjectByType<PlayerGrowth>();


    // Destroy the fish only if the answer is correct
    if (selectedIndex == currentQuestion.correctIndex)
    {
        // Correct answer → grow
        if (playerGrowth != null)
            playerGrowth.GrowBy(currentFish.pointValue);

        // Destroy the fish
        if (currentFish != null)
            Destroy(currentFish.gameObject);
    }
    else
        {
        
          if (currentFish != null)
            currentFish.IncreaseValue();
            // Wrong answer → shrink
            if (playerGrowth != null)
                playerGrowth.Shrink();

        // Fish stays alive
    }

    StartCoroutine(HidePanelAfterDelay(2f));
}
    private IEnumerator HidePanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        panel.SetActive(false);

        IsQuestionActive = false;

        // Reset button colors for next question
        foreach (Button btn in answerButtons)
            btn.image.color = defaultColor;
    }
}
