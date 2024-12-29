using UnityEngine;

[CreateAssetMenu(fileName = "NewStatUpgradeOption", menuName = "Upgrades/StatUpgradeOption")]
public class StatUpgradeOption : ScriptableObject, UpgradeOption
{
    public string description;   // Popis upgradu
    public StatType statType;    // Typ statu, kter� se m� zv��it
    public float increaseAmount; // Hodnota, o kterou se stat zv���

    string UpgradeOption.description => description;

    public void Apply(ShipStats stats)
    {
        // Ov���me, zda hr�� m��e p�idat tento stat
        if (stats.CanAddStatUpgrade(statType.ToString()))
        {
            stats.AddStatUpgrade(statType.ToString());

            // Zv���me konkr�tn� atribut lodi podle v�b�ru
            switch (statType)
            {
                case StatType.Speed:
                    stats.Speed += increaseAmount;
                    break;
                case StatType.Health:
                    stats.MaxHealth += increaseAmount;
                    stats.CurrentHealth += increaseAmount;
                    break;
                case StatType.Damage:
                    stats.BaseDamage += increaseAmount;
                    break;
                case StatType.FireRate:
                    stats.FireRate += increaseAmount;
                    break;
                case StatType.XP:
                    stats.XP += increaseAmount;
                    break;
                case StatType.AttractRadius:
                    stats.AttractionRadius += increaseAmount;
                    break;
                default:
                    Debug.LogWarning("Unknown stat type!");
                    break;
            }
        }
        else
        {
            Debug.Log($"Cannot upgrade {statType}, maximum reached or already selected.");
        }
    }
}

// V��et dostupn�ch stat�
public enum StatType
{
    Speed,
    Health,
    Damage,
    FireRate,
    XP,
    AttractRadius
}