using UnityEngine;
using System.Collections.Generic;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;           // Rychlost støely
    private Vector3 direction;          // Smìr pohybu støely
    public SpaceEntity owner;           // Odkaz na hráèe nebo nepøítele, který projektil vystøelil
    [SerializeField]
    private List<string> collisionTags; // Seznam tagù, se kterými projektil mùže kolidovat

    private float projectileDamage;     // Poškození projektilu

    // Nastavení smìru pohybu støely
    public void SetDirection(Vector3 direction)
    {
        this.direction = direction.normalized;
    }

    // Nastavení vlastnictví a poškození
    public void Initialize(SpaceEntity owner, float damage)
    {
        this.owner = owner;
        this.projectileDamage = damage;
    }

    private void Update()
    {
        // Pohyb støely
        transform.position += direction * speed * Time.deltaTime;
    }

    private void Start()
    {
        // Znièení støely po 5 sekundách
        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Zkontroluj, zda tag objektu je v seznamu povolených kolizí
        if (collisionTags.Contains(other.tag))
        {
            // Najdi komponentu SpaceEntity na objektu, se kterým jsme kolidovali
            SpaceEntity target = other.GetComponent<SpaceEntity>();
            if (target != null && target != owner) // Zajistíme, že nezraníme vlastníka
            {
                target.TakeDamage(projectileDamage); // Udìlíme poškození cíli
                Destroy(gameObject); // Znièíme støelu po zásahu
            }
        }
    }
}