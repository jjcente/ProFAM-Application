using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

[System.Serializable]
public class SubQuestion
{
    public string question;
    public string correct;
    public string[] options;
}

[System.Serializable]
public class Quiz
{
    public int quizNumber;
    public string problem;
    public SubQuestion[] subQuestions;
}

[System.Serializable]
public class QuizCollection
{
    public Quiz[] quizzes;
}

public class QuizManager : MonoBehaviour
{
    [Header("Quiz UI References")]
    public GameObject quizPanel;
    public TextMeshProUGUI questionText;
    public List<Button> choiceButtons;
    public TextMeshProUGUI feedbackText;

    private GameObject targetFish;    
    public UserFish playerFish;     
    private string correctAnswer = "";

    private int currentSubQuestion = 0;
    private List<(string question, string correct, string[] options)> subQuestions;

    private QuizCollection allQuizzes;

    private void Start()
    {
        LoadQuizzesFromJSON();

        if (quizPanel != null)
        {
            quizPanel.SetActive(false);
            CanvasGroup cg = quizPanel.GetComponent<CanvasGroup>();
            if (cg == null) cg = quizPanel.AddComponent<CanvasGroup>();
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }

        if (feedbackText != null)
            feedbackText.text = "";

        foreach (Button btn in choiceButtons)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnChoiceSelected(btn));
        }
    }

    private void LoadQuizzesFromJSON()
    {
        TextAsset quizJSON = Resources.Load<TextAsset>("quizzes"); // quizzes.json in Resources folder
        if (quizJSON != null)
        {
            allQuizzes = JsonUtility.FromJson<QuizCollection>(quizJSON.text);
        }
        else
        {
            Debug.LogError("⚠️ Quiz JSON not found in Resources!");
        }
    }

    // Call this from QuizFish on collision
    public void SetPlayerFish(UserFish player)
    {
        playerFish = player;
    }

    public void ShowQuiz(GameObject fishToRemove) => ShowQuizFromJSON(fishToRemove, 1);
    public void ShowQuiz2(GameObject fishToRemove) => ShowQuizFromJSON(fishToRemove, 2);
    public void ShowQuiz3(GameObject fishToRemove) => ShowQuizFromJSON(fishToRemove, 3);
    public void ShowQuiz4(GameObject fishToRemove) => ShowQuizFromJSON(fishToRemove, 4);
    public void ShowQuiz5(GameObject fishToRemove) => ShowQuizFromJSON(fishToRemove, 5);

    private void ShowQuizFromJSON(GameObject fishToRemove, int quizNumber)
    {
        if (allQuizzes == null)
        {
            Debug.LogError("⚠️ Quizzes not loaded!");
            return;
        }

        Quiz selectedQuiz = null;
        foreach (Quiz q in allQuizzes.quizzes)
        {
            if (q.quizNumber == quizNumber)
            {
                selectedQuiz = q;
                break;
            }
        }

        if (selectedQuiz != null)
        {
            targetFish = fishToRemove;
            currentSubQuestion = 0;
            subQuestions = new List<(string question, string correct, string[] options)>();
            foreach (var sq in selectedQuiz.subQuestions)
            {
                subQuestions.Add((sq.question, sq.correct, sq.options));
            }

            SetupQuiz(selectedQuiz.problem);
        }
        else
        {
            Debug.LogWarning("⚠️ Quiz number not found in JSON!");
        }
    }

    private void SetupQuiz(string problem)
    {
        if (quizPanel == null) return;

        quizPanel.SetActive(true);
        CanvasGroup cg = quizPanel.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }

        currentSubQuestion = 0;
        DisplaySubQuestion(problem);
    }

    private void DisplaySubQuestion(string problem)
    {
        var current = subQuestions[currentSubQuestion];
        correctAnswer = current.correct.ToLower().Trim();

        if (questionText != null)
        {
            questionText.text = $"{problem}\n\n<b>{current.question}</b>";
            questionText.fontSize = 40f;
            questionText.alignment = TextAlignmentOptions.TopLeft;
            questionText.textWrappingMode = TMPro.TextWrappingModes.Normal;
        }

        for (int i = 0; i < choiceButtons.Count && i < current.options.Length; i++)
        {
            var btnText = choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null)
            {
                btnText.text = current.options[i];
                btnText.fontSize = 30f;
                btnText.color = Color.black;
            }
        }

        if (feedbackText != null)
            feedbackText.text = "";
    }

    private void OnChoiceSelected(Button selectedButton)
    {
        string selectedAnswer = selectedButton.GetComponentInChildren<TextMeshProUGUI>().text.Trim().ToLower();
        string selectedLetter = selectedAnswer.Substring(0, 1);

        if (selectedLetter == correctAnswer)
        {
            feedbackText.text = "Correct!";
            feedbackText.color = Color.green;

            if (playerFish != null)
            {
                float growthAmount = 0.2f;
                playerFish.transform.localScale += new Vector3(growthAmount, growthAmount, 0f);
            }

            currentSubQuestion++;
            if (currentSubQuestion < subQuestions.Count)
                StartCoroutine(NextSubQuestionAfterDelay(1.2f));
            else
                StartCoroutine(CloseQuizAfterDelay(1.2f));
        }
        else
        {
            feedbackText.text = "Incorrect! Quiz closed.";
            feedbackText.color = Color.red;

            if (playerFish != null)
            {
                float shrinkAmount = 0.2f;
                playerFish.transform.localScale -= new Vector3(shrinkAmount, shrinkAmount, 0f);
            }

            StartCoroutine(CloseQuizAfterDelayWrongAnswer(0.8f));
        }
    }

    private IEnumerator NextSubQuestionAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        DisplaySubQuestion("");
    }

    private IEnumerator CloseQuizAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        if (targetFish != null)
            Destroy(targetFish); // destroy quiz fish only on correct answer

        HideQuiz();
    }

    private IEnumerator CloseQuizAfterDelayWrongAnswer(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        HideQuiz(); // closes quiz but does NOT destroy quiz fish
    }

    public void HideQuiz()
    {
        if (quizPanel != null)
        {
            CanvasGroup cg = quizPanel.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.interactable = false;
                cg.blocksRaycasts = false;
            }

            quizPanel.SetActive(false);
        }

        Time.timeScale = 1f;

        targetFish = null;
        StartCoroutine(ResetEventSystemNextFrame());
        Debug.Log("🎯 Quiz closed — gameplay resumed!");
    }

    private IEnumerator ResetEventSystemNextFrame()
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void TriggerQuiz(GameObject fish, int quizNumber)
    {
        ShowQuizFromJSON(fish, quizNumber);
    }
}

