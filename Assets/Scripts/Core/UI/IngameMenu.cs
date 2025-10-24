using UnityEngine;
using UnityEngine.SceneManagement;

public class IngameMenu : MonoBehaviour
{
    [Header("Menu Reference")]
    [SerializeField] private GameObject menuRoot;
    [SerializeField] private KeyCode toggleKey = KeyCode.Escape;

    private bool isMenuOpen = false;
    private bool prevCursorVisible;
    private CursorLockMode prevCursorLockMode;

    private void Start()
    {
        if (menuRoot != null)
            menuRoot.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            if (isMenuOpen)
                CloseMenu();
            else
                OpenMenu();
        }
    }

    public void OpenMenu()
    {
        if (isMenuOpen) return;

        isMenuOpen = true;
        if (menuRoot != null)
            menuRoot.SetActive(true);

        GameSpeedManager.SetGameSpeed(0f);

        prevCursorVisible = Cursor.visible;
        prevCursorLockMode = Cursor.lockState;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void CloseMenu()
    {
        if (!isMenuOpen) return;

        isMenuOpen = false;
        if (menuRoot != null)
            menuRoot.SetActive(false);

        GameSpeedManager.SetGameSpeed(1f);

        Cursor.visible = prevCursorVisible;
        Cursor.lockState = prevCursorLockMode;
    }

    public void ResumeGame()
    {
        CloseMenu();
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
