using UnityEngine;

public class ShootBehavior : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform shootingPoint;      // V�choz� pozice st�ely
    public float fireRate = 1f;
    private float nextFireTime = 0f;
    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Zkontrolujeme, zda je shootingPoint nastaven; pokud ne, pou�ijeme pozici nep��tele
        if (shootingPoint == null)
        {
            shootingPoint = this.transform;
        }
    }

    private void Update()
    {
        if (player != null)
        {
            // Ot��en� sm�rem k hr��i
            Vector3 direction = (player.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

            // St�elba na hr��e v intervalu fireRate
            if (Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + fireRate;
                ShootProjectile(direction);
            }
        }
    }

    private void ShootProjectile(Vector3 direction)
    {
        // Vytvo��me st�elu na pozici shootingPoint a nastav�me jej� sm�r
        GameObject projectileInstance = Instantiate(projectilePrefab, shootingPoint.position, shootingPoint.rotation);
        Projectile projectileScript = projectileInstance.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.SetDirection(direction);
        }
    }
}