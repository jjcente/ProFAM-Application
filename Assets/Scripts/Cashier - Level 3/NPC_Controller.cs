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
    // ✅ Expanded, dialogue-friendly default question set
    // ✅ Balanced-length default question set (short but complete)
    private static readonly List<(string question, string answer, string[] choices, string solution)> defaultQuestions =
        new List<(string, string, string[], string)>
    {
        ("A household receives a ₱1,500 water bill. They get a 20% senior discount but only on the first ₱1,000. What is their final payment?",
        "₱1,300", new string[]{"₱1,200","₱1,300","₱1,350"},
        "SOLUTION: 20% of ₱1,000=₱200, ₱1,500−₱200=₱1,300."),

        ("A coffee shop uses 2.5 kg of beans daily. Each bag has 12 kg. How many bags are needed for 14 days?",
        "3 BAGS", new string[]{"3 BAGS","4 BAGS","5 BAGS"},
        "SOLUTION: 2.5×14=35 kg, 35÷12≈2.9 = 3 bags."),

        ("A school replaces 85 bulbs. A box of 12 costs ₱585, while one bulb is ₱150. What is the lowest total cost?",
        "₱4,245.00", new string[]{"₱4,245.00","₱4,950.00","₱5,010.00"},
        "SOLUTION: 7 boxes (₱585×7=₱4,095) + 1 bulb (₱150)=₱4,245."),

        ("2.5 liters of paint cover 20 sqm. How many liters are needed for 70 sqm?",
        "8.75 LITERS", new string[]{"7.5 LITERS","8.75 LITERS","10.0 LITERS"},
        "SOLUTION: 2.5 ÷ 20=0.125 per sqm, 0.125×70=8.75L."),

        ("A traveler buys a $150 souvenir. If ₱55 equals $1, how much will it cost in pesos?",
        "₱8,250.00", new string[]{"₱8,400.00","₱8,250.00","₱7,950.00"},
        "SOLUTION: 150×55=₱8,250."),

        ("A baker uses 200g of butter for 24 cookies. How much is needed for 60 cookies?",
        "500 G", new string[]{"400 G","500 G","600 G"},
        "SOLUTION: (60÷24)=2.5×200=500g."),

        ("A van travels 360 km using 45L of fuel. With 10L left, how far can it go?",
        "80 KM", new string[]{"80 KM","90 KM","100 KM"},
        "SOLUTION: 360÷45=8 km/L, 8×10=80 km."),

        ("A box of 10 pens costs ₱80, but a single pen costs ₱10. How much is saved per pen when buying the box?",
        "₱2.00", new string[]{"₱1.00","₱1.50","₱2.00"},
        "SOLUTION: (10×₱10)=₱100, ₱100−₱80=₱20 saved = ₱2/pen."),

        ("A 500 km trip uses 10L per 100 km. If fuel costs ₱50/L, what is the total cost?",
        "₱2,500.00", new string[]{"₱2,000.00","₱2,500.00","₱3,000.00"},
        "SOLUTION: 500÷100=5×10L=50L, 50×₱50=₱2,500."),

        ("A machine makes 150 items in 20 minutes. How many items can it make in 3 hours?",
        "1,350 ITEMS", new string[]{"900 ITEMS","1,125 ITEMS","1,350 ITEMS"},
        "SOLUTION: 3h=180min, 180÷20=9×150=1,350 items.")
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
            result = $"CORRECT! THE ANSWER IS {currentAnswer.ToUpper()}. \n{currentSolution}";
            PlayCorrectSound();
            totalScore += 2;
            StartCoroutine(ShowCashAnimation());
        }
        else
        {
            result = $"WRONG! THE CORRECT ANSWER IS {currentAnswer.ToUpper()}. \n{currentSolution}";
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

    // Modified to wait for player interaction (tap/click/key) instead of auto 4s
    IEnumerator ShowResultAndReturn(string result)
    {
        dialogueCanvas.transform.position = defaultDialoguePosition.position;
        dialogueCanvas.transform.rotation = defaultDialoguePosition.rotation;
        dialogueText.text = result;
        dialogueCanvas.SetActive(true);
        timerText.gameObject.SetActive(false);

        // Small delay so the tap used for answering doesn’t close immediately
        yield return new WaitForSeconds(0.3f);

        // Wait until the player taps/clicks or presses any key
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0) || Input.touchCount > 0 || Input.anyKeyDown);

        // After input, allow the Interact coroutine to continue
        waitingForAnswer = false;
    }

    IEnumerator AnswerTimer()
    {
        float timer = 15f;
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
        timerText.gameObject.SetActive(false);

        totalScore = Mathf.Max(0, totalScore - 1);
        UpdateScoreDisplay();

        yield return new WaitForSeconds(3f);
        waitingForAnswer = false;
    }

    public void PlayCorrectSound() => correctAnswerAudio?.Play();
    public void PlayWrongSound() => wrongAnswerAudio?.Play();

    public static void ResetGameData()
    {
        npcCount = 0;
        totalScore = 0;
        questions = new List<(string, string, string[], string)>(defaultQuestions);
    }
}
