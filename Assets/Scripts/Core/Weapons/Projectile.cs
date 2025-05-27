using UnityEngine;
using System.Collections.Generic;

public class Projectile : MonoBehaviour
{
    /** Odkaz na staty hr��e nebo nep��tele, kter� projekt vyst�elil. */
    private ShipStats shipStats;

    /** Odkaz na hr��e nebo nep��tele, kter� projektil vyst�elil. */
    public SpaceEntity owner;

    /** Rychlost st�ely. */
    [SerializeField] public float speed = 10f;

    /** Seznam tag�, se kter�mi projektil m��e kolidovat. */
    [SerializeField] private List<string> collisionTags;

    /** Po�kozen� projektilu. */
    [SerializeField] private float projectileDamage = 10f;

    /** Kritick� �ance po�kozen� projektilu. */
    [SerializeField] private float projectileCritChance = 0f;

    /** Doba, jak dlouho vydr�� projektil ne� se zni��. */
    [SerializeField] private float projectileDuration = 5f;

    /** Sm�r pohybu st�ely. */
    private Vector3 direction;

    private void Start()
    {
        Destroy(gameObject, projectileDuration);
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    // Nastaven� sm�ru pohybu st�ely
    public void SetDirection(Vector3 direction)
    {
        this.direction = direction.normalized;
    }

    // Nastaven� vlastnictv� a po�kozen�
    public void Initialize(SpaceEntity owner, float damage)
    {
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
                Destroy(gameObject);
            }
        }
    }
}