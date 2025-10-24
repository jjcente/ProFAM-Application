using UnityEngine;

public class ScreenFishMovement : MonoBehaviour
{
    public float speed = 2f;                  // Movement speed
    public float changeDirectionTime = 3f;    // How often to randomize direction
    public float bottomBuffer = 0.5f;           // How far above the bottom edge the fish can go

    private Vector2 direction;
    private float timer;
    private Camera mainCam;
    private Vector2 screenMin;
    private Vector2 screenMax;

    void Start()
    {
        mainCam = Camera.main;
        UpdateScreenBounds();
        PickNewDirection();
    }

    void Update()
    {
        timer += Time.deltaTime;
        UpdateScreenBounds();

        // Move the fish
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        // Flip sprite if needed
        if (direction.x > 0 && transform.localScale.x < 0)
            Flip();
        else if (direction.x < 0 && transform.localScale.x > 0)
            Flip();

        // Check screen boundaries (bounce back)
        Vector3 pos = transform.position;

        if (pos.x < screenMin.x || pos.x > screenMax.x)
        {
            direction.x *= -1;
            Flip();
            pos.x = Mathf.Clamp(pos.x, screenMin.x, screenMax.x);
        }

        // Bottom limit adjusted by buffer
        float adjustedMinY = screenMin.y + bottomBuffer;
        if (pos.y < adjustedMinY || pos.y > screenMax.y)
        {
            direction.y *= -1;
            pos.y = Mathf.Clamp(pos.y, adjustedMinY, screenMax.y);
        }

        transform.position = pos;

        // Occasionally change direction
        if (timer >= changeDirectionTime)
        {
            PickNewDirection();
        }
    }

    void PickNewDirection()
    {
        direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-0.5f, 0.5f)).normalized;
        timer = 0f;
    }

    void UpdateScreenBounds()
    {
        // Convert screen corners to world coordinates
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

