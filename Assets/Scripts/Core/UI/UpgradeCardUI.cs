using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeCardUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Transform evolveIconsContainer;
    [SerializeField] private GameObject evolveIconPrefab;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;
    [SerializeField] private Button button;
    [SerializeField] private Image innerBorder;
    [SerializeField] private Image outerBorder;

    public void SetUpgradeData(IUpgradeOption upgrade, System.Action onClick, bool Evolve = false)
    {
        if (icon != null) icon.sprite = upgrade.icon;
        if (title != null) title.text = upgrade.name;

        foreach (Transform child in evolveIconsContainer)
            Destroy(child.gameObject);

        if (upgrade.evolvesRequired != null && upgrade.evolvesRequired.Count != 0)
        {
            foreach (var evo in upgrade.evolvesRequired)
            {
                if (evo?.icon == null)
                    continue;

                GameObject newIcon = Instantiate(evolveIconPrefab, evolveIconsContainer);
                Image img = newIcon.GetComponent<Image>();
                img.sprite = evo.icon;

                img.color = new Color(1f, 1f, 1f, 0.9f);

                RectTransform rectTransform = img.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(64, 64);
            }
        }

        if (Evolve == true)
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
