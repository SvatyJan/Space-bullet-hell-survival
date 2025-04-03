using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class ElectricField : MonoBehaviour, IWeapon
{
    [SerializeField] private float baseDamage = 10f;
    
    /** Interval mezi výstøely. */
    [SerializeField]  public float fireRate = 0.5f;

    /** Èas pro pøíští výstøel. */
    [SerializeField] private float nextFireTime = 0f;

    [SerializeField] private float size = 6f;

    [SerializeField] public GameObject electricFieldZonePrefab;

    private SpaceEntity owner;

    [SerializeField] private GameObject activeFieldZone;


    /** Seznam tagù, se kterými projektil mùže kolidovat. */
    [SerializeField] private List<string> collisionTags;

    private void Start()
    {
        owner = GetComponentInParent<SpaceEntity>();
        if (owner == null)
        {
            Debug.LogError("ElectricField: Nenalezen vlastník!");
        }
    }

    public void Fire()
    {
        if (activeFieldZone == null)
        {
            activeFieldZone = Instantiate(electricFieldZonePrefab, transform.position, Quaternion.identity, transform);
            ElectricFieldZone fieldZoneScript = activeFieldZone.GetComponent<ElectricFieldZone>();
            if (fieldZoneScript != null)
            {
                fieldZoneScript.Initialize(baseDamage, size, collisionTags);
            }
        }
        else
        {
            ElectricFieldZone fieldZoneScript = activeFieldZone.GetComponent<ElectricFieldZone>();

            if (Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + fireRate;
                fieldZoneScript.DealDamage();
            } 
        }
    }

    public void Upgrade()
    {
        baseDamage += 10f;
        size += 1f;

        ElectricFieldZone fieldZoneScript = activeFieldZone.GetComponent<ElectricFieldZone>();
        if (fieldZoneScript != null)
        {
            fieldZoneScript.Initialize(baseDamage, size, collisionTags);
        }
        else
        {
            Destroy(fieldZoneScript.gameObject);
            fieldZoneScript.Initialize(baseDamage, size, collisionTags);
        }    
    }

    public void Evolve()
    {
        baseDamage += 50f;
        size += 5f;

        ElectricFieldZone fieldZoneScript = activeFieldZone.GetComponent<ElectricFieldZone>();
        if (fieldZoneScript != null)
        {
            fieldZoneScript.Initialize(baseDamage, size, collisionTags);
        }
        else
        {
            Destroy(fieldZoneScript.gameObject);
            fieldZoneScript.Initialize(baseDamage, size, collisionTags);
        }
    }
}
