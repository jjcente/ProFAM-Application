using UnityEngine;

public class CameraMoving : MonoBehaviour
{
    public Transform target;
    public float delayBeforePan = 2f;   
        public float panSpeed = 2f;          
    public float followSmooth = 5f;      
    public Vector3 offset = new Vector3(0, 0, -10);

    private bool isFollowing = false;
    private float timer = 0f;
    public float zoom = 5f;
    private Camera cam;


    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam != null)
            cam.orthographicSize = zoom;
    }

    void LateUpdate()
    {
        if (target == null) return;

        timer += Time.deltaTime;

        if (timer < delayBeforePan) return;

        if (!isFollowing)
        {
            Vector3 targetPos = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPos, panSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.1f)
                isFollowing = true;

            return;
        }

        Vector3 desiredPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, followSmooth * Time.deltaTime);
    }
}
