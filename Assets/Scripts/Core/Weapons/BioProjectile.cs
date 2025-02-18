using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BioProjectile : MonoBehaviour
{
    /** Odkaz na staty hráèe nebo nepøítele, který projekt vystøelil. */
    private ShipStats shipStats;

    /** Odkaz na hráèe nebo nepøítele, který projektil vystøelil. */
    public SpaceEntity owner;

    /** Prefab projektilu pøi explozi. */
    public GameObject explosionProjectilePrefab;

    /** Rychlost støely. */
    [SerializeField] public float speed = 10f;

    /** Seznam tagù, se kterými projektil mùže kolidovat. */
    [SerializeField] private List<string> collisionTags;

    /** Procentuální poškození projektilu pøi nalepení. */
    [SerializeField] private float healthPercentageDamage = 0.05f;

    /** Doba pøilepení projektilu. */
    [SerializeField] private float stickDuration = 3f;

    /** Doba, jak dlouho vydrží projektil než se znièí. */
    [SerializeField] private float projectileDuration = 5f;

    /** Smìr pohybu støely. */
    private Vector3 direction;

    private Transform target;
    private SpaceEntity targetEntity;
    [SerializeField] private bool isAttached = false;

    private void Start()
    {
        Destroy(gameObject, projectileDuration);
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    public void SetDirection(Vector3 direction)
    {
        this.direction = direction.normalized;
    }

    public void Initialize(SpaceEntity owner, float damage)
    {
        this.owner = owner;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isAttached) return;

        Debug.Log(isAttached);

        // Zkontroluj, zda tag objektu je v seznamu povolených kolizí
        if (collisionTags.Contains(other.tag))
        {
            target = other.transform;
            targetEntity = other.GetComponent<SpaceEntity>();
            if (targetEntity != null && targetEntity != owner)
            {
                AttachToTarget();
            }
        }
    }

    private void AttachToTarget()
    {
        isAttached = true;
        transform.SetParent(target);
        transform.localPosition = Vector3.zero;

        StartCoroutine(DamageOverTime());
    }

    private IEnumerator DamageOverTime()
    {
        float damage = shipStats.BaseDamage + (targetEntity.getShipStats().MaxHealth * healthPercentageDamage);
        targetEntity.TakeDamage(damage);

        yield return new WaitForSeconds(stickDuration);
        
        if(targetEntity.getShipStats().CurrentHealth < 0)
        {
            Explode();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Explode()
    {
        for(int i = 0; i < 5; i++)
        {
            GameObject newProjectile = Instantiate(explosionProjectilePrefab, transform.position, Quaternion.identity);
            Rigidbody2D rb = newProjectile.GetComponent<Rigidbody2D>();

            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            rb.velocity = randomDirection;
        }

        Destroy(gameObject);
    }
}
