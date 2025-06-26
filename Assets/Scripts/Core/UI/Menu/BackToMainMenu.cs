using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMainMenu : MonoBehaviour
{
    private const string mainMenuNameSceneName = "Main Menu";

    public void BackToMenu()
    {
        SceneManager.LoadScene(mainMenuNameSceneName);
    }
}
