using UnityEngine;

public class UserFish : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;             // Movement speed
    public QuizManager quizManager;      // Reference to QuizManager for quiz popups

    private Camera mainCam;
    private Vector2 screenMin;
    private Vector2 screenMax;
    private bool canTrigger = false;      // Grace period to prevent early collisions

    private Vector2 moveDirection = Vector2.zero; // Movement direction from arrow buttons
    private Rigidbody2D rb;               // Rigidbody2D reference

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            Debug.LogError("Rigidbody2D missing on UserFish! Add one to detect collisions properly.");

        mainCam = Camera.main;

        if (mainCam == null)
            Debug.LogError("Main Camera not found! Assign the MainCamera tag to your camera.");

        UpdateScreenBounds();

        if (quizManager == null)
            quizManager = FindFirstObjectByType<QuizManager>();

        Invoke(nameof(EnableTrigger), 0.5f);
    }

    void EnableTrigger() => canTrigger = true;

    void Update()
    {
        UpdateScreenBounds();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        if (rb == null) return;

        Vector2 newPos = rb.position + moveDirection * speed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);

        // Flip sprite horizontally
        Vector3 scale = transform.localScale;
        if (moveDirection.x > 0) scale.x = Mathf.Abs(scale.x);
        else if (moveDirection.x < 0) scale.x = -Mathf.Abs(scale.x);
        transform.localScale = scale;

        // Clamp within bounds
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, screenMin.x, screenMax.x);
        pos.y = Mathf.Clamp(pos.y, screenMin.y, screenMax.y);
        transform.position = pos;
    }

    void UpdateScreenBounds()
    {
        if (mainCam == null)
        {
            mainCam = Camera.main;
            if (mainCam == null)
                return;
        }

        if (mainCam.pixelWidth <= 1 || mainCam.pixelHeight <= 1)
            return;

        float zDistance = Mathf.Abs(transform.position.z - mainCam.transform.position.z);
        Vector3 bottomLeft = mainCam.ScreenToWorldPoint(new Vector3(0, 0, zDistance));
        Vector3 topRight = mainCam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, zDistance));

        screenMin = new Vector2(bottomLeft.x, bottomLeft.y);
        screenMax = new Vector2(topRight.x, topRight.y);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!canTrigger) return;

        if (other.CompareTag("QuizFish"))
        {
            Debug.Log("✅ User collided with QuizFish — showing quiz!");
            Time.timeScale = 0f;

            if (quizManager != null)
                quizManager.ShowQuiz(other.gameObject);
            else
                Debug.LogWarning("QuizManager not assigned in UserFish!");
        }
    }

    // Movement control
    public void MoveUp()    { moveDirection = Vector2.up; Debug.Log("MoveUp() pressed"); }
    public void MoveDown()  { moveDirection = Vector2.down; Debug.Log("MoveDown() pressed"); }
    public void MoveLeft()  { moveDirection = Vector2.left; Debug.Log("MoveLeft() pressed"); }
    public void MoveRight() { moveDirection = Vector2.right; Debug.Log("MoveRight() pressed"); }
    public void StopMove()  { moveDirection = Vector2.zero; Debug.Log("StopMove() pressed"); }
}

