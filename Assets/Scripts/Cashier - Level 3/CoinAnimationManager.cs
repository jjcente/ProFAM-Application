using UnityEngine;

public class CoinAnimationManager : MonoBehaviour
{
    public GameObject coinAnimation;   // Assign your Cash_Anim prefab or GameObject here
    public float displayTime = 2f;     // Duration before hiding again

    void Start()
    {
        if (coinAnimation != null)
            coinAnimation.SetActive(false); // Hide at start
    }

    // ✅ Call this method when the player answers correctly
    public void ShowCoin()
    {
        if (coinAnimation == null) return;

        coinAnimation.SetActive(true);

        // Restart animation from beginning if it’s looping
        Animator animator = coinAnimation.GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play(0);
        }

        // Hide after displayTime seconds
        CancelInvoke(nameof(HideCoin));
        Invoke(nameof(HideCoin), displayTime);
    }

    private void HideCoin()
    {
        if (coinAnimation != null)
            coinAnimation.SetActive(false);
    }
}
