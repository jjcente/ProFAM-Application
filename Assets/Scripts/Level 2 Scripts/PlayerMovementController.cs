using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
   public Rigidbody2D rigid { get; private set; }
    private Vector2 direction = Vector2.down;
    public float speed = 5f;

    public KeyCode inputUp = KeyCode.UpArrow;
    public KeyCode inputDown = KeyCode.DownArrow;
    public KeyCode inputLeft = KeyCode.LeftArrow;
    public KeyCode inputRight = KeyCode.RightArrow;

    public Animation spriteRendererUp;
    public Animation spriteRendererDown;
    public Animation spriteRendererLeft;
    public Animation spriteRendererRight;
    private Animation activeSprite;
       

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        activeSprite = spriteRendererDown;
    }

    private void Update()
    {
        if (Input.GetKey(inputUp))
        {
            Debug.Log("Up pressed");
            SetDirection(Vector2.up, spriteRendererUp);
        }
        else if (Input.GetKey(inputDown))
        {
            Debug.Log("Down pressed");
            SetDirection(Vector2.down, spriteRendererDown);
        }
        else if (Input.GetKey(inputLeft))
        {
            Debug.Log("Left pressed");
            SetDirection(Vector2.left, spriteRendererLeft);
        }
        else if (Input.GetKey(inputRight))
        {
            Debug.Log("Right pressed");
            SetDirection(Vector2.right, spriteRendererRight);
        }
        else
        {
            SetDirection(Vector2.zero, activeSprite);
        }
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

}
