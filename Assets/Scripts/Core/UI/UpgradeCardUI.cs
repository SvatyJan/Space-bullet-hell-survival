using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeCardUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;
    [SerializeField] private Button button;
    [SerializeField] private Image innerBorder;
    [SerializeField] private Image outerBorder;

    public void SetUpgradeData(IUpgradeOption upgrade, System.Action onClick, bool Evolve = false)
    {
        if (icon != null) icon.sprite = upgrade.icon;
        if (title != null) title.text = upgrade.name;


        if(Evolve == true)
        {
            if (innerBorder != null) innerBorder.color = HexToColor("#2B0A3D");
            if (outerBorder != null) outerBorder.color = HexToColor("#A63FFF");
            if (description != null) description.text = "Evolve!";
        }
        else
        {
            if (innerBorder != null) innerBorder.color = HexToColor("#003749");
            if (outerBorder != null) outerBorder.color = HexToColor("#F2A315");
            if (description != null) description.text = upgrade.description;
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick.Invoke());
    }

    // Pomocná metoda pro pøevod hex na Color
    private Color HexToColor(string hex)
    {
        Color color;
        if (UnityEngine.ColorUtility.TryParseHtmlString(hex, out color))
        {
            return color;
        }
        else
        {
            Debug.LogWarning($"Neplatná hex barva: {hex}");
            return Color.white;
        }
    }

}
