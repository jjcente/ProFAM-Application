using UnityEngine;

public class UserFish : MonoBehaviour
{
    public float speed = 2f;                   // Movement speed
    public float bottomBuffer = 0.5f;          // How far above the bottom edge the fish can go
    public QuizManager quizManager;            // Reference to QuizManager for showing questions

    private Camera mainCam;
    private Vector2 screenMin;
    private Vector2 screenMax;

    void Start()
    {
        mainCam = Camera.main;
        UpdateScreenBounds();
    }

    void Update()
    {
        UpdateScreenBounds();

        // ✅ Move steadily to the right
        transform.Translate(Vector2.right * speed * Time.deltaTime, Space.World);

        // ✅ Flip sprite if needed (face right)
        Vector3 scale = transform.localScale;
        if (scale.x < 0)
        {
            scale.x *= -1;
            transform.localScale = scale;
        }

        // ✅ Keep fish within screen horizontally (loop around)
        Vector3 pos = transform.position;

        if (pos.x > screenMax.x)
        {
            pos.x = screenMin.x; // reappear from the left
        }

        // ✅ Keep it within vertical range
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
        if (other.CompareTag("Fish") || other.CompareTag("Player"))
        {
            Debug.Log("Fish collision detected!");

            Time.timeScale = 0f;

            if (quizManager != null)
                quizManager.ShowQuiz();
            else
                Debug.LogWarning("QuizManager not assigned in the Inspector!");
        }
    }
}

