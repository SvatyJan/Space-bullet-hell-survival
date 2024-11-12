using UnityEngine;

public class ShootBehavior : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform shootingPoint;      // Výchozí pozice støely
    public float fireRate = 1f;
    private float nextFireTime = 0f;
    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Zkontrolujeme, zda je shootingPoint nastaven; pokud ne, použijeme pozici nepøítele
        if (shootingPoint == null)
        {
            shootingPoint = this.transform;
        }
    }

    private void Update()
    {
        if (player != null)
        {
            // Otáèení smìrem k hráèi
            Vector3 direction = (player.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

            // Støelba na hráèe v intervalu fireRate
            if (Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + fireRate;
                ShootProjectile(direction);
            }
        }
    }

    private void ShootProjectile(Vector3 direction)
    {
        // Vytvoøíme støelu na pozici shootingPoint a nastavíme její smìr
        GameObject projectileInstance = Instantiate(projectilePrefab, shootingPoint.position, shootingPoint.rotation);
        Projectile projectileScript = projectileInstance.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.SetDirection(direction);
        }
    }
}