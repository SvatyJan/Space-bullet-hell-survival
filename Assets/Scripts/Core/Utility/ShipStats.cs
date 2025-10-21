using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShipStats : MonoBehaviour
{
    /** Násobizel kritického poškození. */
    public const float BASE_CRITICAL_MULTIPLIER = 1.5f;

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

    /** Rychlost regenerace zdraví za sekundu. */
    [SerializeField] private float healthRegen = 0f;

    /** Základní poškození lodi. */
    [SerializeField] private float baseDamage = 10f;

    /** 
     * Kritické poškození lodi.
     * 0-100 je 1,5x multiplier
     * 100-200 je 2x multiplier.
     */
    [SerializeField] private float criticalChance = 0f;

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

    /** Poèet xp pro další level. Je poèítáno GetXPForNextLevel metodou.*/
    [SerializeField] private float xpNextlevelUp = 15f;

    /** */
    [SerializeField] private float XPBase = 1f;
    [SerializeField] private float XPGrowth = 1.3f;
    [SerializeField] private float XPScale = 2.5f;

    /** Aktuální level. */
    [SerializeField] private float level = 1f;

    /** Maximální poèet zbraní. */
    [SerializeField] public int maxWeapons = 2;

    /** Poèet projektilù. */
    [SerializeField] private int projectilesCount = 2;

    /** Poèet vylepšení podle statù. */
    private Dictionary<StatType, int> statUpgradeCounts = new Dictionary<StatType, int>();

    /** Maximální poèet rùzných statù. */
    [SerializeField] public int maxStatUpgrades = 3;

    /** List zbraní na lodi. */
    private List<string> equippedWeapons = new List<string>();

    /** List vylepšených atributù. */
    private List<string> upgradedStats = new List<string>();

    /** Pole bodù ze kterých loï mùže støílet. */
    [SerializeField] public Transform[] ShootingPoints;

    private void Awake()
    {
        foreach (StatType stat in System.Enum.GetValues(typeof(StatType)))
        {
            statUpgradeCounts[stat] = 0;
        }
    }

    /** Kontrola jestli mùže pøidat vylepšení pro atribut. */
    public bool CanAddStatUpgrade(StatType statType)
    {
        return statUpgradeCounts.ContainsKey(statType);
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

    /** Vrátí true, pokud je stat na max úrovni. */
    public bool IsStatMaxed(StatType statType)
    {
        return statUpgradeCounts.ContainsKey(statType) && statUpgradeCounts[statType] >= 5;
    }

    /** Vrátí true, pokud má hráè danou zbraò. */
    public bool HasWeapon(string weaponName)
    {
        return equippedWeapons.Contains(weaponName);
    }

    /** Vrátí true, pokud má hráè maximální poèet zbraní. */
    public bool HasMaxWeapons()
    {
        return equippedWeapons.Count >= maxWeapons;
    }

    /** Vrátí úroveò zbranì (zatím jednoduše: poèet výskytù dané zbranì v listu). */
    public int GetWeaponLevel(string weaponName)
    {
        int level = 0;
        foreach (var weapon in equippedWeapons)
        {
            if (weapon == weaponName) level++;
        }
        return level;
    }

    /** Pøidá úroveò zbranì (i opakovanì). */
    public void UpgradeWeapon(string weaponName)
    {
        if (HasWeapon(weaponName))
        {
            equippedWeapons.Add(weaponName);
        }
        else if (!HasMaxWeapons())
        {
            equippedWeapons.Add(weaponName);
        }
    }

    /** Vrátí true, pokud je zbraò na max levelu (napø. 5). */
    public bool IsWeaponMaxed(string weaponName)
    {
        return GetWeaponLevel(weaponName) >= 5;
    }

    /** Vrátí true, pokud zbraò mùže být evolvována – má zbraò i její potøebný stat na max. */
    public bool CanEvolveWeapon(string weaponName)
    {
        if (!IsWeaponMaxed(weaponName)) return false;

        StatType requiredStat = GetLinkedStat(weaponName);
        return IsStatMaxed(requiredStat);
    }

    /** Vrátí typ stat upgrade, který danou zbraò umožòuje evolvovat. */
    private StatType GetLinkedStat(string weaponName)
    {
        switch (weaponName)
        {
            case "Laser": return StatType.FireRate;
            case "Blaster": return StatType.FireRate;
            default: return StatType.None;
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

    public float CriticalChance
    {
        get { return criticalChance; }
        set { criticalChance = value; }
    }

    public float MaxHealth
    {
        get { return maxHealth; }
        set { maxHealth = Mathf.Max(0, value); }
    }    

    public float HealthRegen
    {
        get { return healthRegen; }
        set { healthRegen = Mathf.Max(0, value); }
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

    public float GetXPForNextLevel(float level)
    {
        return Mathf.Round(XPBase + Mathf.Pow(level, XPGrowth) * XPScale);
    }

    public float Level
    {
        get { return level; }
        set { level = Mathf.Max(0, value); }
    }

    public int ProjectilesCount
    {
        get { return projectilesCount; }
        set { projectilesCount = value; }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        Gizmos.DrawSphere(transform.position, AttackRadius);
    }
}