using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    public Rigidbody2D rigid { get; private set; }
    private Vector2 direction = Vector2.down;
    public float speed = 3f;

    public KeyCode inputUp = KeyCode.UpArrow;
    public KeyCode inputDown = KeyCode.DownArrow;
    public KeyCode inputLeft = KeyCode.LeftArrow;
    public KeyCode inputRight = KeyCode.RightArrow;

    public Animation spriteRendererUp;
    public Animation spriteRendererDown;
    public Animation spriteRendererLeft;
    public Animation spriteRendererRight;
    public Animation spriteRenderDeath;
    private Animation activeSprite;

    private Vector2 uiDirection = Vector2.zero;
    public Transform spawnPoint;


    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        activeSprite = spriteRendererDown;
    }

    private void Update()
    {
        Vector2 finalDirection = uiDirection != Vector2.zero ? uiDirection : GetKeyboardDirection();

        if (finalDirection == Vector2.up) SetDirection(Vector2.up, spriteRendererUp);
        else if (finalDirection == Vector2.down) SetDirection(Vector2.down, spriteRendererDown);
        else if (finalDirection == Vector2.left) SetDirection(Vector2.left, spriteRendererLeft);
        else if (finalDirection == Vector2.right) SetDirection(Vector2.right, spriteRendererRight);
        else SetDirection(Vector2.zero, activeSprite);


    }

    private void FixedUpdate()
    {
        Vector2 position = rigid.position;
        Vector2 translation = direction * speed * Time.fixedDeltaTime;

        rigid.MovePosition(position + translation);
    }

    private void SetDirection(Vector2 newDirection, Animation spriterender)
    {
        direction = newDirection;

        spriteRendererUp.enabled = spriterender == spriteRendererUp;
        spriteRendererDown.enabled = spriterender == spriteRendererDown;
        spriteRendererLeft.enabled = spriterender == spriteRendererLeft;
        spriteRendererRight.enabled = spriterender == spriteRendererRight;

        activeSprite = spriterender;
        activeSprite.idle = direction == Vector2.zero;




    }

    private Vector2 GetKeyboardDirection()
    {
        if (Input.GetKey(inputUp)) return Vector2.up;
        if (Input.GetKey(inputDown)) return Vector2.down;
        if (Input.GetKey(inputLeft)) return Vector2.left;
        if (Input.GetKey(inputRight)) return Vector2.right;
        return Vector2.zero;
    }

    public void MoveUpUI() => uiDirection = Vector2.up;
    public void MoveDownUI() => uiDirection = Vector2.down;
    public void MoveLeftUI() => uiDirection = Vector2.left;
    public void MoveRightUI() => uiDirection = Vector2.right;
    public void StopMoveUI() => uiDirection = Vector2.zero;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Explosion"))
        {
            DeathSequence();
        }
    }
private void DeathSequence()
{
    Debug.Log("DeathSequence triggered");

    enabled = false; // stop movement input
    var col = GetComponent<Collider2D>();
    if (col) col.enabled = false;

    // Disable all other sprites
    spriteRendererUp.enabled = false;
    spriteRendererDown.enabled = false;
    spriteRendererLeft.enabled = false;
    spriteRendererRight.enabled = false;

    // Enable death animation
    if (spriteRenderDeath != null)
    {
        spriteRenderDeath.enabled = true;

        // 🔥 force animation to play immediately
        spriteRenderDeath.idle = false;
        spriteRenderDeath.loop = false;

        // If your Animation script has a Play() method, call it
        // spriteRnderDeath.Play();
    }
    else
    {
        Debug.LogWarning("No death animation assigned to Player!");
    }

    Invoke(nameof(onDeathSequenceEnded), 1.25f);
}

    public void onDeathSequenceEnded()
{
    if (BombManager.Instance != null && BombManager.Instance.IsTimerRunning())
    {
        // Reset bombs and respawn player
        BombManager.Instance.ResetLevelWithoutTimer();
        Respawn();
    }
    else
    {
        gameObject.SetActive(false); // timer expired — end game
    }
}

    
    public void Respawn()
{
    Debug.Log("Respawning player...");

    // Reactivate movement
    enabled = true;

    // Reactivate collider
    var col = GetComponent<Collider2D>();
    if (col) col.enabled = true;

    // Reset position
    if (spawnPoint != null)
        transform.position = spawnPoint.position;

    // Reset animation to idle down
    spriteRenderDeath.enabled = false;
    spriteRendererDown.enabled = true;
    spriteRendererUp.enabled = false;
    spriteRendererLeft.enabled = false;
    spriteRendererRight.enabled = false;

    activeSprite = spriteRendererDown;
    activeSprite.idle = true;
}
}
