using UnityEngine;

public class FishMovement : MonoBehaviour
{
    public float speed = 5f;           // Movement speed
    public float changeDirectionTime = 5f; // How often to pick a new direction

    private Vector2 moveDirection;
    private float timer;

    void Start()
    {
        PickNewDirection();
    }

    void Update()
    {

        if (FishQuestionManager.IsQuestionActive)
        return;
        // Move fish
        transform.Translate(moveDirection * speed * Time.deltaTime);

        // Update timer
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            PickNewDirection();
        }

        
    }

    void PickNewDirection()
    {
        float angle = Random.Range(0f, 360f);
        moveDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        moveDirection.Normalize();

        timer = changeDirectionTime;

        if (moveDirection.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);
    }
}
