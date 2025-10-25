using UnityEngine;

public class QuizManager : MonoBehaviour
{
    public GameObject quizPanel;

    public void ShowQuiz()
    {
        if (quizPanel != null)
        {
            quizPanel.SetActive(true);
            Debug.Log("Quiz triggered!");
        }
        else
        {
            Debug.LogWarning("Quiz panel not assigned in QuizManager!");
        }
    }

    public void HideQuiz()
    {
        if (quizPanel != null)
            quizPanel.SetActive(false);
    }
}

