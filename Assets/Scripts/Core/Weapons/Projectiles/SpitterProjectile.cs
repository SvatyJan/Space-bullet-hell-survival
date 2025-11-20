using UnityEngine;
using System.Collections.Generic;

public class SpitterProjectile : MonoBehaviour
{
    [Header("References")]
    /** Odkaz na hráèe nebo nepøítele, který projektil vystøelil. */
    public SpaceEntity owner;

    [Header("Attributes")]
    /** Rychlost støely. */
    [SerializeField] public float speed = 10f;

    /** Poškození projektilu. */
    [SerializeField] private float projectileDamage = 10f;

    /** Kritická šance poškození projektilu. */
    [SerializeField] private float projectileCritChance = 0f;

    /** Doba, jak dlouho vydrží projektil než se znièí. */
    [SerializeField] private float projectileDuration = 5f;

    [Header("Collision")]
    /** Seznam tagù, se kterými projektil mùže kolidovat. */
    [SerializeField] private List<string> collisionTags;

    [Header("Runtime")]
    /** Smìr pohybu støely. */
    private Vector3 direction;

    /** Potøebujeme referenci nepøátele pro object pooling. */
    private SpitterBehavior spitterBehavior;

    private void Start()
    {
        Destroy(gameObject, projectileDuration);
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    // Nastavení smìru pohybu støely
    public void SetDirection(Vector3 direction)
    {
        this.direction = direction.normalized;
    }

    // Nastavení vlastnictví a poškození
    public void Initialize(SpitterBehavior spitterBehavior, SpaceEntity owner, float damage)
    {
        this.spitterBehavior = spitterBehavior;
        this.owner = owner;
        this.projectileDamage = projectileDamage + damage;
        this.projectileCritChance = owner.GetComponent<ShipStats>().CriticalChance;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collisionTags.Contains(other.tag))
        {
            SpaceEntity target = other.GetComponent<SpaceEntity>();
            if (target != null && target != owner)
            {
                target.TakeDamage(projectileDamage, projectileCritChance);
                spitterBehavior.ReleaseProjectileFromPool(this.gameObject);
            }
        }
    }
}