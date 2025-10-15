using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ShipCard : MonoBehaviour
{
    [Header("UI Refs")]
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text statsText;
    [SerializeField] private Button selectButton;
    [SerializeField] private GameObject selectedHighlight;

    public Action onSelect;

    private void Awake()
    {
        if (selectButton != null)
            selectButton.onClick.AddListener(() => onSelect?.Invoke());
    }

    public void Setup(string title, Sprite sprite, string statsMultiline, bool isSelected)
    {
        if (titleText) titleText.text = title;
        if (icon) icon.sprite = sprite;
        if (statsText) statsText.text = statsMultiline;
        if (selectedHighlight) selectedHighlight.SetActive(isSelected);
    }

    public void SetSelected(bool isSelected)
    {
        if (selectedHighlight) selectedHighlight.SetActive(isSelected);
    }
}
