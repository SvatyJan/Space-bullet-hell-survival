using UnityEngine;

public class BioLauncher : MonoBehaviour, IWeapon
{
    [Header("Prefabs")]
    public GameObject projectilePrefab;

    [Header("Firing")]
    [SerializeField] private Transform shootingPoint;
    private float baseDamage = 10f;
    private float nextFireTime = 0f;
    public static int baseProjectileCount = 1;

    public SpaceEntity owner;
    private ShipStats stats;

    private bool evolved = false;

    private void Awake()
    {
        if (shootingPoint == null)
            shootingPoint = transform;
    }

    private void Start()
    {
        owner = GetComponentInParent<SpaceEntity>();
        stats = owner.GetComponent<ShipStats>();
    }

    public void Fire()
    {
        if (Time.time < nextFireTime) return;

        nextFireTime = Time.time + stats.FireRate;

        int projectilesFired = (baseProjectileCount + stats.ProjectilesCount);
        int count = Mathf.Max(1, projectilesFired);
        float spreadAngle = evolved ? 20f : 10f;
        float totalAngle = spreadAngle * (count - 1);

        for (int i = 0; i < count; i++)
        {
            float angleOffset = -totalAngle / 2 + spreadAngle * i;
            Quaternion rotation = shootingPoint.rotation * Quaternion.Euler(0, 0, angleOffset);

            GameObject projectile = Instantiate(projectilePrefab, shootingPoint.position, rotation);
            BioProjectile projectileScript = projectile.GetComponent<BioProjectile>();
            if (projectileScript != null)
            {
                float finalDamage = baseDamage + stats.BaseDamage;
                projectileScript.Initialize(owner, finalDamage);

                Vector2 direction = rotation * Vector2.up;
                projectileScript.SetDirection(direction);
            }
        }
    }

    public void Upgrade()
    {
        baseDamage += 3f;
        stats.FireRate *= 0.9f;
    }

    public void Evolve()
    {
        evolved = true;
        baseDamage += 5f;
        baseProjectileCount += 1;
    }
}
