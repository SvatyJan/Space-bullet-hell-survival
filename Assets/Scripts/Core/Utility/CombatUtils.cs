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

        if (criticalChance != null)
        {
            float maxCritRange = ((float)criticalChance / CRITICAL_CEILING) * 100f;

            System.Random random = new System.Random();
            float critRoll = random.Next(0, (int)maxCritRange);

            int critLevel = (int)(critRoll / 100f);

            float actualCriticalMultiplier = 1f;
            if (critLevel >= 0)
            {
                actualCriticalMultiplier = BASE_CRITICAL_MULTIPLIER + (critLevel * ADDITIONAL_CRITICAL_MULTIPLIER);
            }

            actualDamage = baseDamage * actualCriticalMultiplier;

            if (critLevel == 1)
                critColor = Color.yellow;
            else if (critLevel == 2)
                critColor = new Color(1f, 0.5f, 0f);
            else if (critLevel >= 3)
                critColor = Color.red;
        }

        return (actualDamage, critColor);
    }
}
