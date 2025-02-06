using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class Turret : MonoBehaviour, IWeapon
{
    /** Prefab projektilu. */
    public GameObject projectilePrefab;

    /** Výchozí bod støely. */
    public Transform shootingPoint;

    /** Interval mezi výstøely. */
    public float fireRate = 0.5f;

    /** Èas pro pøíští výstøel. */
    private float nextFireTime = 0f;

    /** Základní poškození zbranì. */
    private float baseDamage = 10f;

    /** Odkaz na vlastníka zbranì. */
    public SpaceEntity owner;

    /** Cíl støelby. */
    private Vector3 targetDirection;

    [SerializeField] float detectionRadius = 10f;

    void Start()
    {
        owner = GetComponentInParent<SpaceEntity>();
        baseDamage = owner.GetComponent<ShipStats>().BaseDamage;
    }

    void Update()
    {
        shootingPoint = this.transform;
    }

    public void Fire()
    {
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;

            targetDirection = GetNearestEnemyDirection();
            if (targetDirection == new Vector3(0,0,0)) { return; }

            GameObject projectile = Instantiate(projectilePrefab, shootingPoint.position, shootingPoint.rotation);

            Projectile projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.Initialize(owner, baseDamage);
                if(targetDirection != null)
                {
                    projectileScript.SetDirection(targetDirection);
                }                
            }
        }
    }

    private Vector3 GetNearestEnemyDirection()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                float distance = Vector2.Distance(transform.position, collider.transform.position);
                if(distance < detectionRadius)
                {
                    return (collider.transform.position - transform.position).normalized;
                }
            }
        }
        return new Vector3(0,0,0);
    }

    public void Upgrade()
    {
        throw new System.NotImplementedException();
    }

    public void Evolve()
    {
        throw new System.NotImplementedException();
    }
}
