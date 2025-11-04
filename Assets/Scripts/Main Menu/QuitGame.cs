using UnityEngine;
using UnityEngine.UI;

public class QuitGame : MonoBehaviour
{
    public Button quitButton;

    void Start()
    {
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitApplication);
    }

    public void QuitApplication()
    {
        Debug.Log("Quit Game pressed — exiting the game.");

        // Works only in a built version of the game
        Application.Quit();

        // In the Unity Editor, this will just stop Play Mode
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
