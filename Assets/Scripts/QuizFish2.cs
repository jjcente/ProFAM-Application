using UnityEngine;

public class QuizFish2 : MonoBehaviour
{
    public float speed = 2f;
    public QuizManager quizManager;
    private bool hasTriggered = false;
    private Camera mainCam;
    private Vector2 screenMin;
    private Vector2 screenMax;

    private Vector2 moveDirection; // new: stores random movement direction
    private float directionChangeInterval = 2f; // time before changing direction
    private float directionTimer;

    void Start()
    {
        mainCam = Camera.main;
        UpdateScreenBounds();

        if (quizManager == null)
            quizManager = FindFirstObjectByType<QuizManager>();

        // initialize with a random direction
        ChangeDirection();
    }

    void Update()
    {
        MoveRandomly();
    }

    void MoveRandomly()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);

        // Flip based on x direction
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            Debug.Log("QuizFish collided with Player — triggering quiz!");

            Time.timeScale = 0f;

            if (quizManager != null)
                quizManager.ShowQuiz(gameObject);
            else
                Debug.LogWarning("QuizManager not assigned in QuizFish!");
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


