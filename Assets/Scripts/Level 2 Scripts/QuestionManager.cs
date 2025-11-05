using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.Collections;

public class QuestionManager : MonoBehaviour
{
    public static QuestionManager Instance { get; private set; }

    [Header("UI References")]
    public GameObject panel;          // Question panel
    public TMP_Text questionText;     // Question text
    public Button[] answerButtons;    // Answer buttons
    public TMP_Text[] answerTexts;    // Text on buttons
    public TMP_Text solutionText;     // Solution text

    [Header("Settings")]
    public float showCorrectTime = 2f;

    [Header("SFX")]
    public AudioClip correctClip;
    public AudioClip wrongClip;

    private Bomb currentBomb;
    private Action<bool> answerCallback;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
        solutionText.gameObject.SetActive(false);
    }

   public void AskQuestion(Bomb bomb, Question question, Action<bool> callback = null)
    {
        currentBomb = bomb;
        answerCallback = callback;
        panel.SetActive(true);

        questionText.text = question.question;
        solutionText.gameObject.SetActive(false);
        solutionText.text = "";

        // Setup answer buttons
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int idx = i;
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => OnAnswerClicked(idx, question));
            answerTexts[i].text = question.answers.Length > i ? question.answers[i] : "";
            answerButtons[i].interactable = true;
            answerButtons[i].image.color = Color.white; // reset color
        }
    }

    private void OnAnswerClicked(int chosen, Question question)
    {
        bool ok = chosen == question.correctIndex;

        // Disable all buttons immediately
        foreach (var btn in answerButtons)
            btn.interactable = false;

        // Show the solution
        solutionText.text = "Solution: " + question.solution;
        solutionText.gameObject.SetActive(true);

        if (ok)
        {
            // ✅ Correct answer
            answerButtons[chosen].image.color = Color.green;
            AudioManager.Instance.PlaySFX(correctClip);
            currentBomb?.Defuse();
            StartCoroutine(HidePanelAfterDelay());
            answerCallback?.Invoke(true);
        }
        else
        {
            // ❌ Wrong answer
            answerButtons[chosen].image.color = Color.red;
            AudioManager.Instance.PlaySFX(wrongClip);
            StartCoroutine(HandleIncorrectAnswer(question, chosen));
        }
    }

    private IEnumerator HandleIncorrectAnswer(Question question, int chosen)
    {
        int correctIndex = question.correctIndex;

        // Highlight correct button green
        answerButtons[correctIndex].image.color = Color.green;

        yield return new WaitForSeconds(showCorrectTime);

        // Reset colors (optional)
        answerButtons[chosen].image.color = Color.white;
        answerButtons[correctIndex].image.color = Color.white;

        // Explode bomb and close panel
        currentBomb?.Explode();
        panel.SetActive(false);

        answerCallback?.Invoke(false);
    }

    private IEnumerator HidePanelAfterDelay()
    {
        yield return new WaitForSeconds(showCorrectTime);
        panel.SetActive(false);
    }
}
