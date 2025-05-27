using Unity.VisualScripting;
using UnityEngine;

public class Blaster : MonoBehaviour, IWeapon
{
    /** Prefab projektilu. */
    public GameObject projectilePrefab;

    /** V�choz� bod st�ely. */
    public Transform shootingPoint;

    /** Interval mezi v�st�ely. */
    public float baseFireRate = 1f;

    /** �as pro p��t� v�st�el. */
    private float nextFireTime = 0f;

    /** Z�kladn� po�kozen� zbran�. */
    private float baseDamage = 10f;

    /** Odkaz na vlastn�ka zbran�. */
    public SpaceEntity owner;

    /** Odkaz na atributy vlastn�ka zbran�. */
    private ShipStats shipStats;

    /** Pomocn� prom�nn� pro pr�ci s body v�st�elu. */
    private Transform[] shootingPoints;

    /** Index ur�uj�c� po�ad� bodu v�st�elu. */
    private int currentPointIndex = 0;

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