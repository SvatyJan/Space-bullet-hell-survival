using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeCardUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;
    [SerializeField] private Button button;

    public void SetUpgradeData(IUpgradeOption upgrade, System.Action onClick)
    {
        if (icon != null) icon.sprite = upgrade.icon;
        if (title != null) title.text = upgrade.name;
        if (description != null) description.text = upgrade.description;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick.Invoke());
    }
}
