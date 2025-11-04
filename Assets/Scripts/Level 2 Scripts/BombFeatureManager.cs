using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BombFeatureManager : MonoBehaviour
{
    public static BombFeatureManager Instance { get; private set; }

    // ✅ Add this static property
    public static bool IsFeatureActive { get; private set; } = false;

    [Header("UI References")]
    public GameObject featurePanel;
    public TMP_Text featureText;
    public Button closeButton;

    [TextArea]
    public string featureDescription = "Your goal is to defuse all bombs in the level before the timer runs out. Use the arrow keys to move your character and approach each bomb to solve its puzzle. Be careful — if a bomb explodes, you’ll respawn at your spawn point, with defused bombs remaining defused and exploded bombs respawning. Once all bombs are defused, a portal appears, and walking into it takes you to the next level.";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        featurePanel.SetActive(false);

        if (closeButton != null)
            closeButton.onClick.AddListener(HideFeaturePanel);
    }

    public void ShowFeaturePanel()
    {
        featureText.text = featureDescription;
        featurePanel.SetActive(true);
        IsFeatureActive = true; // ✅ mark as active
    }

    public void HideFeaturePanel()
    {
        featurePanel.SetActive(false);
        IsFeatureActive = false; // ✅ mark as inactive
    }
}
