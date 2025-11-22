using System.Collections.Generic;
using UnityEngine;

public class EndPortal : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject victoryScreenPrefab;
    [SerializeField] private string uiRootTag = "UI";

    [Header("Trigger")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool pauseOnShow = true;

    private GameObject victoryScreenInstance;
    private bool shown;

    private void Start()
    {
        Transform uiRoot = null;
        var uiGO = GameObject.FindGameObjectWithTag(uiRootTag);
        if (uiGO != null) uiRoot = uiGO.transform;

        if (victoryScreenPrefab != null)
        {
            victoryScreenInstance = Instantiate(victoryScreenPrefab, uiRoot);
            victoryScreenInstance.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (shown) return;
        if (!other.CompareTag(playerTag)) return;

        shown = true;

        if (victoryScreenInstance != null)
            victoryScreenInstance.SetActive(true);

        if (pauseOnShow)
            Time.timeScale = 0f;

        var col = GetComponent<Collider2D>();
        if (col) col.enabled = false;
    }
}
