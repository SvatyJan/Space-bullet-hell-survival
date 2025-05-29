using UnityEngine;

public class Blaster : MonoBehaviour, IWeapon
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
    /** Interval mezi v�st�ely. */
    [SerializeField] public float baseFireRate = 1f;

    /** �as pro p��t� v�st�el. */
    [SerializeField] private float nextFireTime = 0f;

    /** Z�kladn� po�kozen� zbran�. */
    [SerializeField] private float baseDamage = 10f;

    [Header("References")]
    /** Odkaz na vlastn�ka zbran�. */
    private SpaceEntity owner;

    /** Odkaz na atributy vlastn�ka zbran�. */
    private ShipStats shipStats;

    private void Awake()
    {
        owner = GetComponentInParent<SpaceEntity>();
        shipStats = owner?.GetComponent<ShipStats>();

        if (shipStats == null)
        {
            Debug.LogError("Blaster: ShipStats not found on parent!");
        }

        Transform[] points = shipStats?.ShootingPoints;
        if (points == null || points.Length == 0)
        {
            shootingPoints = new Transform[] { transform };
            Debug.LogWarning("Blaster: No shooting points in ShipStats, using self.");
        }
        else
        {
            shootingPoints = points;
        }
    }

    private void Start()
    {
        owner = GetComponentInParent<SpaceEntity>();
        shipStats = owner.GetComponent<ShipStats>();

        baseDamage = shipStats.BaseDamage;
    }

    public void Fire()
    {
        if (Time.time < nextFireTime) return;

        float totalFireRate = Mathf.Max(0.05f, baseFireRate * shipStats.FireRate);

        nextFireTime = Time.time + totalFireRate;

        Transform firingPoint = shootingPoints[currentPointIndex];

        GameObject projectile = Instantiate(projectilePrefab, firingPoint.position, firingPoint.rotation);

        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.Initialize(owner, baseDamage);
            projectileScript.SetDirection(firingPoint.up);
        }

        currentPointIndex = (currentPointIndex + 1) % shootingPoints.Length;
    }

    public void Upgrade()
    {
        baseDamage += 5f;
        baseFireRate *= 0.9f;
    }

    public void Evolve()
    {
        baseDamage *= 2f;
        baseFireRate *= 0.5f;
    }
}