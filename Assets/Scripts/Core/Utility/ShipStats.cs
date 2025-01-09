using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShipStats : MonoBehaviour
{
    [SerializeField] private float speed = 10f;           // Maximální rychlost
    [SerializeField] private float rotationSpeed = 100f;  // Rychlost rotace
    [SerializeField] private float acceleration = 5f;     // Zrychlení
    [SerializeField] private float deceleration = 2f;     // Zpomalení
    [SerializeField] private float maxHealth = 100f;      // Maximální zdraví lodi
    [SerializeField] private float currentHealth = 100f;  // Aktuální zdraví lodi
    [SerializeField] private float baseDamage = 10f;      // Základní poškození lodi
    [SerializeField] private Vector3 velocity;            // Rychlost pohybu
    [SerializeField] private float fireRate = 1f;         // Rychlost ptoku
    [SerializeField] private float attackRadius = 1f;     // Oblast útoku
    [SerializeField] private float attractionRadius = 5f; // Radius pro pøitažení xp
    [SerializeField] private float attractionSpeed = 2f;  // Rychlost pøitahování xp
    [SerializeField] private float xp = 0f;               // Aktuální poèet xp
    [SerializeField] private float xpNextlevelUp = 15f;   // Poèet xp pro další level
    [SerializeField] private float level = 1f;            // Aktuální level

    [SerializeField] private int maxWeapons = 2;          // Maximální poèet zbraní
    private Dictionary<StatType, int> statUpgradeCounts = new Dictionary<StatType, int>(); // Poèet vylepšení podle statù
    [SerializeField] private int maxStatUpgrades = 3;     // Maximální poèet rùzných statù
    private List<string> equippedWeapons = new List<string>();
    private List<string> upgradedStats = new List<string>();

    private void Awake()
    {
        // Inicializace poètu vylepšení pro každý StatType
        foreach (StatType stat in System.Enum.GetValues(typeof(StatType)))
        {
            statUpgradeCounts[stat] = 0;
        }
    }

    public bool CanAddStatUpgrade(StatType statType)
    {
        // Zkontrolujeme, zda poèet vylepšení pro tento stat nepøekroèil maximum
        return statUpgradeCounts.ContainsKey(statType) && statUpgradeCounts[statType] < maxStatUpgrades;
    }

    public void AddStatUpgrade(StatType statType)
    {
        if (CanAddStatUpgrade(statType))
        {
            statUpgradeCounts[statType]++;
        }
        else
        {
            Debug.LogWarning($"Cannot add upgrade to {statType}, maximum upgrades reached.");
        }
    }

    public bool CanAddWeapon(string weaponName)
    {
        return equippedWeapons.Count < maxWeapons && !equippedWeapons.Contains(weaponName);
    }

    public void AddWeapon(string weaponName)
    {
        if (CanAddWeapon(weaponName))
        {
            equippedWeapons.Add(weaponName);
        }
    }

    public bool CanAddStatUpgrade(string statName)
    {
        return upgradedStats.Count < maxStatUpgrades || upgradedStats.Contains(statName);
    }

    public void AddStatUpgrade(string statName)
    {
        if (!upgradedStats.Contains(statName))
        {
            upgradedStats.Add(statName);
        }
    }

    public float Speed
    {
        get { return speed; }
        set { speed = Mathf.Max(0, value); }
    }

    public float RotationSpeed
    {
        get { return rotationSpeed; }
        set { rotationSpeed = Mathf.Max(0, value); }
    }

    public float Acceleration
    {
        get { return acceleration; }
        set { acceleration = Mathf.Max(0, value); }
    }

    public float Deceleration
    {
        get { return deceleration; }
        set { deceleration = Mathf.Max(0, value); }
    }

    public float BaseDamage
    {
        get { return baseDamage; }
        set { baseDamage = value; }
    }

    public float MaxHealth
    {
        get { return maxHealth; }
        set { maxHealth = Mathf.Max(0, value); }
    }

    public float CurrentHealth
    {
        get { return currentHealth; }
        set { currentHealth = Mathf.Max(0, value); }
    }

    public Vector3 Velocity
    {
        get { return velocity; }
        set { velocity = value; }
    }

    public float FireRate
    {
        get { return fireRate; }
        set { fireRate = Mathf.Clamp(value, 0, 100); }
    }

    public float AttackRadius
    {
        get { return attackRadius; }
        set { attackRadius = Mathf.Max(0, value); }
    }

    public float AttractionRadius
    {
        get { return attractionRadius; }
        set { attractionRadius = Mathf.Max(0, value); }
    }

    public float AttractionSpeed
    {
        get { return attractionSpeed; }
        set { attractionSpeed = Mathf.Max(0, value); }
    }

    public float XP
    {
        get { return xp; }
        set { xp = Mathf.Max(0, value); }
    }

    public float XpNextLevelUp
    {
        get { return xpNextlevelUp; }
        set { xpNextlevelUp = Mathf.Max(0, value); }
    }

    public float Level
    {
        get { return level; }
        set { level = Mathf.Max(0, value); }
    }
}