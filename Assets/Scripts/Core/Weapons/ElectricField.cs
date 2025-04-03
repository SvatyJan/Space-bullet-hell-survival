using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class ElectricField : MonoBehaviour, IWeapon
{
    [SerializeField] private float baseDamage = 10f;
    
    /** Interval mezi v�st�ely. */
    [SerializeField]  public float fireRate = 0.5f;

    /** �as pro p��t� v�st�el. */
    [SerializeField] private float nextFireTime = 0f;

    [SerializeField] private float size = 6f;

    [SerializeField] public GameObject electricFieldZonePrefab;

    private SpaceEntity owner;

    [SerializeField] private GameObject activeFieldZone;


    /** Seznam tag�, se kter�mi projektil m��e kolidovat. */
    [SerializeField] private List<string> collisionTags;

    private void Start()
    {
        owner = GetComponentInParent<SpaceEntity>();
        if (owner == null)
        {
            Debug.LogError("ElectricField: Nenalezen vlastn�k!");
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
