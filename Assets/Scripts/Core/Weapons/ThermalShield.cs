using System.Collections;
using UnityEngine;

public class ThermalShield : MonoBehaviour, IWeapon
{
    [Header("Prefabs")]
    /** Prefab termální exploze. */
    [SerializeField] private GameObject explosionPrefab;

    [Header("Attributes")]
    /** Výchozí recharge èas mezi aktivacemi. */
    [SerializeField] private float rechargeTime = 12f;

    /** Výchozí velikost exploze. */
    [SerializeField] private float size = 2f;

    /** Maximální velikost výbuchu. */
    [SerializeField] private float maxSize = 10f;

    /** Rychlost rùstu výbuchu. */
    [SerializeField] private float expansionSpeed = 50f;

    /** Základní poškození výbuchu. */
    [SerializeField] private float baseDamage = 10f;

    [Header("Runtime")]
    /** Stav aktivace štítu. */
    [SerializeField] private bool activeShield = true;

    /** Aktivní instance exploze. */
    private GameObject activeExplosion;

    [Header("References")]
    /** Odkaz na entitu hráèe. */
    private SpaceEntity owner;

    /** Odkaz na komponentu hráèské lodì. */
    private PlayerShip player;

    /** Atributy hráèe. */
    private ShipStats shipStats;

    private void Start()
    {
        owner = GetComponentInParent<SpaceEntity>();
        player = GetComponentInParent<PlayerShip>();
        shipStats = owner?.GetComponent<ShipStats>();

        if (owner == null)
        {
            Debug.LogError("ThermalShield: Nelze najít vlastníka (SpaceEntity)!");
        }

        if (player == null)
        {
            Debug.LogError("ThermalShield: Tento skript lze použít pouze na PlayerShip!");
        }

        if (shipStats == null)
        {
            Debug.LogError("ThermalShield: Chybí ShipStats!");
        }
    }

    public void Fire()
    {
        // ThermalShield nevyužívá aktivní støelbu.
    }

    public void TriggerExplosion()
    {
        if (!activeShield || activeExplosion != null) return;

        setActiveShield(false);

        float finalDamage = baseDamage + shipStats.BaseDamage;
        float critChance = shipStats.CriticalChance;

        activeExplosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity, transform);
        ThermalExplosion explosionScript = activeExplosion.GetComponent<ThermalExplosion>();

        if (explosionScript != null)
        {
            explosionScript.Initialize(finalDamage, critChance, size, this, maxSize, expansionSpeed);
        }
    }

    public void Upgrade()
    {
        rechargeTime = Mathf.Max(1f, rechargeTime - 1f);
    }

    public void Evolve()
    {
        rechargeTime = Mathf.Max(1f, rechargeTime - 2f);
        size += 1f;
        maxSize += 2f;
        baseDamage += 10f;
    }

    public void setActiveShield(bool state)
    {
        activeShield = state;
    }

    public bool getActiveShield()
    {
        return activeShield;
    }

    public void CleanupExplosion()
    {
        if (activeExplosion != null)
        {
            Destroy(activeExplosion);
            activeExplosion = null;
        }

        if (!activeShield)
        {
            float adjustedCooldown = Mathf.Max(1f, rechargeTime * shipStats.FireRate);
            StartCoroutine(ResetShieldAfterCooldown(adjustedCooldown));
        }
    }

    private IEnumerator ResetShieldAfterCooldown(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        setActiveShield(true);
        Debug.Log("Thermal Shield je znovu nabitý!");
    }
}
