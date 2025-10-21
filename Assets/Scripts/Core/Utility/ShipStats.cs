using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShipStats : MonoBehaviour
{
    /** N�sobizel kritick�ho po�kozen�. */
    public const float BASE_CRITICAL_MULTIPLIER = 1.5f;

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

    /** Rychlost regenerace zdrav� za sekundu. */
    [SerializeField] private float healthRegen = 0f;

    /** Z�kladn� po�kozen� lodi. */
    [SerializeField] private float baseDamage = 10f;

    /** 
     * Kritick� po�kozen� lodi.
     * 0-100 je 1,5x multiplier
     * 100-200 je 2x multiplier.
     */
    [SerializeField] private float criticalChance = 0f;

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

    /** Po�et xp pro dal�� level. Je po��t�no GetXPForNextLevel metodou.*/
    [SerializeField] private float xpNextlevelUp = 15f;

    /** */
    [SerializeField] private float XPBase = 1f;
    [SerializeField] private float XPGrowth = 1.3f;
    [SerializeField] private float XPScale = 2.5f;

    /** Aktu�ln� level. */
    [SerializeField] private float level = 1f;

    /** Maxim�ln� po�et zbran�. */
    [SerializeField] public int maxWeapons = 2;

    /** Po�et projektil�. */
    [SerializeField] private int projectilesCount = 2;

    /** Po�et vylep�en� podle stat�. */
    private Dictionary<StatType, int> statUpgradeCounts = new Dictionary<StatType, int>();

    /** Maxim�ln� po�et r�zn�ch stat�. */
    [SerializeField] public int maxStatUpgrades = 3;

    /** List zbran� na lodi. */
    private List<string> equippedWeapons = new List<string>();

    /** List vylep�en�ch atribut�. */
    private List<string> upgradedStats = new List<string>();

    /** Pole bod� ze kter�ch lo� m��e st��let. */
    [SerializeField] public Transform[] ShootingPoints;

    private void Awake()
    {
        foreach (StatType stat in System.Enum.GetValues(typeof(StatType)))
        {
            statUpgradeCounts[stat] = 0;
        }
    }

    /** Kontrola jestli m��e p�idat vylep�en� pro atribut. */
    public bool CanAddStatUpgrade(StatType statType)
    {
        return statUpgradeCounts.ContainsKey(statType);
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

    /** Vr�t� true, pokud je stat na max �rovni. */
    public bool IsStatMaxed(StatType statType)
    {
        return statUpgradeCounts.ContainsKey(statType) && statUpgradeCounts[statType] >= 5;
    }

    /** Vr�t� true, pokud m� hr�� danou zbra�. */
    public bool HasWeapon(string weaponName)
    {
        return equippedWeapons.Contains(weaponName);
    }

    /** Vr�t� true, pokud m� hr�� maxim�ln� po�et zbran�. */
    public bool HasMaxWeapons()
    {
        return equippedWeapons.Count >= maxWeapons;
    }

    /** Vr�t� �rove� zbran� (zat�m jednodu�e: po�et v�skyt� dan� zbran� v listu). */
    public int GetWeaponLevel(string weaponName)
    {
        int level = 0;
        foreach (var weapon in equippedWeapons)
        {
            if (weapon == weaponName) level++;
        }
        return level;
    }

    /** P�id� �rove� zbran� (i opakovan�). */
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

    /** Vr�t� true, pokud je zbra� na max levelu (nap�. 5). */
    public bool IsWeaponMaxed(string weaponName)
    {
        return GetWeaponLevel(weaponName) >= 5;
    }

    /** Vr�t� true, pokud zbra� m��e b�t evolvov�na � m� zbra� i jej� pot�ebn� stat na max. */
    public bool CanEvolveWeapon(string weaponName)
    {
        if (!IsWeaponMaxed(weaponName)) return false;

        StatType requiredStat = GetLinkedStat(weaponName);
        return IsStatMaxed(requiredStat);
    }

    /** Vr�t� typ stat upgrade, kter� danou zbra� umo��uje evolvovat. */
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