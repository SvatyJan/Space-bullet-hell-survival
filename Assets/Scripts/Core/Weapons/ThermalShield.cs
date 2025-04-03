using System.Collections;
using UnityEngine;

public class ThermalShield : MonoBehaviour, IWeapon
{
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float rechargeTime = 12f;
    [SerializeField] private float size = 2f;
    [SerializeField] private bool activeShield = true;

    [SerializeField] private float maxSize = 10f;
    [SerializeField] private float expansionSpeed = 50f;
    [SerializeField] private float damage = 10f;

    private GameObject activeExplosion;

    private SpaceEntity owner;
    private PlayerShip player;

    private void Start()
    {
        owner = GetComponentInParent<SpaceEntity>();
        if (owner == null)
        {
            Debug.LogError("ThermalShield: Nelze najít vlastníka (SpaceEntity)!");
            return;
        }

        player = GetComponentInParent<PlayerShip>();
        if (player == null)
        {
            Debug.LogError("ThermalShield: Tento skript lze použít pouze na PlayerShip!");
            return;
        }
    }
    public void Fire()
    {
        return;
    }

    public void TriggerExplosion()
    {
        if (!activeShield || activeExplosion != null) return;

        setActiveShield(false);

        activeExplosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity, transform);
        activeExplosion.GetComponent<ThermalExplosion>().Initialize(owner.getShipStats().BaseDamage, size, this, maxSize, expansionSpeed);
    }

    public void Upgrade()
    {
        rechargeTime = Mathf.Max(1f, rechargeTime - 1f);
    }

    public void Evolve()
    {
        rechargeTime = Mathf.Max(1f, rechargeTime - 2f);
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
        Destroy(activeExplosion);
        activeExplosion = null;

        if (!activeShield)
        {
            Debug.Log("Thermal Shield není aktivní! Nabíjím...");
            StartCoroutine(ResetShieldAfterCooldown());
            return;
        }
    }

    private IEnumerator ResetShieldAfterCooldown()
    {
        yield return new WaitForSeconds(rechargeTime);
        setActiveShield(true);
        Debug.Log("Thermal Shield je znovu nabitý!");
    }
}
