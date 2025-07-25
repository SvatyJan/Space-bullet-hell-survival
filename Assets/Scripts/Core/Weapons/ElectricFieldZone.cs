﻿using System.Collections.Generic;
using UnityEngine;

public class ElectricFieldZone : MonoBehaviour
{
    [Header("Attributes")]
    /** Poškození aplikované na entitu. */
    private float baseDamage;

    /** Poloměr elektrického pole. */
    [SerializeField] private float radius = 1f;

    [Header("Targeting")]
    /** Tagy entit, které mohou být poškozeny. */
    private List<string> collisionTags;

    /** Seznam entit uvnitř pole. */
    private HashSet<SpaceEntity> enemiesInField = new HashSet<SpaceEntity>();

    [Header("References")]
    /** Odkaz na vlastníka pole pro získání dynamických hodnot (např. krit. šance). */
    private ShipStats shipStats;

    public void Initialize(float damage, float entitySize, List<string> entityCollisionTags, ShipStats stats)
    {
        this.baseDamage = damage;
        this.collisionTags = entityCollisionTags;
        this.shipStats = stats;

        transform.localScale = new Vector3(entitySize, entitySize, 0);
    }

    public void DealDamage()
    {
        float currentCritChance = shipStats != null ? shipStats.CriticalChance : 0f;

        foreach (var enemy in enemiesInField)
        {
            if (enemy != null)
            {
                enemy.TakeDamage(baseDamage, currentCritChance);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collisionTags.Contains(other.tag))
        {
            SpaceEntity enemy = other.GetComponent<SpaceEntity>();
            if (enemy != null)
            {
                enemiesInField.Add(enemy);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        SpaceEntity enemy = other.GetComponent<SpaceEntity>();
        if (enemy != null)
        {
            enemiesInField.Remove(enemy);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
