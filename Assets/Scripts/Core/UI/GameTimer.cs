using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public TMP_Text timerText;
    [SerializeField] public float elapsedTime;

    void Update()
    {
        elapsedTime += Time.deltaTime;
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        timerText.text = $"{minutes:D2}:{seconds:D2}";
    }
}
