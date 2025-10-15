using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void LoadSceneByName(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("[SceneLoader] Scene name is empty!");
            return;
        }

        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"[SceneLoader] Scene '{sceneName}' není v Build Settings.");
        }
    }

    public void QuitGame()
    {
        Debug.Log("[SceneLoader] Quit requested");
        Application.Quit();
    }
}
