using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour
{
    [Header("Portal Settings")]
    public float delayBeforeLoad = 1f;                 // Delay before triggering loading
    public string nextSceneName;                       // The next scene to load
    public AudioSource portalSound;                    // Optional portal SFX

    private bool isActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        // Only the player can activate
        if (isActivated || !other.CompareTag("Player"))
            return;

        isActivated = true;
        StartCoroutine(ActivatePortal());
    }

    private IEnumerator ActivatePortal()
    {
        Debug.Log("✨ Portal activated!");
        
        // Optional portal sound
        if (portalSound != null)
            portalSound.Play();

        // Wait for animation/sound
        yield return new WaitForSeconds(delayBeforeLoad);

        Debug.Log("➡️ Loading screen started...");

        // Go to the loading screen first, then load target scene
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            LoadingScreenManager.LoadSceneByName(nextSceneName);
        }
        else
        {
            Debug.LogWarning("⚠️ No next scene assigned in Portal!");
        }
    }
}
