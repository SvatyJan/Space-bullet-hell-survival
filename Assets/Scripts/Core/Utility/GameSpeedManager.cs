using UnityEngine;

public class GameSpeedManager : MonoBehaviour
{
    public static GameSpeedManager Instance { get; private set; }

    [Header("Current Runtime Speed")]
    [SerializeField] private float currentGameSpeed = 1f;
    [SerializeField] private float savedGameSpeed = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        ApplyGameSpeed(currentGameSpeed);
    }

    public static void SetGameSpeed(float newGameSpeed)
    {
        if (Instance == null)
        {
            Debug.LogWarning("GameSpeedManager not found in scene!");
            return;
        }

        Instance.ApplyGameSpeed(newGameSpeed);
    }

    public static float GetGameSpeed()
    {
        return Instance != null ? Instance.currentGameSpeed : 1f;
    }

    private void ApplyGameSpeed(float newGameSpeed)
    {
        currentGameSpeed = Mathf.Max(0f, newGameSpeed);
        Time.timeScale = currentGameSpeed;
    }

    public static void SetSavedGameSpeed(float newSavedGameSpeed)
    {
        if (Instance == null)
        {
            Debug.LogWarning("GameSpeedManager not found in scene!");
            return;
        }

        Instance.savedGameSpeed = newSavedGameSpeed;
    }

    public static float GetSavedGameSpeed()
    {
        return Instance != null ? Instance.savedGameSpeed : 1f;
    }

    public static void ApplySavedGameSpeed()
    {
        Time.timeScale = GetSavedGameSpeed();
    }
}
