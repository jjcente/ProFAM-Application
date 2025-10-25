using UnityEngine;

public class QuizFish : MonoBehaviour
{
    public float speed = 2f;                // Movement speed
    public QuizManager quizManager;         // Reference to QuizManager (assign in Inspector)
    private bool hasTriggered = false;      // Prevents multiple quiz triggers
    private Camera mainCam;
    private Vector2 screenMin;
    private Vector2 screenMax;

    void Start()
    {
        mainCam = Camera.main;
        UpdateScreenBounds();

        // Try to find QuizManager automatically if not assigned
        if (quizManager == null)
            quizManager = FindFirstObjectByType<QuizManager>();
    }

    void Update()
    {
        // Move left continuously
        transform.Translate(Vector2.left * speed * Time.deltaTime, Space.World);

        // Flip sprite so it faces left (if needed)
        if (transform.localScale.x > 0)
            Flip();

        // Keep fish inside screen bounds (optional)
        Vector3 pos = transform.position;
        if (pos.x < screenMin.x)
        {
            pos.x = screenMax.x; // Wrap to the right side (loop)
        }
        transform.position = pos;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            Debug.Log("QuizFish collided with Player — triggering quiz!");

            Time.timeScale = 0f;

            if (quizManager != null)
                quizManager.ShowQuiz();
            else
                Debug.LogWarning("QuizManager not assigned in QuizFish!");

            Destroy(gameObject, 0.3f);
        }
    }

    void UpdateScreenBounds()
    {
        Vector3 bottomLeft = mainCam.ScreenToWorldPoint(Vector3.zero);
        Vector3 topRight = mainCam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        screenMin = new Vector2(bottomLeft.x, bottomLeft.y);
        screenMax = new Vector2(topRight.x, topRight.y);
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}

