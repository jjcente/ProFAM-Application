using UnityEngine;

public class QuizFish : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2f;
    private Vector2 moveDirection;
    private float directionChangeInterval = 2f;
    private float directionTimer;

    private Camera mainCam;
    private Vector2 screenMin;
    private Vector2 screenMax;

    [Header("Quiz Settings")]
    public int quizNumber = 1; // assign in Inspector
    public QuizManager quizManager;

    private bool hasTriggered = false;

    void Start()
    {
        mainCam = Camera.main;
        UpdateScreenBounds();

        if (quizManager == null)
            quizManager = FindFirstObjectByType<QuizManager>();

        if (quizManager == null)
            Debug.LogWarning("No QuizManager found in scene!");

        ChangeDirection();
    }

    void Update()
    {
        MoveRandomly();
    }

    void MoveRandomly()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);

        // Flip sprite based on direction
        if (moveDirection.x > 0 && transform.localScale.x < 0)
            Flip();
        else if (moveDirection.x < 0 && transform.localScale.x > 0)
            Flip();

        // Keep within screen bounds
        Vector3 pos = transform.position;

        if (pos.x < screenMin.x)
        {
            pos.x = screenMin.x;
            moveDirection.x = Mathf.Abs(moveDirection.x);
        }
        else if (pos.x > screenMax.x)
        {
            pos.x = screenMax.x;
            moveDirection.x = -Mathf.Abs(moveDirection.x);
        }

        if (pos.y < screenMin.y)
        {
            pos.y = screenMin.y;
            moveDirection.y = Mathf.Abs(moveDirection.y);
        }
        else if (pos.y > screenMax.y)
        {
            pos.y = screenMax.y;
            moveDirection.y = -Mathf.Abs(moveDirection.y);
        }

        transform.position = pos;

        // periodically change direction
        directionTimer -= Time.deltaTime;
        if (directionTimer <= 0f)
            ChangeDirection();
    }

    void ChangeDirection()
    {
        moveDirection = Random.insideUnitCircle.normalized;
        directionTimer = directionChangeInterval;
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void UpdateScreenBounds()
    {
        Vector3 bottomLeft = mainCam.ScreenToWorldPoint(Vector3.zero);
        Vector3 topRight = mainCam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        screenMin = new Vector2(bottomLeft.x, bottomLeft.y);
        screenMax = new Vector2(topRight.x, topRight.y);
    }

private void OnTriggerEnter2D(Collider2D other)
{
    if (!hasTriggered && other.CompareTag("Player"))
    {
        // Check distance to avoid accidental triggers from far away
        float maxTriggerDistance = 2f; // adjust as needed
        if (Vector2.Distance(transform.position, other.transform.position) > maxTriggerDistance)
            return;

        hasTriggered = true;

        // Get the UserFish component from the player
        UserFish player = other.GetComponent<UserFish>();
        if (player == null)
        {
            Debug.LogWarning("Player does not have a UserFish component!");
            return;
        }

        // Set the playerFish reference in QuizManager
        if (quizManager != null)
        {
            quizManager.playerFish = player;

            // Trigger the quiz
            quizManager.TriggerQuiz(gameObject, quizNumber);

            // Only pause time if the quiz panel actually shows up
            if (quizManager.quizPanel != null && !quizManager.quizPanel.activeSelf)
                Time.timeScale = 1f; // keep normal time until quiz shows
            else
                Time.timeScale = 0f;
        }
        else
        {
            Debug.LogWarning("QuizManager not assigned in QuizFish!");
        }

        Debug.Log($"🐠 QuizFish #{quizNumber} triggered quiz for PlayerFish.");
    }
}
}

