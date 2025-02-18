using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BioProjectile : MonoBehaviour
{
    /** Odkaz na staty hr��e nebo nep��tele, kter� projekt vyst�elil. */
    private ShipStats shipStats;

    /** Odkaz na hr��e nebo nep��tele, kter� projektil vyst�elil. */
    public SpaceEntity owner;

    /** Prefab projektilu p�i explozi. */
    public GameObject explosionProjectilePrefab;

    /** Rychlost st�ely. */
    [SerializeField] public float speed = 10f;

    /** Seznam tag�, se kter�mi projektil m��e kolidovat. */
    [SerializeField] private List<string> collisionTags;

    /** Procentu�ln� po�kozen� projektilu p�i nalepen�. */
    [SerializeField] private float healthPercentageDamage = 0.05f;

    /** Doba p�ilepen� projektilu. */
    [SerializeField] private float stickDuration = 3f;

    /** Doba, jak dlouho vydr�� projektil ne� se zni��. */
    [SerializeField] private float projectileDuration = 5f;

    /** Sm�r pohybu st�ely. */
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

        // Zkontroluj, zda tag objektu je v seznamu povolen�ch koliz�
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
