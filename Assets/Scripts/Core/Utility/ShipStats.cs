using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShipStats : MonoBehaviour
{
    /** Maximální rychlost. */
    [SerializeField] private float speed = 10f;

    /** Rychlost rotace. */
    [SerializeField] private float rotationSpeed = 100f;

    /** Zrychlení. */
    [SerializeField] private float acceleration = 5f;

    /** Zpomalení. */
    [SerializeField] private float deceleration = 2f;

    /** Maximální zdraví lodi. */
    [SerializeField] private float maxHealth = 100f;

    /** Aktuální zdraví lodi. */
    [SerializeField] private float currentHealth = 100f;

    /** Základní poškození lodi. */
    [SerializeField] private float baseDamage = 10f;

    /** Rychlost pohybu. */
    [SerializeField] private Vector3 velocity;

    /** Rychlost útoku. */
    [SerializeField] private float fireRate = 1f;

    /** Oblast útoku. */
    [SerializeField] private float attackRadius = 1f;

    /** Vzdálenost ve které lod utoci. */
    [SerializeField] private float detectionRadius = 1f;

    /** Radius pro pøitažení xp. */
    [SerializeField] private float attractionRadius = 5f;

    /** Rychlost pøitahování xp. */
    [SerializeField] private float attractionSpeed = 2f;

    /** Aktuální poèet xp. */
    [SerializeField] private float xp = 0f;

    /** Poèet xp pro další level. */
    [SerializeField] private float xpNextlevelUp = 15f;

    /** Aktuální level. */
    [SerializeField] private float level = 1f;

    /** Maximální poèet zbraní. */
    [SerializeField] private int maxWeapons = 2;

    /** Poèet vylepšení podle statù. */
    private Dictionary<StatType, int> statUpgradeCounts = new Dictionary<StatType, int>();

    /** Maximální poèet rùzných statù. */
    [SerializeField] private int maxStatUpgrades = 3;

    /** List zbraní na lodi. */
    private List<string> equippedWeapons = new List<string>();

    /** List vylepšených atributù. */
    private List<string> upgradedStats = new List<string>();

    private void Awake()
    {
        // Inicializace poètu vylepšení pro každý StatType
        foreach (StatType stat in System.Enum.GetValues(typeof(StatType)))
        {
            statUpgradeCounts[stat] = 0;
        }
    }

    /** Kontrola jestli mùže pøidat vylepšení pro atribut. */
    public bool CanAddStatUpgrade(StatType statType)
    {
        // Zkontrolujeme, zda poèet vylepšení pro tento stat nepøekroèil maximum
        return statUpgradeCounts.ContainsKey(statType) && statUpgradeCounts[statType] < maxStatUpgrades;
    }

    /** Pøidá vylepšení atrubutu. */
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

    /** Kontrola zda mùže pøidat vylepšení atributu. */
    public bool CanAddStatUpgrade(string statName)
    {
        return upgradedStats.Count < maxStatUpgrades || upgradedStats.Contains(statName);
    }

    /** Pøidá vylepšení atrubutu. */
    public void AddStatUpgrade(string statName)
    {
        if (!upgradedStats.Contains(statName))
        {
            upgradedStats.Add(statName);
        }
    }

    /** Kontrola zda mùže pøidat zbraò. */
    public bool CanAddWeapon(string weaponName)
    {
        return equippedWeapons.Count < maxWeapons && !equippedWeapons.Contains(weaponName);
    }


    /** Pøidá zbraò. */
    public void AddWeapon(string weaponName)
    {
        if (CanAddWeapon(weaponName))
        {
            equippedWeapons.Add(weaponName);
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

    public float DetectionRadius
    {
        get { return detectionRadius; }
        set { detectionRadius = Mathf.Max(0, value); }
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