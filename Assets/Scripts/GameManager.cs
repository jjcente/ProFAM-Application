using UnityEngine;
using UnityEngine.UI; // if you want to show timer UI
using System.Collections;

public class GameManager : MonoBehaviour
{
    public float timeLimit = 600f; 
    private float timeRemaining;

    public TextMeshProUGUI timerText;
    private bool timerRunning = false;

    public GameObject gameOverPanel;
    public QuizFish[] quizFishes; 
}

