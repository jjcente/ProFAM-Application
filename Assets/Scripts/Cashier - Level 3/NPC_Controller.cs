using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class NPCController : MonoBehaviour
{
    public Transform counterPoint;
    public Transform exitPoint;
    public Transform topScreenPosition; // Position for dialogue at top of screen
    public Transform defaultDialoguePosition; // Default dialogue position (before moving)
    public float moveSpeed = 2f;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI timerText;   // Timer display text (no clock icon)
    public GameObject dialogueCanvas;
    public GameObject choicePanel;      // Panel containing 3 answer buttons
    public Button[] choiceButtons;      // The 3 buttons inside the panel
    public NPCManager manager;

    // 🎵 Audio
    public AudioSource correctAnswerAudio;  // For correct answer sound
    public AudioSource wrongAnswerAudio;    // For wrong answer sound

    private bool movingToCounter = true;
    private bool interacted = false;
    private bool isInteracting = false;
    private bool waitingForAnswer = false;

    private string currentQuestion;
    private string currentAnswer;
    private Coroutine answerTimerCoroutine;

    // 📘 10 math word problems with 3 choices each
    private static List<(string question, string answer, string[] choices)> questions = new List<(string, string, string[])>
    {
        (
            "A student wants to subscribe to an online e-book library. Plan A: ₱4,500 per year. Plan B: ₱150/month + ₱30 per e-book (10 books/month). When is Plan A cheaper?",
            "12 months",
            new string[]{"7 months", "10 months", "12 months"}
        ),
        (
            "A coffee shop uses 2.5 kg of coffee beans every day. Bags are 12 kg each. How many bags are needed for 14 days?",
            "3 bags",
            new string[]{"3 bags", "4 bags", "5 bags"}
        ),
        (
            "A school replaces 85 bulbs. Single: ₱150 each, or 12 for ₱585. What is the total minimum cost?",
            "₱4,950.00",
            new string[]{"₱4,920.00", "₱4,950.00", "₱5,010.00"}
        ),
        (
            "2.5 liters of paint cover 20 sqm. How many liters for 70 sqm?",
            "8.75 liters",
            new string[]{"7.5 liters", "8.75 liters", "10.0 liters"}
        ),
        (
            "A souvenir costs $150. Exchange rate: ₱55 = $1. How much in pesos?",
            "₱8,250.00",
            new string[]{"₱8,400.00", "₱8,250.00", "₱7,950.00"}
        ),
        (
            "200g of butter makes 24 cookies. How much for 60 cookies?",
            "500 g",
            new string[]{"400 g", "500 g", "600 g"}
        ),
        (
            "A van travels 360 km using 45L. With 10L left, how far can it go?",
            "80 km",
            new string[]{"80 km", "90 km", "100 km"}
        ),
        (
            "A box of 10 pens costs ₱80. A single pen costs ₱10. How much is saved per pen?",
            "₱2.00",
            new string[]{"₱1.00", "₱1.50", "₱2.00"}
        ),
        (
            "A 500 km trip uses 10L per 100 km. Fuel costs ₱50/L. What is the total cost?",
            "₱2,500.00",
            new string[]{"₱2,000.00", "₱2,500.00", "₱3,000.00"}
        ),
        (
            "A machine makes 150 items in 20 minutes. How many in 3 hours?",
            "1,350 items",
            new string[]{"900 items", "1,125 items", "1,350 items"}
        )
    };

    void Start()
    {
        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(false);

        if (choicePanel != null)
            choicePanel.SetActive(false);

        if (timerText != null)
            timerText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isInteracting)
        {
            if (movingToCounter && !interacted)
            {
                MoveTo(counterPoint.position);

                if (Vector2.Distance(transform.position, counterPoint.position) < 0.05f)
                {
                    StartCoroutine(Interact());
                }
            }
            else if (interacted)
            {
                MoveTo(exitPoint.position);

                if (Vector2.Distance(transform.position, exitPoint.position) < 0.05f)
                {
                    if (manager != null)
                        manager.SpawnNextNPC();

                    Destroy(gameObject);
                }
            }
        }
    }

    void MoveTo(Vector2 target)
    {
        transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
    }

    IEnumerator Interact()
    {
        isInteracting = true;
        movingToCounter = false;

        if (questions.Count == 0)
        {
            dialogueText.text = "No more questions left!";
            yield return new WaitForSeconds(2f);
            interacted = true;
            isInteracting = false;
            yield break;
        }

        int randomIndex = Random.Range(0, questions.Count);
        currentQuestion = questions[randomIndex].question;
        currentAnswer = questions[randomIndex].answer;
        string[] choices = questions[randomIndex].choices;
        questions.RemoveAt(randomIndex);

        // Show question
        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(true);

        if (dialogueText != null)
            dialogueText.text = currentQuestion;

        yield return new WaitForSeconds(4f);

        // Move dialogue to top
        if (topScreenPosition != null && dialogueCanvas != null)
        {
            dialogueCanvas.transform.position = topScreenPosition.position;
            dialogueCanvas.transform.rotation = topScreenPosition.rotation;
        }

        // Show choices
        if (choicePanel != null)
        {
            choicePanel.SetActive(true);

            // Randomize button order
            List<string> shuffled = new List<string>(choices);
            for (int i = 0; i < shuffled.Count; i++)
            {
                string temp = shuffled[i];
                int random = Random.Range(i, shuffled.Count);
                shuffled[i] = shuffled[random];
                shuffled[random] = temp;
            }

            for (int i = 0; i < choiceButtons.Length; i++)
            {
                if (i < shuffled.Count)
                {
                    string answer = shuffled[i];
                    choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = answer;
                    choiceButtons[i].onClick.RemoveAllListeners();
                    choiceButtons[i].onClick.AddListener(() =>
                    {
                        SubmitChoice(answer);
                    });
                }
            }
        }

        waitingForAnswer = true;

        // Start 10-second timer
        answerTimerCoroutine = StartCoroutine(AnswerTimer());

        yield return new WaitUntil(() => !waitingForAnswer);

        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(false);

        if (timerText != null)
            timerText.gameObject.SetActive(false);

        interacted = true;
        isInteracting = false;
    }

    public void SubmitChoice(string selectedAnswer)
    {
        if (!waitingForAnswer) return;

        StopCoroutine(answerTimerCoroutine);

        string result;

        if (selectedAnswer == currentAnswer)
        {
            result = "Here's Your Change!";
            PlayCorrectSound();
        }
        else
        {
            result = $"Wrong! The correct answer is {currentAnswer}.";
            PlayWrongSound();
        }

        if (choicePanel != null)
            choicePanel.SetActive(false);

        StartCoroutine(ShowResultAndReturn(result));
    }

    IEnumerator ShowResultAndReturn(string result)
    {
        if (defaultDialoguePosition != null && dialogueCanvas != null)
        {
            dialogueCanvas.transform.position = defaultDialoguePosition.position;
            dialogueCanvas.transform.rotation = defaultDialoguePosition.rotation;
        }

        if (dialogueText != null)
        {
            dialogueText.text = result;
            dialogueCanvas.SetActive(true);
        }

        if (timerText != null)
            timerText.gameObject.SetActive(false);

        yield return new WaitForSeconds(2f);
        waitingForAnswer = false;
    }

    IEnumerator AnswerTimer()
    {
        float timer = 10f;

        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
            timerText.text = "10s";
        }

        while (timer > 0f)
        {
            timer -= Time.deltaTime;

            if (timerText != null)
                timerText.text = Mathf.CeilToInt(timer).ToString() + "s";

            yield return null;

            if (!waitingForAnswer)
                yield break;
        }

        // Time's up
        if (choicePanel != null)
            choicePanel.SetActive(false);

        if (defaultDialoguePosition != null && dialogueCanvas != null)
        {
            dialogueCanvas.transform.position = defaultDialoguePosition.position;
            dialogueCanvas.transform.rotation = defaultDialoguePosition.rotation;
        }

        if (dialogueText != null)
            dialogueText.text = "Time’s up!!";

        if (timerText != null)
            timerText.gameObject.SetActive(false);

        yield return new WaitForSeconds(2f);
        waitingForAnswer = false;
    }

    // ✅ Correct answer sound
    public void PlayCorrectSound()
    {
        if (correctAnswerAudio != null && correctAnswerAudio.clip != null)
            correctAnswerAudio.Play();
    }

    // ❌ Wrong answer sound
    public void PlayWrongSound()
    {
        if (wrongAnswerAudio != null && wrongAnswerAudio.clip != null)
            wrongAnswerAudio.Play();
    }
}
