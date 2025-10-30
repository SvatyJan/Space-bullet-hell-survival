using UnityEngine;

public class BioLauncher : MonoBehaviour, IWeapon
{
    [Header("Prefabs")]
    /** Prefab projektilu. */
    public GameObject projectilePrefab;

    [Header("Firing Points")]
    /** Pole bod�, ze kter�ch m��e b�t vyst�elen projektil. */
    private Transform[] shootingPoints;

    /** Index ur�uj�c� po�ad� bodu v�st�elu. */
    private int currentPointIndex = 0;

    [Header("Attributes")]
    /** Z�kladn� po�kozen� zbran�. */
    [SerializeField] private float baseDamage = 10f;

    /** Statick� hodnota ur�uj�c� po�et projektil� pro efekty (nap�. bio v�buch). */
    public static int baseProjectileCount = 1;

    [Header("Runtime State")]
    /** �as pro p��t� v�st�el. */
    private float nextFireTime = 0f;

    /** Zda je zbra� v evolvovan�m stavu. */
    private bool evolved = false;

    [Header("References")]
    /** Odkaz na vlastn�ka zbran�. */
    public SpaceEntity owner;

    /** Odkaz na atributy vlastn�ka zbran�. */
    private ShipStats stats;

    private void Awake()
    {
        owner = GetComponentInParent<SpaceEntity>();
        stats = owner?.GetComponent<ShipStats>();

        if (stats == null)
        {
            Debug.LogError("BioLauncher: ShipStats not found!");
            shootingPoints = new Transform[] { transform };
            return;
        }

        Transform[] points = stats.ShootingPoints;
        if (points == null || points.Length == 0)
        {
            shootingPoints = new Transform[] { transform };
            Debug.LogWarning("BioLauncher: No shooting points assigned, using self.");
        }
        else
        {
            shootingPoints = points;
        }
    }

    public void Fire()
    {
        if (Time.time < nextFireTime) return;

        nextFireTime = Time.time + stats.FireRate;

        int projectilesFired = baseProjectileCount + stats.ProjectilesCount;
        int count = Mathf.Max(1, projectilesFired);
        float spreadAngle = evolved ? 20f : 10f;
        float totalAngle = spreadAngle * (count - 1);

        for (int i = 0; i < count; i++)
        {
            float angleOffset = -totalAngle / 2 + spreadAngle * i;
            Transform firingPoint = shootingPoints[currentPointIndex];
            currentPointIndex = (currentPointIndex + 1) % shootingPoints.Length;

            Quaternion rotation = firingPoint.rotation * Quaternion.Euler(0, 0, angleOffset);

            GameObject projectile = Instantiate(projectilePrefab, firingPoint.position, rotation);
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

    public void Downgrade()
    {
        baseDamage -= 3f;
        stats.FireRate /= 0.9f;
    }

    public void Evolve()
    {
        evolved = true;
        baseDamage += 5f;
        baseProjectileCount += 1;
    }
}
