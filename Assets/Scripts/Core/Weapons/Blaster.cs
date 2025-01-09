using UnityEngine;

public class Blaster : MonoBehaviour, IWeapon
{
    /** Prefab projektilu. */
    public GameObject projectilePrefab;

    /** Výchozí bod støely. */
    public Transform shootingPoint;

    /** Interval mezi výstøely. */
    public float fireRate = 0.5f;

    /** Èas pro pøíští výstøel. */
    private float nextFireTime = 0f;

    /** Základní poškození zbranì. */
    private float baseDamage = 10f;

    /** Odkaz na vlastníka zbranì. */
    public SpaceEntity owner;

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
        throw new System.NotImplementedException();
    }

    public void Evolve()
    {
        throw new System.NotImplementedException();
    }
}