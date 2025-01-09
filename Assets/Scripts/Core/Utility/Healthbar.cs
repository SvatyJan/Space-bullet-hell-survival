using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    /** Reference na Slider UI element. */
    [SerializeField] private Slider healthSlider;

    /** Fill image pro zdrav�. */
    [SerializeField] private Image fillIHealthmage;

    /** Background image pro zdrav�. */
    [SerializeField] private Image backgroundHealthImage;

    /** Reference na atributy lod�. */
    [SerializeField] private ShipStats shipStats;

    private void Start()
    {
        if (shipStats == null)
        {
            Debug.LogError("HealthBar is missing a reference to ShipStats!");
            return;
        }

        if (healthSlider == null)
        {
            Debug.LogError("HealthBar is missing a reference to the Slider!");
            return;
        }

        healthSlider.maxValue = shipStats.MaxHealth;
        healthSlider.value = shipStats.CurrentHealth;

        UpdateFillColor();
    }

    private void Update()
    {
        if (shipStats == null) return;

        healthSlider.value = shipStats.CurrentHealth;

        UpdateFillColor();
    }

    /** Aktualizuje barvu zrav�. */
    private void UpdateFillColor()
    {
        if (fillIHealthmage != null)
        {
            // Zm�na barvy podle procentu�ln� hodnoty zdrav�
            float healthPercentage = shipStats.CurrentHealth / shipStats.MaxHealth;

            if (healthPercentage > 0.5f)
                fillIHealthmage.color = Color.green;
            else if (healthPercentage > 0.2f)
                fillIHealthmage.color = Color.yellow;
            else
                fillIHealthmage.color = Color.red;
        }
    }
}
