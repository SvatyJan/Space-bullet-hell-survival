using UnityEngine;

public class Turret : MonoBehaviour, IWeapon
{
    /** Prefab projektilu. */
    public GameObject projectilePrefab;

    /** V�choz� bod st�ely. */
    public Transform shootingPoint;

    /** Interval mezi v�st�ely. */
    public float fireRate = 0.5f;

    /** �as pro p��t� v�st�el. */
    private float nextFireTime = 0f;

    /** Z�kladn� po�kozen� zbran�. */
    private float baseDamage = 10f;

    /** Odkaz na vlastn�ka zbran�. */
    public SpaceEntity owner;

    /** C�l st�elby. */
    private Vector3 targetDirection;

    [SerializeField] float baseDetectionRadius = 10f;

    void Start()
    {
        if (owner == null)
        {
            owner = GetComponentInParent<SpaceEntity>();

            if (owner == null)
            {
                owner = transform.root.GetComponentInChildren<SpaceEntity>();
            }

            if (owner == null)
            {
                Debug.LogError($"{gameObject.name}: SpaceEntity nebyl nalezen ani v p�edc�ch ani v rootu.");
            }
        }

        baseDamage = owner.GetComponent<ShipStats>().BaseDamage;
        if (shootingPoint == null)
        {
            shootingPoint = transform;
            Debug.LogWarning("Turret: shootingPoint nebyl nastaven, pou��v�m this.transform!");
        }
    }

    public void Fire()
    {
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;

            if (projectilePrefab == null)
            {
                Debug.LogError("Turret: projectilePrefab nen� p�i�azen! St�elba zru�ena.");
                return;
            }

            if (shootingPoint == null)
            {
                Debug.LogError("Turret: shootingPoint je null! St�elba zru�ena.");
                return;
            }

            targetDirection = GetNearestEnemyDirection();
            if (targetDirection == new Vector3(0,0,0)) { return; }

            GameObject projectile = Instantiate(projectilePrefab, shootingPoint.position, shootingPoint.rotation);

            Projectile projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.Initialize(owner, baseDamage);
                projectileScript.SetDirection(targetDirection);
            }
        }
    }

    private Vector3 GetNearestEnemyDirection()
    {
        float detectionRadius = baseDetectionRadius + owner.GetComponent<ShipStats>().DetectionRadius;
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
        fireRate -= 0.05f;
    }

    public void Evolve()
    {
        fireRate -= 0.5f;
        baseDamage += 5f;
    }
}
