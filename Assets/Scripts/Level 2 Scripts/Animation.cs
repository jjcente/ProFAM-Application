using UnityEngine;

public class Animation : MonoBehaviour
{
    private SpriteRenderer spriterender;

    public Sprite idleSprite;
    public Sprite[] animationSprites;
    public float animationTime = 0.25f;
    private int animationFrame;
    public bool loop = true;
    public bool idle = true;
    private void Awake()
    {
        spriterender = GetComponent<SpriteRenderer>();

    }

    private void OnEnable()
    {
        spriterender.enabled = true;
    }

    private void OnDisable()
    {
        spriterender.enabled = false;
    }

    private void Start()
    {
        InvokeRepeating(nameof(NextFrame), animationTime, animationTime);
    }
    
    private void NextFrame()
    {
        animationFrame++;
        if (loop && animationFrame >= animationSprites.Length)
        {
            animationFrame = 0;

        }

        if (idle)
        {
            spriterender.sprite = idleSprite;

        } else if (animationFrame >= 0 && animationFrame < animationSprites.Length)
        {
            spriterender.sprite = animationSprites[animationFrame];
        }
    }

}
