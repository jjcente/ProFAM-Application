using UnityEngine;
using System.Collections;


public class Explosion : MonoBehaviour
{
    public Animation start;
    public Animation middle;
    public Animation end;
    public float lifetime = 0.9f;

    public void SetActiveRenderer(Animation renderer)
    {
        start.enabled = renderer == start;
        middle.enabled = renderer == middle;
        end.enabled = renderer == end;
    }

    public void SetDirection(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x);
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public void Play()
    {
        SetActiveRenderer(start);
        StartCoroutine(AutoDestroy());
    }

    private IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
}
