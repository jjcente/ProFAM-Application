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

    // Simple 10 PISA-style questions
    private static List<(string question, string answer, string[] choices)> questions = new List<(string, string, string[])>
    {
        ("If a pencil costs 10 pesos and a notebook costs 5 times more, how much is the notebook?", "50", new string[]{"40", "50", "60"}),
        ("A train leaves at 3:00 PM and arrives at 5:30 PM. How long is the trip?", "2.5", new string[]{"3", "2.5", "2"}),
        ("You have ₱500 and spend ₱275. How much is left?", "225", new string[]{"200", "225", "250"}),
        ("A pizza is cut into 8 equal slices. If you eat 3, what fraction is left?", "5/8", new string[]{"3/8", "5/8", "4/8"}),
        ("If water freezes at 0°C, what is this in °F?", "32", new string[]{"0", "32", "100"}),
        ("There are 24 hours in a day. How many hours in 3 days?", "72", new string[]{"60", "72", "48"}),
        ("If 6 apples cost ₱60, how much for 1 apple?", "10", new string[]{"5", "10", "15"}),
        ("Your phone battery drops from 100% to 40%. How much percentage is used?", "60", new string[]{"40", "50", "60"}),
        ("If a rectangle’s length is 8 and width is 3, what’s its area?", "24", new string[]{"20", "24", "28"}),
        ("If 12 students share 3 pizzas equally, how many slices does each get if each pizza has 8 slices?", "2", new string[]{"3", "4", "2"})
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
