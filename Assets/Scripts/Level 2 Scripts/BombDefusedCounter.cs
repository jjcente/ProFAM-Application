using UnityEngine;
using TMPro;

public class BombDefusedCounter : MonoBehaviour
{
    [Header("UI Reference")]
    public TMP_Text defusedCounterText;

    private int totalBombs;
    private int defusedBombs;

    /// <summary>
    /// Initialize the counter at level start.
    /// </summary>
    public void Initialize(int total)
    {
        totalBombs = total;
        defusedBombs = 0;
        UpdateUI();
    }

    /// <summary>
    /// Called whenever a bomb is successfully defused.
    /// </summary>
    public void OnBombDefused(int defused)
    {
        defusedBombs = defused;
        UpdateUI();
    }

    /// <summary>
    /// Updates the text display (e.g., "Defused: 3/5")
    /// </summary>
    private void UpdateUI()
    {
        if (defusedCounterText != null)
        {
            defusedCounterText.text = $"Defused: {defusedBombs}/{totalBombs}";
        }
    }
}
