using UnityEngine;
using System.Collections.Generic;

public class Projectile : MonoBehaviour
{
    /** Odkaz na staty hráèe nebo nepøítele, který projekt vystøelil. */
    private ShipStats shipStats;

    /** Odkaz na hráèe nebo nepøítele, který projektil vystøelil. */
    public SpaceEntity owner;

    /** Rychlost støely. */
    [SerializeField] public float speed = 10f;

    /** Seznam tagù, se kterými projektil mùže kolidovat. */
    [SerializeField] private List<string> collisionTags;

    /** Poškození projektilu. */
    [SerializeField] private float projectileDamage = 10f;

    /** Doba, jak dlouho vydrží projektil než se znièí. */
    [SerializeField] private float projectileDuration = 5f;

    /** Smìr pohybu støely. */
    private Vector3 direction;

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
    public void Initialize(SpaceEntity owner, float damage)
    {
        this.owner = owner;
        this.projectileDamage = projectileDamage + damage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Zkontroluj, zda tag objektu je v seznamu povolených kolizí
        if (collisionTags.Contains(other.tag))
        {
            SpaceEntity target = other.GetComponent<SpaceEntity>();
            if (target != null && target != owner)
            {
                target.TakeDamage(projectileDamage);
                Destroy(gameObject);
            }
        }
    }
}