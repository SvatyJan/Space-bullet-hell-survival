using UnityEngine;

public class ShootBehavior : EnemyBehaviorBase 
{
    public GameObject projectilePrefab;
    public float fireRate = 1f;
    private float nextFireTime = 0f;

    private Transform shootingPoint;      // V�choz� pozice st�ely
    private Transform target;
    private SpaceEntity spaceEntity;
    protected new ShipStats shipStats;

    private Vector3 direction;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        spaceEntity = GetComponent<SpaceEntity>();
        shipStats = GetComponent<ShipStats>();

        // Zkontrolujeme, zda je shootingPoint nastaven; pokud ne, pou�ijeme pozici nep��tele
        if (shootingPoint == null)
        {
            shootingPoint = spaceEntity.shootingPoint;
        }
    }

    public override void Execute()
    {
        if (target == null) return;

        // Ot��en� sm�rem k hr��i
        this.direction = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

        // St�elba na hr��e v intervalu fireRate
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;

            ShootProjectile(direction);
        }
    }

    private void ShootProjectile(Vector3 direction)
    {
        // Vytvo��me st�elu na pozici shootingPoint a nastav�me jej� sm�r
        GameObject projectileInstance = Instantiate(projectilePrefab, shootingPoint.position, shootingPoint.rotation);
        Projectile projectileScript = projectileInstance.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.Initialize(spaceEntity, shipStats.BaseDamage);
            projectileScript.SetDirection(direction);
        }
    }
}