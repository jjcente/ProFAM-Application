using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class QuestionManager : MonoBehaviour
{
    public static QuestionManager Instance { get; private set; }

    public GameObject panel;        // assign QuestionPanel (UI)
    public TMP_Text  questionText;       // UI Text
    public Button[] answerButtons;  // 4 buttons
    public TMP_Text [] answerTexts;      // labels on those buttons

    private Bomb currentBomb;
    private Action<bool> answerCallback;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    // call this to ask a question for a specific bomb
    public void AskQuestion(Bomb bomb, string question, string[] answers, int correctIndex, Action<bool> callback = null)
    {
        currentBomb = bomb;
        answerCallback = callback;
        panel.SetActive(true);
        questionText.text = question;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int idx = i;
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => OnAnswerClicked(idx, correctIndex));
            answerTexts[i].text = answers.Length > i ? answers[i] : "";
        }
    }

    void OnAnswerClicked(int chosen, int correct)
    {
        bool ok = chosen == correct;
        panel.SetActive(false);

        if (ok)
        {
            currentBomb?.Defuse();
        }
        // call optional callback
        answerCallback?.Invoke(ok);
    }
}
