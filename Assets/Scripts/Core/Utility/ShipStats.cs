using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShipStats : MonoBehaviour
{
    /** Maxim�ln� rychlost. */
    [SerializeField] private float speed = 10f;

    /** Rychlost rotace. */
    [SerializeField] private float rotationSpeed = 100f;

    /** Zrychlen�. */
    [SerializeField] private float acceleration = 5f;

    /** Zpomalen�. */
    [SerializeField] private float deceleration = 2f;

    /** Maxim�ln� zdrav� lodi. */
    [SerializeField] private float maxHealth = 100f;

    /** Aktu�ln� zdrav� lodi. */
    [SerializeField] private float currentHealth = 100f;

    /** Z�kladn� po�kozen� lodi. */
    [SerializeField] private float baseDamage = 10f;

    /** Rychlost pohybu. */
    [SerializeField] private Vector3 velocity;

    /** Rychlost �toku. */
    [SerializeField] private float fireRate = 1f;

    /** Oblast �toku. */
    [SerializeField] private float attackRadius = 1f;

    /** Vzd�lenost ve kter� lod utoci. */
    [SerializeField] private float detectionRadius = 1f;

    /** Radius pro p�ita�en� xp. */
    [SerializeField] private float attractionRadius = 5f;

    /** Rychlost p�itahov�n� xp. */
    [SerializeField] private float attractionSpeed = 2f;

    /** Aktu�ln� po�et xp. */
    [SerializeField] private float xp = 0f;

    /** Po�et xp pro dal�� level. */
    [SerializeField] private float xpNextlevelUp = 15f;

    /** Aktu�ln� level. */
    [SerializeField] private float level = 1f;

    /** Maxim�ln� po�et zbran�. */
    [SerializeField] private int maxWeapons = 2;

    /** Po�et vylep�en� podle stat�. */
    private Dictionary<StatType, int> statUpgradeCounts = new Dictionary<StatType, int>();

    /** Maxim�ln� po�et r�zn�ch stat�. */
    [SerializeField] private int maxStatUpgrades = 3;

    /** List zbran� na lodi. */
    private List<string> equippedWeapons = new List<string>();

    /** List vylep�en�ch atribut�. */
    private List<string> upgradedStats = new List<string>();

    private void Awake()
    {
        // Inicializace po�tu vylep�en� pro ka�d� StatType
        foreach (StatType stat in System.Enum.GetValues(typeof(StatType)))
        {
            statUpgradeCounts[stat] = 0;
        }
    }

    /** Kontrola jestli m��e p�idat vylep�en� pro atribut. */
    public bool CanAddStatUpgrade(StatType statType)
    {
        // Zkontrolujeme, zda po�et vylep�en� pro tento stat nep�ekro�il maximum
        return statUpgradeCounts.ContainsKey(statType) && statUpgradeCounts[statType] < maxStatUpgrades;
    }

    /** P�id� vylep�en� atrubutu. */
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

    /** Kontrola zda m��e p�idat vylep�en� atributu. */
    public bool CanAddStatUpgrade(string statName)
    {
        return upgradedStats.Count < maxStatUpgrades || upgradedStats.Contains(statName);
    }

    /** P�id� vylep�en� atrubutu. */
    public void AddStatUpgrade(string statName)
    {
        if (!upgradedStats.Contains(statName))
        {
            upgradedStats.Add(statName);
        }
    }

    /** Kontrola zda m��e p�idat zbra�. */
    public bool CanAddWeapon(string weaponName)
    {
        return equippedWeapons.Count < maxWeapons && !equippedWeapons.Contains(weaponName);
    }


    /** P�id� zbra�. */
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