using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float localFuse = 999f; // not used for shared timer - kept for animation timing
    public GameObject explosionPrefab; 
    public Sprite defusedSprite;

    public bool isDefused { get; private set; } = false;
    public LayerMask wallLayerMask;
    public AudioClip explodeClip;
    public AudioClip defuseClip;


    SpriteRenderer sr;
    Collider2D col;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

public void Defuse()
    {
    AudioManager.Instance.PlaySFX(defuseClip);
    if (isDefused) return;
    isDefused = true;

    var anim = GetComponent<Animation>();
    if (anim) anim.Stop();

    // Optional: make it look "inactive" (e.g. darken it)
    var sr = GetComponent<SpriteRenderer>();
    if (sr) sr.color = Color.gray;

    // Notify manager
    BombManager.Instance?.OnBombDefused(this);

}


public void Explode()
{
    if (isDefused) return;

    int explosionRange = 5; // how far the explosion travels in tiles
    Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    foreach (var dir in directions)
    {
        for (int i = 0; i < explosionRange; i++)
        {
            Vector3 spawnPos = transform.position + (Vector3)(dir * (i + 1));

            // Check if a wall blocks the explosion *before* spawning
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, i + 1, wallLayerMask);
            if (hit.collider != null)
            {
                // Stop if wall is in the way
                break;
            }

            // Spawn explosion piece
            GameObject go = Instantiate(explosionPrefab, spawnPos, Quaternion.identity);
            Explosion e = go.GetComponent<Explosion>();
            if (e != null)
            {
                // Middle vs End piece
                if (i < explosionRange - 1)
                    e.SetActiveRenderer(e.middle);
                else
                    e.SetActiveRenderer(e.end);

                e.SetDirection(dir);
                e.Play();
                AudioManager.Instance.PlaySFX(explodeClip);
            }
        }
    }

    // Spawn center explosion
    GameObject center = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
    Explosion centerExplosion = center.GetComponent<Explosion>();
    if (centerExplosion != null)
    {
        centerExplosion.SetActiveRenderer(centerExplosion.start);
        centerExplosion.Play();
    }

    if (col != null) col.enabled = false;

    BombManager.Instance?.OnBombExploded(this);
    Destroy(gameObject);
}

}
