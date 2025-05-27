using UnityEngine;

public class CombatUtils
{
    private const float CRITICAL_CEILING = 100f;
    private const float BASE_CRITICAL_MULTIPLIER = 1.5f;
    private const float ADDITIONAL_CRITICAL_MULTIPLIER = 0.5f;

    public static (float damageDealt, Color critColor) CalculateCriticalDamage(float baseDamage, float? criticalChance)
    {
        float actualDamage = baseDamage;
        Color critColor = Color.white;

        if (criticalChance == null || criticalChance <= 0f)
            return (actualDamage, critColor);

        int level = Mathf.FloorToInt((float)criticalChance / CRITICAL_CEILING);
        float min = level * CRITICAL_CEILING;
        float max = min + CRITICAL_CEILING;

        float roll = Random.Range(min, max);
        Debug.Log("ROLL: " + roll);

        if (roll >= criticalChance)
        {
            float multiplier = BASE_CRITICAL_MULTIPLIER + (level * ADDITIONAL_CRITICAL_MULTIPLIER);
            actualDamage = baseDamage * multiplier;

            critColor = level switch
            {
                0 => Color.yellow,
                1 => new Color(1f, 0.5f, 0f),
                2 => Color.red,
                _ => new Color(0.8f, 0f, 1f)
            };
        }

        return (actualDamage, critColor);
    }
}
