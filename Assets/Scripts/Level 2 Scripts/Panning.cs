using UnityEngine;

public class Panning : MonoBehaviour
{
     public Transform target;             // Your player
    public float delayBeforePan = 2f;    // Time before camera starts panning
    public float panSpeed = 2f;          // How fast the camera pans to the player
    public float followSmooth = 5f;      // How smooth the follow is
    public Vector3 offset = new Vector3(0, 0, -10);

    private bool isFollowing = false;
    private float timer = 0f;

    void Start()
    {
        // Optional: start at a fixed position (like an intro spot)
        // transform.position = new Vector3(0, 0, -10);
    }

    void LateUpdate()
    {
        if (target == null) return;

        timer += Time.deltaTime;

        // Step 1: Wait for delay
        if (timer < delayBeforePan) return;

        // Step 2: Pan toward the player
        if (!isFollowing)
        {
            Vector3 targetPos = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPos, panSpeed * Time.deltaTime);

            // Once close enough, switch to follow mode
            if (Vector3.Distance(transform.position, targetPos) < 0.1f)
                isFollowing = true;

            return;
        }

        // Step 3: Follow the player
        Vector3 desiredPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, followSmooth * Time.deltaTime);
    }
}
