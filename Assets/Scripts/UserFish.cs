using UnityEngine;

public class UserFish : MonoBehaviour
{
    public float speed = 2f;
    public float bottomBuffer = 0.5f;
    public QuizManager quizManager;

    private Camera mainCam;
    private Vector2 screenMin;
    private Vector2 screenMax;

    private bool canTrigger = false; // Grace period to prevent early collision

    void Start()
    {
        mainCam = Camera.main;
        UpdateScreenBounds();

        if (quizManager == null)
            quizManager = FindFirstObjectByType<QuizManager>();

        // Enable triggers after 0.5 seconds
        Invoke(nameof(EnableTrigger), 0.5f);
    }

    void EnableTrigger()
    {
        canTrigger = true;
    }

    void Update()
    {
        UpdateScreenBounds();

        // Move steadily to the right
        transform.Translate(Vector2.right * speed * Time.deltaTime, Space.World);

        // Face right
        Vector3 scale = transform.localScale;
        if (scale.x < 0)
        {
            scale.x *= -1;
            transform.localScale = scale;
        }

        // Wrap around horizontally
        Vector3 pos = transform.position;
        if (pos.x > screenMax.x)
            pos.x = screenMin.x;

        // Keep within vertical bounds
        float adjustedMinY = screenMin.y + bottomBuffer;
        pos.y = Mathf.Clamp(pos.y, adjustedMinY, screenMax.y);
        transform.position = pos;
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
        if (!canTrigger) return;

        if (other.CompareTag("QuizFish"))
        {
            Debug.Log("User collided with QuizFish — showing quiz!");
            Time.timeScale = 0f;

            if (quizManager != null)
                quizManager.ShowQuiz(other.gameObject); // Pass the QuizFish reference
            else
                Debug.LogWarning("QuizManager not assigned in UserFish!");
        }
    }
}

