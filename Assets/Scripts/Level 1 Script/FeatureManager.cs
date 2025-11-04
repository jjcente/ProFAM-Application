using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FeaturePanelManager : MonoBehaviour
{
    public static FeaturePanelManager Instance { get; private set; }

    // ✅ Add this static property
    public static bool IsFeatureActive { get; private set; } = false;

    [Header("UI References")]
    public GameObject featurePanel;
    public TMP_Text featureText;
    public Button closeButton;

    [TextArea]
    public string featureDescription = "Control a small fish that grows by eating smaller fish—but there’s a twist! Each fish carries a PISA-like question. Answer correctly to grow, answer wrong and shrink. Reach exactly 10 kg before the timer runs out to win!";

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
