using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricFieldZone : MonoBehaviour
{
    private float damage;
    [SerializeField] private float radius = 1f;
    private HashSet<SpaceEntity> enemiesInField = new HashSet<SpaceEntity>();
    private List<string> collisionTags;

    public void Initialize(float baseDamage, float entitySize, List<string> entityCollisionTags)
    {
        damage = baseDamage;
        collisionTags = entityCollisionTags;

        transform.localScale = new Vector3(entitySize, entitySize, 0);
    }

    public void DealDamage()
    {
        foreach (var enemy in enemiesInField)
        {
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
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
