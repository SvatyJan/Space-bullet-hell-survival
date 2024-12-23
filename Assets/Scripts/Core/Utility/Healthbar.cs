using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;   // Reference na Slider UI element
    [SerializeField] private Image fillImage;       // Obrazov� ��st pro vypln�n� zdrav�
    [SerializeField] private Image backgroundImage; // Obrazov� ��st pro pozad�
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

        // Nastav�me maxim�ln� hodnotu Slideru na maxHealth z ShipStats
        healthSlider.maxValue = shipStats.MaxHealth;
        healthSlider.value = shipStats.CurrentHealth;

        // Aktualizujeme barvu (voliteln�)
        UpdateFillColor();
    }

    private void Update()
    {
        if (shipStats == null) return;

        // Synchronizujeme Slider s aktu�ln�m zdrav�m
        healthSlider.value = shipStats.CurrentHealth;

        // Aktualizujeme barvu (pokud se rozhodnete dynamicky m�nit barvu)
        UpdateFillColor();
    }

    private void UpdateFillColor()
    {
        if (fillImage != null)
        {
            // Zm�na barvy podle procentu�ln� hodnoty zdrav�
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
