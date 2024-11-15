using UnityEngine;

public class Blaster : MonoBehaviour, IWeapon
{
    public GameObject projectilePrefab;   // Prefab projektilu
    public Transform shootingPoint;       // Výchozí bod støely
    public float fireRate = 0.5f;         // Interval mezi výstøely
    private float nextFireTime = 0f;      // Èas pro pøíští výstøel

    private float baseDamage = 10f;       // Základní poškození zbranì
    public SpaceEntity owner;             // Odkaz na vlastníka zbranì
    private void Start()
    {
        // Automaticky pøiøadí vlastníka z parent objektu
        baseDamage = GetComponent<ShipStats>().BaseDamage;
        owner = GetComponentInParent<SpaceEntity>();
    }

    public void Fire()
    {
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;

            // Vytvoøení støely na pozici a smìru `shootingPoint`
            GameObject projectile = Instantiate(projectilePrefab, shootingPoint.position, shootingPoint.rotation);

            // Pøidáme komponentì projektilu rychlost smìrem, kterým míøí `shootingPoint`
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.Initialize(owner, baseDamage);
                projectileScript.SetDirection(shootingPoint.up); // Nastaví smìr podle výchozího bodu
            }
        }
    }
}