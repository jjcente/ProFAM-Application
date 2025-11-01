using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.Collections;

public class QuestionManager : MonoBehaviour
{
    public static QuestionManager Instance { get; private set; }

    public GameObject panel;        // assign QuestionPanel (UI)
    public TMP_Text  questionText;       // UI Text
    public Button[] answerButtons;  // 4 buttons
    public TMP_Text[] answerTexts;      // labels on those buttons
    public float showCorrectTime = 2f;

    private Bomb currentBomb;
    private Action<bool> answerCallback;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

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

        // Re-enable buttons in case they were disabled before
        answerButtons[i].interactable = true;
    }
}
  void OnAnswerClicked(int chosen, int correct)
    {
        bool ok = chosen == correct;

        if (ok)
        {
            panel.SetActive(false);
            currentBomb?.Defuse();
            answerCallback?.Invoke(true);
        }
        else
        {
            StartCoroutine(HandleIncorrectAnswer(correct));
        }
    }
    
        private IEnumerator HandleIncorrectAnswer(int correctIndex)
    {
        Color originalColor = answerButtons[correctIndex].image.color;
        answerButtons[correctIndex].image.color = Color.green;

        foreach (var btn in answerButtons) btn.interactable = false;

        yield return new WaitForSeconds(showCorrectTime);

        answerButtons[correctIndex].image.color = originalColor;

        currentBomb?.Explode();
        panel.SetActive(false);

        answerCallback?.Invoke(false);
    }

}
