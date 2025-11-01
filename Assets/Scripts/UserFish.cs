using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class UserFish : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;                  
    public QuizManager quizManager;           

    private Vector2 moveDirection = Vector2.zero;
    private Camera mainCam;
    private bool facingRight = true; 

    private void Start()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Vector3 movement = new Vector3(moveDirection.x, moveDirection.y, 0f) * speed * Time.deltaTime;
            transform.Translate(movement, Space.World);

            HandleFlip(moveDirection.x); // 👈 Flip when moving horizontally
        }
    }

    // 🐟 Flip sprite when moving left/right
    private void HandleFlip(float horizontal)
    {
        if (horizontal > 0 && !facingRight)
        {
            Flip();
        }
        else if (horizontal < 0 && facingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1; // 🔁 Flip horizontally
        transform.localScale = localScale;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Time.timeScale = 0f;

        float maxTriggerDistance = 2f; // adjust as needed
        if (Vector2.Distance(transform.position, other.transform.position) > maxTriggerDistance)
        return;

        QuizFish quizFish = other.GetComponent<QuizFish>();
        if (quizFish != null)
        {
            Debug.Log($"🐠 User collided with QuizFish #{quizFish.quizNumber}");

            if (quizManager != null)
                quizManager.TriggerQuiz(other.gameObject, quizFish.quizNumber);
            else 
                Time.timeScale = 1f;
        }
    }

    // 🕹️ Button Movement Controls
    public void MoveUp()    
    { 
        moveDirection = Vector2.up; 
        Debug.Log("MoveUp() pressed"); 
    }

    public void MoveDown()  
    { 
        moveDirection = Vector2.down; 
        Debug.Log("MoveDown() pressed"); 
    }

    public void MoveLeft()  
    { 
        moveDirection = Vector2.left; 
        HandleFlip(-1); 
        Debug.Log("MoveLeft() pressed"); 
    }

    public void MoveRight() 
    { 
        moveDirection = Vector2.right; 
        HandleFlip(1); 
        Debug.Log("MoveRight() pressed"); 
    }

    public void StopMove()  
    { 
        moveDirection = Vector2.zero; 
        Debug.Log("StopMove() pressed"); 
    }

    public void Grow(float amount)
    {
        transform.localScale += new Vector3(amount, amount, amount);
    }

    public void Shrink(float amount)
    {
        transform.localScale -= new Vector3(amount, amount, amount);
    }


    // ✅ EventSystem reset (if quiz re-enables input later)
    public void ForceUpdateAfterQuiz()
    {
        Debug.Log("🔄 ForceUpdateAfterQuiz called");
        if (EventSystem.current != null)
        {
            StartCoroutine(EnableEventSystemNextFrame());
        }
    }

    private IEnumerator EnableEventSystemNextFrame()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.enabled = false;
        yield return null;
        EventSystem.current.enabled = true;
        Debug.Log("✅ EventSystem fully refreshed next frame.");
    }
}

