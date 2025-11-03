using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // ✅ Load next scene in build order with the loading screen
    public void LoadNextLevel()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        // Make sure next scene exists
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            LoadingScreenManager.LoadSceneByIndex(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("⚠️ No next scene found in Build Settings!");
        }
    }

    // ✅ Load a specific scene by index (useful for Main Menu)
    public void LoadSceneByIndex(int sceneIndex)
    {
        if (sceneIndex < 0 || sceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning("⚠️ Invalid scene index!");
            return;
        }

        LoadingScreenManager.LoadSceneByIndex(sceneIndex);
    }

    // ✅ Load a specific scene by name (optional)
    public void LoadSceneByName(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("⚠️ Invalid scene name!");
            return;
        }

        LoadingScreenManager.LoadSceneByName(sceneName);
    }
}
