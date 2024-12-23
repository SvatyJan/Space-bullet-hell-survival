using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;   // Reference na Slider UI element
    [SerializeField] private Image fillImage;       // Obrazová èást pro vyplnìní zdraví
    [SerializeField] private Image backgroundImage; // Obrazová èást pro pozadí
    [SerializeField] private ShipStats shipStats;   // Reference na ShipStats komponentu

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

        // Nastavíme maximální hodnotu Slideru na maxHealth z ShipStats
        healthSlider.maxValue = shipStats.MaxHealth;
        healthSlider.value = shipStats.CurrentHealth;

        // Aktualizujeme barvu (volitelné)
        UpdateFillColor();
    }

    private void Update()
    {
        if (shipStats == null) return;

        // Synchronizujeme Slider s aktuálním zdravím
        healthSlider.value = shipStats.CurrentHealth;

        // Aktualizujeme barvu (pokud se rozhodnete dynamicky mìnit barvu)
        UpdateFillColor();
    }

    private void UpdateFillColor()
    {
        if (fillImage != null)
        {
            // Zmìna barvy podle procentuální hodnoty zdraví
            float healthPercentage = shipStats.CurrentHealth / shipStats.MaxHealth;

            if (healthPercentage > 0.5f)
                fillImage.color = Color.green;
            else if (healthPercentage > 0.2f)
                fillImage.color = Color.yellow;
            else
                fillImage.color = Color.red;
        }
    }
}
