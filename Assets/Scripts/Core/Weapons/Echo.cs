using UnityEngine;

public class Echo : MonoBehaviour, IWeapon
{
    [Header("Prefabs")]
    /** Prefab projektilu. */
    [SerializeField] private GameObject echoProjectilePrefab;

    [Header("Firing Points")]
    /** Pole bodù, ze kterých mùže být vystøelen projektil. */
    private Transform[] shootingPoints;

    /** Index urèující poøadí bodu výstøelu. */
    private int currentPointIndex = 0;

    [Header("Attributes")]
    /** Interval mezi výstøely. */
    [SerializeField] private float fireRate = 2f;

    /** Poèet projektilù vystøelených v oblouku. */
    [SerializeField] private int projectileCount = 6;

    /** Úhel rozptylu støel. */
    [SerializeField] private float spreadAngle = 180f;

    /** Základní poškození zbranì. */
    [SerializeField] private float baseDamage;

    [Header("Runtime")]
    /** Èas pro pøíští výstøel. */
    private float nextFireTime = 0f;

    [Header("References")]
    /** Odkaz na vlastníka zbranì. */
    private SpaceEntity owner;

    /** Odkaz na atributy vlastníka zbranì. */
    private ShipStats shipStats;

    private void Awake()
    {
        owner = GetComponentInParent<SpaceEntity>();
        shipStats = owner?.GetComponent<ShipStats>();

        if (shipStats == null)
        {
            Debug.LogError("Echo: ShipStats not found on parent!");
            shootingPoints = new Transform[] { transform };
            return;
        }

        Transform[] points = shipStats.ShootingPoints;
        shootingPoints = (points != null && points.Length > 0) ? points : new Transform[] { transform };
    }

    private void Start()
    {
        baseDamage = shipStats.BaseDamage;
    }

    public void Fire()
    {
        if (Time.time < nextFireTime) return;

        nextFireTime = Time.time + fireRate;
        FireEchoWave();
    }

    private void FireEchoWave()
    {
        float startAngle = -spreadAngle / 2f;
        float angleStep = spreadAngle / (Mathf.Max(projectileCount - 1, 1));

        Transform firingPoint = shootingPoints[currentPointIndex];
        currentPointIndex = (currentPointIndex + 1) % shootingPoints.Length;

        float baseRotation = firingPoint.rotation.eulerAngles.z;

        for (int i = 0; i < projectileCount; i++)
        {
            float angle = baseRotation + startAngle + (i * angleStep);
            Quaternion projectileRotation = Quaternion.Euler(0, 0, angle);

            GameObject projectile = Instantiate(echoProjectilePrefab, firingPoint.position, projectileRotation);

            Projectile projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.Initialize(owner, baseDamage);
                projectileScript.SetDirection(projectile.transform.up);
            }
        }
    }

    public void Upgrade()
    {
        projectileCount += 2;
        baseDamage += 5f;
        spreadAngle += 20f;
    }
    public void Downgrade()
    {
        projectileCount -= 2;
        baseDamage -= 5f;
        spreadAngle -= 20f;
    }

    public void Evolve()
    {
        spreadAngle = 360f;
        projectileCount += 3;
    }
}
