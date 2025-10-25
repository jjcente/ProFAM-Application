using UnityEngine;

public class BackgroundFish : MonoBehaviour
{
    public float speed = 1f;      // movement speed
    public float moveRange = 0.5f;  // how far left/right it can go

    private float startX;         // starting x-position
    private int direction = -1;   // start moving left (-1)

    void Start()
    {
        startX = transform.position.x;
    }

    void Update()
    {
        // Move the fish
        float newX = transform.position.x + speed * direction * Time.deltaTime;

        // Check boundaries
        if (newX > startX + moveRange)
        {
            newX = startX + moveRange;
            direction = -1;
            Flip();
        }
        else if (newX < startX - moveRange)
        {
            newX = startX - moveRange;
            direction = 1;
            Flip();
        }

        // Apply the new position
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}

