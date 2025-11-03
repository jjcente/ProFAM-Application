using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class NPCController : MonoBehaviour
{
    public Transform counterPoint;
    public Transform exitPoint;
    public Transform topScreenPosition;
    public Transform defaultDialoguePosition;
    public float moveSpeed = 2f;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI timerText;
    public GameObject dialogueCanvas;
    public GameObject choicePanel;
    public Button[] choiceButtons;
    public NPCManager manager;

    public AudioSource correctAnswerAudio;
    public AudioSource wrongAnswerAudio;

    public GameObject cashAnimation;
    public float animationDuration = 2f;

    public static int totalScore = 0;
    public static int npcCount = 0;

    private bool movingToCounter = true;
    private bool interacted = false;
    private bool isInteracting = false;
    private bool waitingForAnswer = false;

    private string currentQuestion;
    private string currentAnswer;
    private string currentSolution;
    private Coroutine answerTimerCoroutine;

    public ResultManager resultManager;
    public TextMeshProUGUI scoreDisplay;

    // ✅ Default question set with inline solutions (commas instead of new lines)
    private static readonly List<(string question, string answer, string[] choices, string solution)> defaultQuestions =
        new List<(string, string, string[], string)>
    {
        ("A COFFEE SHOP USES 2.5 KG OF COFFEE BEANS EVERY DAY. BAGS ARE 12 KG EACH. HOW MANY BAGS ARE NEEDED FOR 14 DAYS?",
         "3 BAGS", new string[]{"3 BAGS", "4 BAGS", "5 BAGS"},
         "SOLUTION: 2.5 × 14 = 35 kg, 35 ÷ 12 = 2.92 ≈ 3 bags"),

        ("A SCHOOL REPLACES 85 BULBS. SINGLE: P150 EACH, OR 12 FOR P585. WHAT IS THE TOTAL MINIMUM COST?",
         "P4245.00", new string[]{"P4245.00", "P4950.00", "P5010.00"},
         "SOLUTION: 7 boxes = 84 bulbs, 7 × 585 = 4095, +1 bulb = 150, 4095 + 150 = 4245"),

        ("2.5 LITERS OF PAINT COVER 20 SQM. HOW MANY LITERS FOR 70 SQM?",
         "8.75 LITERS", new string[]{"7.5 LITERS", "8.75 LITERS", "10.0 LITERS"},
         "SOLUTION: 70 × (2.5 ÷ 20) = 8.75 liters"),

        ("A SOUVENIR COSTS $150. EXCHANGE RATE: P55 = $1. HOW MUCH IN PESOS?",
         "P8250.00", new string[]{"P8400.00", "P8250.00", "P7950.00"},
         "SOLUTION: 150 × 55 = 8250 pesos"),

        ("200G OF BUTTER MAKES 24 COOKIES. HOW MUCH FOR 60 COOKIES?",
         "500 G", new string[]{"400 G", "500 G", "600 G"},
         "SOLUTION: (60 ÷ 24) × 200 = 500 g"),

        ("A VAN TRAVELS 360 KM USING 45L. WITH 10L LEFT, HOW FAR CAN IT GO?",
         "80 KM", new string[]{"80 KM", "90 KM", "100 KM"},
         "SOLUTION: 360 ÷ 45 = 8 km/L, 10 × 8 = 80 km"),

        ("A BOX OF 10 PENS COSTS P80. A SINGLE PEN COSTS P10. HOW MUCH IS SAVED PER PEN?",
         "P2.00", new string[]{"P1.00", "P1.50", "P2.00"},
         "SOLUTION: 10 × 10 = 100, 100 − 80 = 20, 20 ÷ 10 = 2 per pen"),

        ("A 500 KM TRIP USES 10L PER 100 KM. FUEL COSTS P50/L. WHAT IS THE TOTAL COST?",
         "P2500.00", new string[]{"P2000.00", "P2500.00", "P3000.00"},
         "SOLUTION: 500 ÷ 100 = 5, 5 × 10 = 50L, 50 × 50 = 2500 pesos"),

        ("A MACHINE MAKES 150 ITEMS IN 20 MINUTES. HOW MANY IN 3 HOURS?",
         "1350 ITEMS", new string[]{"900 ITEMS", "1125 ITEMS", "1350 ITEMS"},
         "SOLUTION: 3 hr = 180 min, 180 ÷ 20 = 9, 150 × 9 = 1350 items"),

        ("A SURVEY ESTIMATES THAT 80% ±5% OF CUSTOMERS PREFER COFFEE OVER TEA. IF THERE ARE 200 CUSTOMERS, WHAT IS THE RANGE OF CUSTOMERS THAT MIGHT PREFER COFFEE?",
         "150 TO 170 CUSTOMERS", new string[]{"150 TO 170 CUSTOMERS", "155 TO 165 CUSTOMERS", "160 TO 180 CUSTOMERS"},
         "SOLUTION: 80% of 200 = 160, ±5% of 200 = ±10, Range = 150 to 170 customers")
    };

    private static List<(string question, string answer, string[] choices, string solution)> questions =
        new List<(string, string, string[], string)>(defaultQuestions);

    void Start()
    {
        if (dialogueCanvas != null) dialogueCanvas.SetActive(false);
        if (choicePanel != null) choicePanel.SetActive(false);
        if (timerText != null) timerText.gameObject.SetActive(false);
        if (cashAnimation != null) cashAnimation.SetActive(false);
        UpdateScoreDisplay();
    }

    void Update()
    {
        if (!isInteracting)
        {
            if (movingToCounter && !interacted)
            {
                MoveTo(counterPoint.position);
                if (Vector2.Distance(transform.position, counterPoint.position) < 0.05f)
                    StartCoroutine(Interact());
            }
            else if (interacted)
            {
                MoveTo(exitPoint.position);
                if (Vector2.Distance(transform.position, exitPoint.position) < 0.05f)
                {
                    npcCount++;
                    if (npcCount >= 10)
                        resultManager?.ShowFinalResult(totalScore);
                    else
                    {
                        manager?.SpawnNextNPC();
                        Destroy(gameObject);
                    }
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
            dialogueText.text = "NO MORE QUESTIONS LEFT!";
            AdjustDialogueBoxSize();
            yield return new WaitForSeconds(2f);
            interacted = true;
            isInteracting = false;
            yield break;
        }

        int i = Random.Range(0, questions.Count);
        currentQuestion = questions[i].question;
        currentAnswer = questions[i].answer;
        currentSolution = questions[i].solution;
        string[] choices = questions[i].choices;
        questions.RemoveAt(i);

        dialogueCanvas.SetActive(true);
        dialogueText.text = currentQuestion;
        AdjustDialogueBoxSize();

        yield return new WaitForSeconds(4f);

        if (topScreenPosition != null)
        {
            dialogueCanvas.transform.position = topScreenPosition.position;
            dialogueCanvas.transform.rotation = topScreenPosition.rotation;
        }

        choicePanel.SetActive(true);
        SetupChoices(choices);

        waitingForAnswer = true;
        answerTimerCoroutine = StartCoroutine(AnswerTimer());
        yield return new WaitUntil(() => !waitingForAnswer);

        dialogueCanvas.SetActive(false);
        timerText.gameObject.SetActive(false);
        interacted = true;
        isInteracting = false;
    }

    void SetupChoices(string[] choices)
    {
        List<string> shuffled = new List<string>(choices);
        for (int i = 0; i < shuffled.Count; i++)
        {
            string temp = shuffled[i];
            int r = Random.Range(i, shuffled.Count);
            shuffled[i] = shuffled[r];
            shuffled[r] = temp;
        }

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < shuffled.Count)
            {
                string ans = shuffled[i].ToUpper();
                choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = ans;
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => SubmitChoice(ans));
            }
        }
    }

    public void SubmitChoice(string selectedAnswer)
    {
        if (!waitingForAnswer) return;
        StopCoroutine(answerTimerCoroutine);

        string result;
        if (selectedAnswer.Equals(currentAnswer.ToUpper()))
        {
            result = $"CORRECT! THE ANSWER IS {currentAnswer.ToUpper()}. {currentSolution}";
            PlayCorrectSound();
            totalScore += 2;
            StartCoroutine(ShowCashAnimation());
        }
        else
        {
            result = $"WRONG! THE CORRECT ANSWER IS {currentAnswer.ToUpper()}. {currentSolution}";
            PlayWrongSound();
            totalScore -= 1;
        }

        if (totalScore < 0) totalScore = 0;

        UpdateScoreDisplay();
        choicePanel.SetActive(false);
        StartCoroutine(ShowResultAndReturn(result));
    }

    void UpdateScoreDisplay()
    {
        if (scoreDisplay != null)
            scoreDisplay.text = "SCORE: " + totalScore.ToString();
    }

    IEnumerator ShowCashAnimation()
    {
        if (cashAnimation != null)
        {
            cashAnimation.SetActive(true);
            Transform obj = cashAnimation.transform;
            Vector3 start = new Vector3(0.8f, 0.8f, 1f);
            Vector3 end = Vector3.one;
            float t = 0f;

            while (t < animationDuration)
            {
                t += Time.deltaTime * 3f;
                float step = Mathf.Round(t * 5f) / 5f;
                obj.localScale = Vector3.Lerp(start, end, step);
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);
            cashAnimation.SetActive(false);
        }
    }

    IEnumerator ShowResultAndReturn(string result)
    {
        dialogueCanvas.transform.position = defaultDialoguePosition.position;
        dialogueCanvas.transform.rotation = defaultDialoguePosition.rotation;
        dialogueText.text = result;
        AdjustDialogueBoxSize();
        dialogueCanvas.SetActive(true);
        timerText.gameObject.SetActive(false);

        yield return new WaitForSeconds(4f);
        waitingForAnswer = false;
    }

    IEnumerator AnswerTimer()
    {
        float timer = 10f;
        timerText.gameObject.SetActive(true);

        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            timerText.text = Mathf.CeilToInt(timer) + "s";
            yield return null;
            if (!waitingForAnswer) yield break;
        }

        choicePanel.SetActive(false);
        dialogueCanvas.transform.position = defaultDialoguePosition.position;
        dialogueText.text = $"TIME’S UP!!";
        AdjustDialogueBoxSize();
        timerText.gameObject.SetActive(false);

        totalScore = Mathf.Max(0, totalScore - 1);
        UpdateScoreDisplay();

        yield return new WaitForSeconds(3f);
        waitingForAnswer = false;
    }

    public void PlayCorrectSound() => correctAnswerAudio?.Play();
    public void PlayWrongSound() => wrongAnswerAudio?.Play();

    void AdjustDialogueBoxSize()
    {
        if (dialogueText == null || dialogueCanvas == null) return;

        dialogueText.ForceMeshUpdate();
        RectTransform canvasRect = dialogueCanvas.GetComponent<RectTransform>();

        Vector2 preferred = dialogueText.GetPreferredValues(dialogueText.text, 600f, Mathf.Infinity);
        float padding = 40f;
        float maxWidth = 650f;
        float newWidth = Mathf.Min(preferred.x + padding, maxWidth);
        float newHeight = preferred.y + padding;

        canvasRect.sizeDelta = new Vector2(newWidth, newHeight);
    }

    public static void ResetGameData()
    {
        npcCount = 0;
        totalScore = 0;
        questions = new List<(string, string, string[], string)>(defaultQuestions);
    }
}
