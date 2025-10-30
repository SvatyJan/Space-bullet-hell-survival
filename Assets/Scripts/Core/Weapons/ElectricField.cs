using System.Collections.Generic;
using UnityEngine;

public class ElectricField : MonoBehaviour, IWeapon
{
    [Header("Attributes")]
    /** Základní poškození zbranì. */
    [SerializeField] private float baseDamage = 10f;

    /** Interval mezi výstøely (èím nižší, tím rychlejší). */
    [SerializeField] public float baseFireRate = 0.5f;

    /** Velikost elektrického pole. */
    [SerializeField] private float size = 5f;

    [Header("Runtime")]
    /** Èas pro pøíští výstøel. */
    [SerializeField] private float nextFireTime = 0f;

    /** Aktivní instance elektrického pole. */
    [SerializeField] private GameObject activeFieldZone;

    [Header("Prefabs")]
    /** Prefab elektrického pole. */
    [SerializeField] public GameObject electricFieldZonePrefab;

    [Header("References")]
    /** Odkaz na vlastníka zbranì. */
    private SpaceEntity owner;

    /** Odkaz na staty hráèe. */
    private ShipStats stats;

    /** Seznam tagù, se kterými projektil mùže kolidovat. */
    [SerializeField] private List<string> collisionTags;

    private void Start()
    {
        owner = GetComponentInParent<SpaceEntity>();
        stats = owner.GetComponent<ShipStats>();

        if (owner == null || stats == null)
        {
            Debug.LogError("ElectricField: Nenalezen vlastník nebo ShipStats!");
        }
    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
    }

    public void Fire()
    {
        if (activeFieldZone == null)
        {
            // Vytvoøení nové zóny
            activeFieldZone = Instantiate(electricFieldZonePrefab, transform.position, Quaternion.identity, transform);
            ElectricFieldZone fieldZoneScript = activeFieldZone.GetComponent<ElectricFieldZone>();
            if (fieldZoneScript != null)
            {
                fieldZoneScript.Initialize(GetFinalDamage(), size, collisionTags, stats);
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

    public void Downgrade()
    {
        baseDamage -= 10f;
        size -= 1f;
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
                fieldZoneScript.Initialize(GetFinalDamage(), size, collisionTags, stats);
            }
        }
    }

    private float GetFinalDamage()
    {
        stats = owner.GetComponent<ShipStats>();
        return baseDamage + stats.BaseDamage;
    }
}
