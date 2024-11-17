using UnityEngine;
using System.Collections.Generic;

public class Rocket : MonoBehaviour
{
    public float speed = 15f;               // Rychlost rakety
    public float maxDistance = 10f;         // Maximální vzdálenost letu
    public float explosionRadius = 2f;      // Rádius exploze
    private Vector3 direction;              // Směr pohybu rakety
    public SpaceEntity owner;               // Odkaz na hráče nebo nepřítele, který raketu vystřelil
    [SerializeField]
    private List<string> collisionTags;     // Seznam tagů, se kterými raketa může kolidovat

    private float rocketDamage;             // Poškození rakety
    private Vector3 startPosition;          // Počáteční pozice rakety

    // Nastavení směru pohybu rakety
    public void SetDirection(Vector3 direction)
    {
        this.direction = direction.normalized;
    }

    // Inicializace vlastníka rakety a poškození
    public void Initialize(SpaceEntity owner, float damage)
    {
        this.owner = owner;
        this.rocketDamage = damage;
        this.startPosition = transform.position;
    }

    private void Update()
    {
        // Pohyb rakety
        transform.position += direction * speed * Time.deltaTime;

        // Zkontrolujeme, zda raketa nepřekročila maximální vzdálenost
        if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
        {
            Explode();
        }
    }

    private void Start()
    {
        // Automatické zničení rakety po 5 sekundách (pro jistotu)
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
                Explode(); // Spustíme explozi
            }
        }
    }

    private void Explode()
    {
        // Najdeme všechny objekty v rádiusu exploze
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D hit in hits)
        {
            // Udělíme poškození objektům, které mají komponentu SpaceEntity
            SpaceEntity target = hit.GetComponent<SpaceEntity>();
            if (target != null && target != owner)
            {
                target.TakeDamage(rocketDamage);
            }
        }

        // Zničíme raketu po explozi
        Debug.Log("Rocket exploded at " + transform.position);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        // Vizualizace rádiusu exploze
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f); // Oranžová
        Gizmos.DrawSphere(transform.position, explosionRadius);
    }
}