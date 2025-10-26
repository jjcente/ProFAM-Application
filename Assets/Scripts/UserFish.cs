using UnityEngine;

public class UserFish : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;             // Movement speed
    public QuizManager quizManager;       // Reference to QuizManager for quiz popups

    private Camera mainCam;
    private Vector2 screenMin;
    private Vector2 screenMax;
    private bool canTrigger = false;      // Grace period to prevent early collisions

    private Vector2 moveDirection = Vector2.zero; // Movement direction from arrow buttons

    void Start()
    {
        mainCam = Camera.main;

        if (mainCam == null)
            Debug.LogError("Main Camera not found! Assign the MainCamera tag to your camera.");

        UpdateScreenBounds();

        // Find QuizManager automatically if not assigned
        if (quizManager == null)
            quizManager = FindFirstObjectByType<QuizManager>();

        // Enable collisions after a short grace period
        Invoke(nameof(EnableTrigger), 0.5f);
    }

    void EnableTrigger()
    {
        canTrigger = true;
    }

    void Update()
    {
        UpdateScreenBounds();
        MovePlayer();
    }

    void MovePlayer()
    {
        // Apply movement based on button input
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);

        // Flip sprite horizontally based on horizontal movement
        Vector3 scale = transform.localScale;
        if (moveDirection.x > 0) scale.x = Mathf.Abs(scale.x);      // Face right
        else if (moveDirection.x < 0) scale.x = -Mathf.Abs(scale.x); // Face left
        transform.localScale = scale;

        // Clamp within screen bounds
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
                return; // no camera found yet
        }

        // Skip if the camera isn't rendering properly (Device Simulator does this briefly)
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
            Debug.Log("User collided with QuizFish — showing quiz!");
            Time.timeScale = 0f;

            if (quizManager != null)
                quizManager.ShowQuiz(other.gameObject); // Pass the QuizFish reference
            else
                Debug.LogWarning("QuizManager not assigned in UserFish!");
        }
    }

    public void MoveUp()    
    { 
        moveDirection = Vector2.up; 
        Debug.Log("MoveUp() pressed — moving up"); 
    }

    public void MoveDown()  
    { 
        moveDirection = Vector2.down; 
        Debug.Log("MoveDown() pressed — moving down"); 
    }

    public void MoveLeft()  
    { 
        moveDirection = Vector2.left; 
        Debug.Log("MoveLeft() pressed — moving left"); 
    }

    public void MoveRight() 
    { 
        moveDirection = Vector2.right; 
        Debug.Log("MoveRight() pressed — moving right"); 
    }

    public void StopMove()  
    { 
        moveDirection = Vector2.zero; 
        Debug.Log("StopMove() pressed — stopping movement"); 
    }

}

