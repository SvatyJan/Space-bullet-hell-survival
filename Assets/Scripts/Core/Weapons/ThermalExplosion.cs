using System;
using System.Collections.Generic;
using UnityEngine;

public class ThermalExplosion : MonoBehaviour
{
    [SerializeField] private float maxSize;
    [SerializeField] private float expansionSpeed;
    [SerializeField] private float damage;

    public ThermalShield thermalShield;

    /** Seznam tagù, se kterými projektil mùže kolidovat. */
    [SerializeField] private List<string> collisionTags;

    public void Initialize(float entityDamage, float entitySize, ThermalShield entityThermalShield, float entityMaxSize, float entityExpansionSpeed)
    {
        damage += entityDamage;
        maxSize += entitySize;
        thermalShield = entityThermalShield;
        maxSize = entityMaxSize;
        expansionSpeed = entityExpansionSpeed;
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
            thermalShield.CleanupExplosion();
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collisionTags.Contains(collision.tag))
        {
            try
            {
                SpaceEntity enemy = collision.GetComponent<SpaceEntity>();
                enemy.TakeDamage(damage);
            }
            catch (NullReferenceException)
            {
                Destroy(collision.gameObject);
                return;
            }
        }
    }
}
