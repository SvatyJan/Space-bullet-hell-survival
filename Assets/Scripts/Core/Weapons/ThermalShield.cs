using System.Collections;
using UnityEngine;

public class ThermalShield : MonoBehaviour, IWeapon
{
    [Header("Prefabs")]
    /** Prefab term�ln� exploze. */
    [SerializeField] private GameObject explosionPrefab;

    [Header("Attributes")]
    /** V�choz� recharge �as mezi aktivacemi. */
    [SerializeField] private float rechargeTime = 12f;

    /** V�choz� velikost exploze. */
    [SerializeField] private float size = 2f;

    /** Maxim�ln� velikost v�buchu. */
    [SerializeField] private float maxSize = 10f;

    /** Rychlost r�stu v�buchu. */
    [SerializeField] private float expansionSpeed = 50f;

    /** Z�kladn� po�kozen� v�buchu. */
    [SerializeField] private float baseDamage = 10f;

    [Header("Runtime")]
    /** Stav aktivace �t�tu. */
    [SerializeField] private bool activeShield = true;

    /** Aktivn� instance exploze. */
    private GameObject activeExplosion;

    [Header("References")]
    /** Odkaz na entitu hr��e. */
    private SpaceEntity owner;

    /** Odkaz na komponentu hr��sk� lod�. */
    private PlayerShip player;

    /** Atributy hr��e. */
    private ShipStats shipStats;

    private void Start()
    {
        owner = GetComponentInParent<SpaceEntity>();
        player = GetComponentInParent<PlayerShip>();
        shipStats = owner?.GetComponent<ShipStats>();

        if (owner == null)
        {
            Debug.LogError("ThermalShield: Nelze naj�t vlastn�ka (SpaceEntity)!");
        }

        if (player == null)
        {
            Debug.LogError("ThermalShield: Tento skript lze pou��t pouze na PlayerShip!");
        }

        if (shipStats == null)
        {
            Debug.LogError("ThermalShield: Chyb� ShipStats!");
        }
    }

    public void Fire()
    {
        // ThermalShield nevyu��v� aktivn� st�elbu.
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
        Debug.Log("Thermal Shield je znovu nabit�!");
    }
}
