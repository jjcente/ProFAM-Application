using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float localFuse = 999f; // not used for shared timer - kept for animation timing
    public Sprite explosionSprite;
    public GameObject defusedVFX; // optional prefab to instantiate
    public bool isDefused { get; private set; } = false;

    SpriteRenderer sr;
    Collider2D col;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    // Called by QuestionManager if player answers correctly
    public void Defuse()
    {
        if (isDefused) return;
        isDefused = true;
        // stop ticking animation if any
        var anim = GetComponent<Animator>();
        if (anim) anim.enabled = false;
        if (defusedVFX) Instantiate(defusedVFX, transform.position, Quaternion.identity);

        // visually mark defused (could change sprite)
        if (explosionSprite != null) sr.sprite = explosionSprite; // use a "defused" sprite or same sprite
        // disable collider so player can walk through
        if (col != null) col.enabled = false;

        // notify manager
        BombManager.Instance?.OnBombDefused(this);

        // optional: destroy after short delay
        Destroy(gameObject, 0.8f);
    }

    // Called when time runs out or explosion logic needed
    public void Explode()
    {
        if (isDefused) return;
        // TODO: explosion effect, damage, etc.
        Destroy(gameObject);
    }
}
