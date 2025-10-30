using System.Collections;
using UnityEngine;

public class Turret : MonoBehaviour, IWeapon
{
    [Header("Prefabs")]
    /** Prefab projektilu. */
    public GameObject projectilePrefab;

    [Header("Firing Points")]
    /** Pole bodù, ze kterých mùže být vystøelen projektil. */
    private Transform[] shootingPoints;

    /** Index urèující poøadí bodu výstøelu. */
    private int currentPointIndex = 0;

    [Header("Attributes")]
    /** Interval mezi výstøely. */
    [SerializeField] private float fireRate = 0.5f;

    /** Èas pro pøíští výstøel. */
    private float nextFireTime = 0f;

    /** Základní poškození zbranì. */
    [SerializeField] private float baseDamage = 10f;

    /** Základní detekèní rádius pro hledání cíle. */
    [SerializeField] private float baseDetectionRadius = 10f;

    [Header("References")]
    /** Odkaz na vlastníka zbranì. */
    private SpaceEntity owner;

    /** Atributy vlastníka zbranì. */
    private ShipStats shipStats;

    [Header("Targeting")]
    /** Cíl støelby. */
    private Vector3 targetDirection;

    private void Start()
    {
        // Najdi vlastníka
        owner = GetComponentInParent<SpaceEntity>();
        shipStats = owner?.GetComponent<ShipStats>();

        if (owner == null || shipStats == null)
        {
            Debug.LogError($"{gameObject.name}: Turret nemùže najít vlastníka nebo ShipStats.");
        }

        // Pøevzít poškození ze statù hráèe
        baseDamage += shipStats.BaseDamage;

        // Získání støeleckých bodù
        if (shipStats.ShootingPoints == null || shipStats.ShootingPoints.Length == 0)
        {
            shootingPoints = new Transform[] { transform };
            Debug.LogWarning("Turret: Nebyly nalezeny shootingPoints, používá se this.transform.");
        }
        else
        {
            shootingPoints = shipStats.ShootingPoints;
        }
    }

    public void Fire()
    {
        if (Time.time < nextFireTime) return;

        int totalProjectiles = 1 + shipStats.ProjectilesCount;
        StartCoroutine(FireProjectilesCoroutine(totalProjectiles));

        float totalFireRate = Mathf.Max(0.05f, fireRate * shipStats.FireRate);
        nextFireTime = Time.time + totalFireRate;
    }

    private IEnumerator FireProjectilesCoroutine(int count)
    {
        for (int i = 0; i < count; i++)
        {
            int safeLength = Mathf.Max(1, shootingPoints.Length);
            Transform firingPoint = shootingPoints[currentPointIndex % safeLength];
            currentPointIndex = (currentPointIndex + 1) % safeLength;

            if (firingPoint == null) yield break;

            targetDirection = GetNearestEnemyDirection();
            if (targetDirection == Vector3.zero) yield break;

            Quaternion rotation = Quaternion.LookRotation(Vector3.forward, targetDirection);

            GameObject projectile = Instantiate(projectilePrefab, firingPoint.position, rotation);

            Projectile projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.Initialize(owner, baseDamage);
                projectileScript.SetDirection(targetDirection);
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private Vector3 GetNearestEnemyDirection()
    {
        float detectionRadius = baseDetectionRadius + shipStats.DetectionRadius;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                float distance = Vector2.Distance(transform.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = collider.transform;
                }
            }
        }

        if (closestEnemy != null)
        {
            return (closestEnemy.position - transform.position).normalized;
        }

        return Vector3.zero;
    }

    public void Upgrade()
    {
        fireRate = Mathf.Max(0.05f, fireRate - 0.05f);
        if (float.IsNaN(fireRate) || fireRate <= 0f) fireRate = 0.05f;
        baseDamage += 3f;
    }

    public void Downgrade()
    {
        fireRate = Mathf.Max(0.05f, fireRate + 0.05f);
        if (float.IsNaN(fireRate) || fireRate <= 0f) fireRate = 0.05f;
        baseDamage -= 3f;
    }

    public void Evolve()
    {
        fireRate = Mathf.Max(0.05f, fireRate - 0.2f);
        if (float.IsNaN(fireRate) || fireRate <= 0f) fireRate = 0.05f;
        baseDamage += 10f;
    }
}
