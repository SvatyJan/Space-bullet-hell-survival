using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class XpBar : MonoBehaviour
{
    /** Reference na Slider UI element. */
    [SerializeField] private Slider xpSlider;

    /** Fill image pro zdraví. */
    [SerializeField] private Image fillIXpmage;

    /** Background image pro zdraví. */
    [SerializeField] private Image backgroundXpImage;

    /** Reference na Text levelu. */
    [SerializeField] private TextMeshProUGUI level;

    /** Reference na atributy lodì. */
    [SerializeField] private ShipStats shipStats;

    private void Start()
    {
        if (shipStats == null)
        {
            Debug.LogError("XpBar is missing a reference to ShipStats!");
            return;
        }

        if (xpSlider == null)
        {
            Debug.LogError("XphBar is missing a reference to the Slider!");
            return;
        }

        if (level == null)
        {
            Debug.LogError("XphBar is missing a reference to level!");
            return;
        }
    }

    private void Update()
    {
        if (shipStats == null) return;

        xpSlider.value = shipStats.XP;
        xpSlider.maxValue = shipStats.XpNextLevelUp;
        level.text = shipStats.Level.ToString();

        UpdateFillColor();
    }

    /** Aktualizuje barvu XP baru. */
    private void UpdateFillColor()
    {
        if (fillIXpmage != null)
        {
            // Zmìna barvy podle procentuální hodnoty xp
            float xpPercentage = shipStats.XP / shipStats.XpNextLevelUp;

            if (xpPercentage > 0.75f)
                fillIXpmage.color = Color.yellow;
            else if (xpPercentage > 0.5f)
                fillIXpmage.color = Color.magenta;
            else
                fillIXpmage.color = Color.cyan;
        }
    }
}
