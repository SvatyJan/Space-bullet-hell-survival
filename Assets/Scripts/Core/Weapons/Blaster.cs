using UnityEngine;

public class Blaster : MonoBehaviour, IWeapon
{
    /** Prefab projektilu. */
    public GameObject projectilePrefab;

    /** V�choz� bod st�ely. */
    public Transform shootingPoint;

    /** Interval mezi v�st�ely. */
    public float fireRate = 0.5f;

    /** �as pro p��t� v�st�el. */
    private float nextFireTime = 0f;

    /** Z�kladn� po�kozen� zbran�. */
    private float baseDamage = 10f;

    /** Odkaz na vlastn�ka zbran�. */
    public SpaceEntity owner;

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
            Debug.LogError("Blaster: FrontShootingPoint tag not found!");
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

            GameObject projectile = Instantiate(projectilePrefab, shootingPoint.position, shootingPoint.rotation);

            Projectile projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.Initialize(owner, baseDamage);
                projectileScript.SetDirection(shootingPoint.up);
            }
        }
    }

    public void Upgrade()
    {
        baseDamage += 5f;
        fireRate *= 0.9f;
    }

    public void Evolve()
    {
        baseDamage *= 2f;
        fireRate *= 0.5f;
    }
}