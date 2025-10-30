using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStatUpgradeOption", menuName = "Upgrades/StatUpgradeOption")]
public class StatUpgradeOption : ScriptableObject, IUpgradeOption
{
    /** Popis upgradu. */
    public string description;

    [SerializeField] public Sprite icon;

    [SerializeField] public List<WeaponUpgradeOption> evolvesRequired;

    /** Hodnota, o kterou se stat zv���. */
    [SerializeField] private float increaseAmount = 1f;

    /** P�i�azen� popisu vylep�en�. */
    string IUpgradeOption.name => name;
    string IUpgradeOption.description => description;
    Sprite IUpgradeOption.icon => icon;
    List<IUpgradeOption>? IUpgradeOption.evolvesRequired => evolvesRequired?.ConvertAll(i => (IUpgradeOption)i);

    /** Typ statu, kter� se m� zv��it. */
    public StatType statType;

    /** Aplikuje atributy. */
    public GameObject? Apply(ShipStats stats)
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
            case StatType.HealthRegen:
                stats.HealthRegen += increaseAmount;
                break;
            case StatType.Damage:
                stats.BaseDamage += increaseAmount;
                break;
            case StatType.CriticalChance:
                stats.CriticalChance += increaseAmount;
                break;
            case StatType.FireRate:
                stats.FireRate -= increaseAmount;
                break;
            case StatType.AttractRadius:
                stats.AttractionRadius += increaseAmount;
                break;
            case StatType.XP:
                stats.XP += increaseAmount;
                break;
            case StatType.ProjectilesCount:
                stats.ProjectilesCount += (int)increaseAmount;
                break;
            default:
                Debug.LogWarning("Unknown stat type!");
                break;
        }

        return null;
    }

    /** Aplikuje downgrade atributu. */
    public GameObject? Remove(ShipStats stats)
    {
        stats.RemoveStatUpgrade(statType.ToString());

        switch (statType)
        {
            case StatType.Speed:
                stats.Speed -= increaseAmount;
                break;
            case StatType.Health:
                stats.MaxHealth -= increaseAmount;
                stats.CurrentHealth -= increaseAmount;
                break;
            case StatType.HealthRegen:
                stats.HealthRegen -= increaseAmount;
                break;
            case StatType.Damage:
                stats.BaseDamage -= increaseAmount;
                break;
            case StatType.CriticalChance:
                stats.CriticalChance -= increaseAmount;
                break;
            case StatType.FireRate:
                stats.FireRate += increaseAmount;
                break;
            case StatType.AttractRadius:
                stats.AttractionRadius -= increaseAmount;
                break;
            case StatType.XP:
                stats.XP -= increaseAmount;
                break;
            case StatType.ProjectilesCount:
                stats.ProjectilesCount -= (int)increaseAmount;
                break;
            default:
                Debug.LogWarning("Unknown stat type!");
                break;
        }

        return null;
    }
}