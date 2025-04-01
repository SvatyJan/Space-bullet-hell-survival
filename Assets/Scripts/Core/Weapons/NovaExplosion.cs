using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class NovaExplosion : MonoBehaviour
{
    [SerializeField] private float maxSize = 15f;
    [SerializeField] private float expansionSpeed = 10f;
    [SerializeField] private float damage;
    [SerializeField] private float baseDamage = 20f;

    /** Seznam tagù, se kterými projektil mùže kolidovat. */
    [SerializeField] private List<string> collisionTags;

    public void Initialize(float entityDamage)
    {
        damage = baseDamage + entityDamage;
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
            catch(NullReferenceException)
            {
                Destroy(collision.gameObject);
                return;
            }
        }
    }
}
