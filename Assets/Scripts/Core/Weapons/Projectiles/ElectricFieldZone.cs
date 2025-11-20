using System;
using System.Collections.Generic;
using UnityEngine;

public class ElectricFieldZone : MonoBehaviour
{
    [Header("Attributes")]
    private float baseDamage = 10f;
    [SerializeField] private float radius = 5f;
    [SerializeField] private float tickInterval = 0.2f;
    [SerializeField] private int maxHitsPerTick = 64;

    [Header("Targeting")]
    [SerializeField] private List<string> collisionTags;

    [Header("References")]
    private ShipStats shipStats;

    private float timer;
    private Collider2D[] hitBuffer;

    private void Awake()
    {
        hitBuffer = new Collider2D[maxHitsPerTick];
    }

    public void Initialize(float damage, float entitySize, List<string> entityCollisionTags, ShipStats stats)
    {
        baseDamage = Math.Max(baseDamage, damage);
        collisionTags = entityCollisionTags;
        shipStats = stats;
        transform.localScale = new Vector3(entitySize, entitySize, 0);
        radius = entitySize-1.5f;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= tickInterval)
        {
            timer = 0f;
            DealDamage();
        }
    }

    public void DealDamage()
    {
        float crit = shipStats != null ? shipStats.CriticalChance : 0f;
        int count = Physics2D.OverlapCircleNonAlloc(transform.position, radius, hitBuffer);

        for (int i = 0; i < count; i++)
        {
            var col = hitBuffer[i];
            if (col == null) continue;
            if (collisionTags != null && collisionTags.Count > 0 && !collisionTags.Contains(col.tag)) continue;

            var entity = col.GetComponent<SpaceEntity>();
            if (entity != null)
            {
                entity.TakeDamage(baseDamage, crit);
            }

            hitBuffer[i] = null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, radius);
    }
}
