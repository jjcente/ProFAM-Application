using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class LoadingScreenManager : MonoBehaviour
{
    public static string nextSceneToLoad;      // ✅ For loading by name
    public static int nextSceneIndex = -1;     // ✅ For loading by index

    [Header("UI References")]
    public CanvasGroup portalCanvasGroup;      // For fade in/out
    public TextMeshProUGUI loadingText;
    public TextMeshProUGUI transportingText;
    public Animator portalAnimator;            // Animator on the image
    public AudioSource loadingSound;

    [Header("Fade Settings")]
    public float fadeDuration = 1.5f;
    public float dotAnimationSpeed = 0.5f;     // Speed for the loading dots

    private bool isAnimatingDots = true;

    private void Start()
    {
        // Start both loading animation and scene transition
        StartCoroutine(AnimateLoadingDots());
        StartCoroutine(HandleLoadingSequence());
    }

    // ✅ Load by Scene Name
    public static void LoadSceneByName(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("⚠️ Invalid scene name passed to LoadSceneByName!");
            return;
        }

        nextSceneToLoad = sceneName;
        nextSceneIndex = -1; // Reset index
        SceneManager.LoadScene("Loading Screen");
    }

    // ✅ Load by Scene Index
    public static void LoadSceneByIndex(int sceneIndex)
    {
        if (sceneIndex < 0 || sceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning($"⚠️ Scene index {sceneIndex} is out of range!");
            return;
        }

        nextSceneIndex = sceneIndex;
        nextSceneToLoad = null; // Reset name
        SceneManager.LoadScene("Loading Screen");
    }

    // ✅ Handles the fade, animation, and loading transition
    private IEnumerator HandleLoadingSequence()
    {
        // Fade In effect
        yield return StartCoroutine(FadeCanvasGroup(portalCanvasGroup, 0f, 1f, fadeDuration));

        // ✅ Safe animator trigger (only if "Open" exists)
        if (portalAnimator != null && HasTrigger(portalAnimator, "Open"))
        {
            portalAnimator.SetTrigger("Open");
        }

        // Play sound (optional)
        if (loadingSound != null)
            loadingSound.Play();

        // Wait for sound or a short default delay
        float waitTime = (loadingSound != null && loadingSound.clip != null)
            ? loadingSound.clip.length
            : 3f;
        yield return new WaitForSeconds(waitTime);

        // Fade Out before switching scenes
        yield return StartCoroutine(FadeCanvasGroup(portalCanvasGroup, 1f, 0f, fadeDuration));

        // Stop loading text animation
        isAnimatingDots = false;

        // ✅ Load the next scene safely
        if (nextSceneIndex >= 0 && nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else if (!string.IsNullOrEmpty(nextSceneToLoad))
        {
            SceneManager.LoadScene(nextSceneToLoad);
        }
        else
        {
            Debug.LogWarning("⚠️ No next scene specified for LoadingScreenManager!");
        }
    }

    // ✅ Fade helper function
    private IEnumerator FadeCanvasGroup(CanvasGroup group, float start, float end, float duration)
    {
        if (group == null)
            yield break;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            group.alpha = Mathf.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        group.alpha = end;
    }

    // ✅ Animator parameter checker (prevents “Parameter not found” errors)
    private bool HasTrigger(Animator animator, string triggerName)
    {
        foreach (var param in animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger && param.name == triggerName)
                return true;
        }
        return false;
    }

    // ✅ Animated "Loading..." text
    private IEnumerator AnimateLoadingDots()
    {
        if (loadingText == null)
            yield break;

        int dotCount = 0;

        while (isAnimatingDots)
        {
            dotCount = (dotCount + 1) % 4; // 0 to 3 dots
            loadingText.text = "Loading" + new string('.', dotCount);
            yield return new WaitForSeconds(dotAnimationSpeed);
        }
    }
}
