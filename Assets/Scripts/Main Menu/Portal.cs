using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Portal : MonoBehaviour
{

    [Header("Scene Settings")]
    public string loadingSceneName = "Loading Scene";

    public IEnumerator ActivateAndLoadNextScene(float delayBeforeLoad, string nextSceneName)
    {
        Debug.Log("✨ Portal activated!");

        yield return new WaitForSeconds(delayBeforeLoad);

        Debug.Log("➡️ Loading next level...");
        SceneManager.LoadScene(nextSceneName);
    }
}
