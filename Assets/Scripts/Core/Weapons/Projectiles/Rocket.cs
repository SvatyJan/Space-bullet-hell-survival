using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Rocket : MonoBehaviour
{
    [Header("Attributes")]
    /** Rychlost rakety. */
    [SerializeField] private float speed = 5f;

    /** Maximální vzdálenost, kterou může raketa urazit. */
    [SerializeField] private float maxDistance = 10f;

    /** Poloměr exploze. */
    [SerializeField] private float explosionRadius = 2f;

    /** Rychlost otáčení rakety. */
    [SerializeField] private float rotateSpeed = 200f;

    /** Čas po kterém raketa sama exploduje. */
    [SerializeField] private float timeUntilExplode = 5f;

    [Header("References")]
    /** Odkaz na cílový objekt. */
    private GameObject target;

    /** Odkaz na entitu, která raketu vystřelila. */
    public SpaceEntity owner;

    /** Seznam tagů, se kterými může raketa kolidovat. */
    [SerializeField] private List<string> collisionTags;

    /** Rigidbody rakety. */
    private Rigidbody2D rb;

    [Header("Runtime")]
    /** Poškození rakety. */
    private float rocketDamage;

    /** Kritická šance rakety. */
    private float rocketCritChance;

    /** Počáteční pozice rakety. */
    private Vector3 startPosition;

    /** Prefab efektu výbuchu rakety. */
    [SerializeField] private ParticleSystem explosionEffect;

    /** Potřebujeme referenci zbraně pro object pooling. */
    private IWeapon weapon;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        StartCoroutine(AutoRelease());
    }

    public void SetTarget(GameObject target)
    {
        this.target = target;
    }

    public void Initialize(IWeapon weapon, SpaceEntity owner, float damage)
    {
        this.weapon = weapon;
        this.owner = owner;
        this.rocketDamage = damage;
        this.rocketCritChance = owner.GetComponent<ShipStats>().CriticalChance;
    }

    private void FixedUpdate()
    {
        if (target == null) return;

        Vector2 direction = ((Vector2)target.transform.position - rb.position).normalized;

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        float angle = Mathf.MoveTowardsAngle(rb.rotation, targetAngle, rotateSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(angle);

        rb.linearVelocity = transform.up * speed;

        if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
        {
            Explode();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collisionTags.Contains(other.tag))
        {
            Explode();
        }
    }

    private void Explode()
    {
        Vector3 explosionPosition = transform.position;
        Collider2D[] hits = Physics2D.OverlapCircleAll(explosionPosition, explosionRadius);

        foreach (Collider2D hit in hits)
        {
            SpaceEntity hitEntity = hit.GetComponent<SpaceEntity>();
            if (hitEntity != null && hitEntity != owner)
            {
                hitEntity.TakeDamage(rocketDamage, rocketCritChance);
            }
        }

        if (explosionEffect != null)
        {
            ParticleSystem instance = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(instance.gameObject, 1f);
        }

        weapon.ReleaseProjectileFromPool(this.gameObject);
    }

    private IEnumerator AutoRelease()
    {
        yield return new WaitForSeconds(timeUntilExplode);
        weapon.ReleaseProjectileFromPool(gameObject);
    }
}
