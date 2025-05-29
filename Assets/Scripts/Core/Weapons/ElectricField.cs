using System.Collections.Generic;
using UnityEngine;

public class ElectricField : MonoBehaviour, IWeapon
{
    [Header("Attributes")]
    /** Z�kladn� po�kozen� zbran�. */
    [SerializeField] private float baseDamage = 10f;

    /** Interval mezi v�st�ely (��m ni���, t�m rychlej��). */
    [SerializeField] public float baseFireRate = 0.5f;

    /** Velikost elektrick�ho pole. */
    [SerializeField] private float size = 6f;

    [Header("Runtime")]
    /** �as pro p��t� v�st�el. */
    [SerializeField] private float nextFireTime = 0f;

    /** Aktivn� instance elektrick�ho pole. */
    [SerializeField] private GameObject activeFieldZone;

    [Header("Prefabs")]
    /** Prefab elektrick�ho pole. */
    [SerializeField] public GameObject electricFieldZonePrefab;

    [Header("References")]
    /** Odkaz na vlastn�ka zbran�. */
    private SpaceEntity owner;

    /** Odkaz na staty hr��e. */
    private ShipStats stats;

    /** Seznam tag�, se kter�mi projektil m��e kolidovat. */
    [SerializeField] private List<string> collisionTags;

    private void Start()
    {
        owner = GetComponentInParent<SpaceEntity>();
        stats = owner.GetComponent<ShipStats>();

        if (owner == null || stats == null)
        {
            Debug.LogError("ElectricField: Nenalezen vlastn�k nebo ShipStats!");
        }
    }

    public void Fire()
    {
        if (activeFieldZone == null)
        {
            // Vytvo�en� nov� z�ny
            activeFieldZone = Instantiate(electricFieldZonePrefab, transform.position, Quaternion.identity, transform);
            ElectricFieldZone fieldZoneScript = activeFieldZone.GetComponent<ElectricFieldZone>();
            if (fieldZoneScript != null)
            {
                fieldZoneScript.Initialize(GetFinalDamage(), size, collisionTags);
            }
        }
        else
        {
            ElectricFieldZone fieldZoneScript = activeFieldZone.GetComponent<ElectricFieldZone>();
            float totalFireRate = Mathf.Max(0.05f, baseFireRate * stats.FireRate);

            if (Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + totalFireRate;
                fieldZoneScript.DealDamage();
            }
        }
    }

    public void Upgrade()
    {
        baseDamage += 10f;
        size += 1f;
        ReinitializeFieldZone();
    }

    public void Evolve()
    {
        baseDamage += 50f;
        size += 5f;
        ReinitializeFieldZone();
    }

    private void ReinitializeFieldZone()
    {
        if (activeFieldZone != null)
        {
            ElectricFieldZone fieldZoneScript = activeFieldZone.GetComponent<ElectricFieldZone>();
            if (fieldZoneScript != null)
            {
                fieldZoneScript.Initialize(GetFinalDamage(), size, collisionTags);
            }
        }
    }

    private float GetFinalDamage()
    {
        return baseDamage + stats.BaseDamage;
    }
}
