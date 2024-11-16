using UnityEngine;

public class ShootBehavior : EnemyBehaviorBase 
{
    public GameObject projectilePrefab;
    public float fireRate = 1f;
    private float nextFireTime = 0f;

    private Transform shootingPoint;      // Výchozí pozice støely
    private Transform target;
    private SpaceEntity spaceEntity;
    protected new ShipStats shipStats;

    private Vector3 direction;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        spaceEntity = GetComponent<SpaceEntity>();
        shipStats = GetComponent<ShipStats>();

        // Zkontrolujeme, zda je shootingPoint nastaven; pokud ne, použijeme pozici nepøítele
        if (shootingPoint == null)
        {
            shootingPoint = spaceEntity.shootingPoint;
        }
    }

    public override void Execute()
    {
        if (target == null) return;

        // Otáèení smìrem k hráèi
        this.direction = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

        // Støelba na hráèe v intervalu fireRate
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;

            ShootProjectile(direction);
        }
    }

    private void ShootProjectile(Vector3 direction)
    {
        // Vytvoøíme støelu na pozici shootingPoint a nastavíme její smìr
        GameObject projectileInstance = Instantiate(projectilePrefab, shootingPoint.position, shootingPoint.rotation);
        Projectile projectileScript = projectileInstance.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.Initialize(spaceEntity, shipStats.BaseDamage);
            projectileScript.SetDirection(direction);
        }
    }
}