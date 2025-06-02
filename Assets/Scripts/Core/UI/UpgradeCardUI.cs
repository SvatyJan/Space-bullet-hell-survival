using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeCardUI : MonoBehaviour
{
    public Image iconImage;
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public Button upgradeButton;

    public void SetUpgradeData(IUpgradeOption upgrade, System.Action onClick)
    {
        if (iconImage != null) iconImage.sprite = upgrade.icon;
        if (titleText != null) titleText.text = upgrade.name;
        if (descriptionText != null) descriptionText.text = upgrade.description;

        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(() => onClick());
    }
}
