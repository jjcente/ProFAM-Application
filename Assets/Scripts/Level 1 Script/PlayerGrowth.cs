using UnityEngine;
using TMPro;

public class PlayerGrowth : MonoBehaviour
{
    public float currentWeight = 1f;      // starting weight
    public float growthPerFish = 1f;      // weight gained per correct answer
    public float shrinkPerWrong = 0.5f;   // weight lost per wrong answer
    public float maxWeight = 10f;         // target weight

    private Vector3 baseScale; // original visual scale of the player

    [Header("UI")]
    public TMP_Text weightText;  

    void Start()
    {
        baseScale = transform.localScale;
        UpdateScale();
        UpdateWeightUI();
    }

    public void Grow()
    {
        FishAudioManager.Instance.PlayPlayerGrow();
        currentWeight += growthPerFish;
        if (currentWeight > maxWeight)
            currentWeight = maxWeight;

        Debug.Log($"✅ Grew to {currentWeight}kg");
        UpdateScale();
        UpdateWeightUI();
    }

    public void Shrink()
    {
            if (Mathf.Approximately(currentWeight, maxWeight) && GameManager.Instance != null)
    {
        // Check if player has already triggered win
        // If so, do nothing
        return;
    }
        FishAudioManager.Instance.PlayPlayerGrow();
        currentWeight -= shrinkPerWrong;
        if (currentWeight < 1f)
            currentWeight = 1f;

        Debug.Log($"⚠️ Shrunk to {currentWeight}kg");
        UpdateScale();
        UpdateWeightUI();
    }

   private void UpdateScale()
{
    // Keep current facing direction while changing size
    float direction = Mathf.Sign(transform.localScale.x);
    float safeWeight = Mathf.Max(currentWeight, 1f); // never scale below 1
    float factor = Mathf.Sqrt(safeWeight);
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
        FishAudioManager.Instance.PlayPlayerGrow();
        currentWeight += amount;
        if (currentWeight > maxWeight)
            currentWeight = maxWeight;

        UpdateScale();
        UpdateWeightUI();
        Debug.Log($"Player gained {amount}kg → total {currentWeight}kg");

        GameManager.Instance.CheckWinCondition(currentWeight);

    }

   private void UpdateWeightUI()
{
    if (weightText != null)
    {
        float displayWeight = Mathf.Max(currentWeight, 1f);
        weightText.text = $"Weight: {displayWeight:0.0} kg";
    }
}

}
