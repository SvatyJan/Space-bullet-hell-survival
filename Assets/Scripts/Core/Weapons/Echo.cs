using UnityEngine;

public class Echo : MonoBehaviour, IWeapon
{
    /** Prefab projektilu. */
    [SerializeField] private GameObject echoProjectilePrefab;

    /** Výchozí bod støely. */
    [SerializeField] private Transform shootingPoint;

    /** Interval mezi výstøely. */
    [SerializeField] private float fireRate = 2f;

    /** Poèet projektilù vystøelených v oblouku. */
    [SerializeField] private int projectileCount = 6;

    /** Úhel rozptylu støel. */
    [SerializeField] private float spreadAngle = 180f;

    /** Základní poškození zbranì. */
    [SerializeField] private float baseDamage;

    /** Odkaz na vlastníka zbranì. */
    private SpaceEntity owner;

    /** Èas pro pøíští výstøel. */
    private float nextFireTime = 0f;

    private void Awake()
    {
        // Najdeme støelecký bod podle tagu
        GameObject point = GameObject.FindGameObjectWithTag("FrontShootingPoint");
        if (point != null)
        {
            shootingPoint = point.transform;
        }
        else
        {
            Debug.LogError("EchoWeapon: FrontShootingPoint tag not found!");
        }
    }

    private void Start()
    {
        owner = GetComponentInParent<SpaceEntity>();
        baseDamage = owner.GetComponent<ShipStats>().BaseDamage;
    }

    public void Fire()
    {
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            FireEchoWave();
        }
    }

    private void FireEchoWave()
    {
        float startAngle = -spreadAngle / 2;
        float angleStep = spreadAngle / (projectileCount - 1);
        float baseRotation = shootingPoint.rotation.eulerAngles.z;

        for (int i = 0; i < projectileCount; i++)
        {
            float angle = baseRotation + startAngle + (i * angleStep);
            Quaternion projectileRotation = Quaternion.Euler(0, 0, angle);
            GameObject projectile = Instantiate(echoProjectilePrefab, shootingPoint.position, projectileRotation);

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
    }

    public void Evolve()
    {
        spreadAngle = 180f;
        projectileCount += 3;
    }
}
