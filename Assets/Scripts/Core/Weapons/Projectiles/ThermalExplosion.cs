using System.Collections.Generic;
using UnityEngine;

public class ThermalExplosion : MonoBehaviour
{
    [Header("Explosion Settings")]
    /** Maximální velikost výbuchu. */
    [SerializeField] private float maxSize;

    /** Rychlost expanze výbuchu. */
    [SerializeField] private float expansionSpeed;

    /** Základní poškození výbuchu. */
    [SerializeField] private float damage;

    /** Kritická šance výbuchu. */
    private float criticalChance;

    [Header("References")]
    /** Odkaz na ThermalShield, který výbuch vytvoøil. */
    public ThermalShield thermalShield;

    [Header("Collision")]
    /** Seznam tagù, se kterými mùže výbuch kolidovat. */
    [SerializeField] private List<string> collisionTags;

    public void Initialize(float entityDamage, float critChance, float entitySize, ThermalShield sourceShield, float entityMaxSize, float entityExpansionSpeed)
    {
        damage = entityDamage;
        criticalChance = critChance;

        maxSize = entityMaxSize;
        expansionSpeed = entityExpansionSpeed;

        thermalShield = sourceShield;

        transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        if (transform.localScale.x < maxSize)
        {
            float scaleIncrease = expansionSpeed * Time.deltaTime;
            transform.localScale += new Vector3(scaleIncrease, scaleIncrease, 0);
        }
        else
        {
            thermalShield?.CleanupExplosion();
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collisionTags.Contains(collision.tag))
        {
            SpaceEntity enemy = collision.GetComponent<SpaceEntity>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, criticalChance);
            }
        }
    }
}
