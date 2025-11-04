using UnityEngine;

public class PlayerGrowth : MonoBehaviour
{
    public float currentWeight = 1f;      // starting weight
    public float growthPerFish = 1f;      // weight gained per correct answer
    public float shrinkPerWrong = 0.5f;   // weight lost per wrong answer
    public float maxWeight = 10f;         // target weight

    private Vector3 baseScale; // original visual scale of the player

    void Start()
    {
        baseScale = transform.localScale;
    }

    public void Grow()
    {
        currentWeight += growthPerFish;
        if (currentWeight > maxWeight)
            currentWeight = maxWeight;

        Debug.Log($"✅ Grew to {currentWeight}kg");
        UpdateScale();
    }

    public void Shrink()
    {
        currentWeight -= shrinkPerWrong;
        if (currentWeight < 1f)
            currentWeight = 1f;

        Debug.Log($"⚠️ Shrunk to {currentWeight}kg");
        UpdateScale();
    }

    private void UpdateScale()
    {
        // Keep current facing direction while changing size
        float direction = Mathf.Sign(transform.localScale.x);
        float factor = Mathf.Sqrt(currentWeight);
        transform.localScale = new Vector3(
            baseScale.x * factor * direction,
            baseScale.y * factor,
            baseScale.z * factor
        );
    }

    // This helper lets other scripts flip direction safely
    public void FlipDirection(float dir)
    {
        transform.localScale = new Vector3(
            Mathf.Abs(transform.localScale.x) * Mathf.Sign(dir),
            transform.localScale.y,
            transform.localScale.z
        );
    }

    public void GrowBy(float amount)
    {
        currentWeight += amount;
        if (currentWeight > maxWeight)
            currentWeight = maxWeight;

        UpdateScale();
        Debug.Log($"Player gained {amount}kg → total {currentWeight}kg");
    }

}
