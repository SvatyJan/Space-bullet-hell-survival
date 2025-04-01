using UnityEngine;

public class Echo : MonoBehaviour, IWeapon
{
    /** Prefab projektilu. */
    [SerializeField] private GameObject echoProjectilePrefab;

    /** V�choz� bod st�ely. */
    [SerializeField] private Transform shootingPoint;

    /** Interval mezi v�st�ely. */
    [SerializeField] private float fireRate = 2f;

    /** Po�et projektil� vyst�elen�ch v oblouku. */
    [SerializeField] private int projectileCount = 6;

    /** �hel rozptylu st�el. */
    [SerializeField] private float spreadAngle = 180f;

    /** Z�kladn� po�kozen� zbran�. */
    [SerializeField] private float baseDamage;

    /** Odkaz na vlastn�ka zbran�. */
    private SpaceEntity owner;

    /** �as pro p��t� v�st�el. */
    private float nextFireTime = 0f;

    private void Awake()
    {
        // Najdeme st�eleck� bod podle tagu
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
