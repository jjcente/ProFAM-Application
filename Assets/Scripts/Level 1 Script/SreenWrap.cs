using UnityEngine;

public class ScreenWrap : MonoBehaviour
{
    private float left, right, top, bottom;
    public float offset = 0.5f; // optional

    void Start()
    {
        Camera cam = Camera.main;
        float vertExtent = cam.orthographicSize;
        float horzExtent = vertExtent * cam.aspect;

        left = cam.transform.position.x - horzExtent;
        right = cam.transform.position.x + horzExtent;
        bottom = cam.transform.position.y - vertExtent;
        top = cam.transform.position.y + vertExtent;
    }

    void Update()
    {
        Vector3 pos = transform.position;

        if (pos.x < left) pos.x = right - offset;
        else if (pos.x > right) pos.x = left + offset;

        if (pos.y < bottom) pos.y = top - offset;
        else if (pos.y > top) pos.y = bottom + offset;

        transform.position = pos;
    }
}
