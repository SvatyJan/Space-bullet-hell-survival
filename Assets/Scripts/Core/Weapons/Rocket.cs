using UnityEngine;
using System.Collections.Generic;

public class Rocket : MonoBehaviour
{
    /** Rychlost rakety. */
    public float speed = 15f;

    /** Maximální vzdálenost letu. */
    public float maxDistance = 10f;

    /** Rádius exploze. */
    public float explosionRadius = 2f;

    /** Směr pohybu rakety. */
    private Vector3 direction;

    /** Odkaz na entitu, která raketu vystřelila. */
    public SpaceEntity owner;

    /** Seznam tagů, se kterými raketa může kolidovat. */
    [SerializeField] private List<string> collisionTags;

    /** Poškození rakety. */
    private float rocketDamage;

    /** Počáteční pozice rakety. */
    private Vector3 startPosition;

    /** Nastavení směru pohybu rakety. */
    public void SetDirection(Vector3 direction)
    {
        this.direction = direction.normalized;
    }

    /** Inicializace vlastníka rakety a poškození. */
    public void Initialize(SpaceEntity owner, float damage)
    {
        this.owner = owner;
        this.rocketDamage = damage;
        this.startPosition = transform.position;
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        // Kontrola, zda raketa nepřekročila maximální vzdálenost.
        if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
        {
            Explode();
        }
    }

    private void Start()
    {
        // Automatické zničení rakety po 5 sekundách.
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

    /** Najde všechny objekty v rádiusu exploze a udělí poškození objektům, které mají komponentu SpaceEntity. */
    private void Explode()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D hit in hits)
        {
            SpaceEntity target = hit.GetComponent<SpaceEntity>();
            if (target != null && target != owner)
            {
                target.TakeDamage(rocketDamage);
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);
        Gizmos.DrawSphere(transform.position, explosionRadius);
    }
}