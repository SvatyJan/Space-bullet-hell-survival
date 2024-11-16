using UnityEngine;

public class ShootBehavior : EnemyBehaviorBase 
{
    public GameObject projectilePrefab;
    private float nextFireTime = 0f;
    private Vector3 direction;

    public override void Execute()
    {
        if (target == null) return;

        // Ot��en� sm�rem k hr��i
        direction = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

        // St�elba na hr��e v intervalu fireRate
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + shipStats.FireRate;

            ShootProjectile(direction);
        }
    }

    private void ShootProjectile(Vector3 direction)
    {
        // Vytvo��me st�elu na pozici shootingPoint a nastav�me jej� sm�r
        GameObject projectileInstance = Instantiate(projectilePrefab, shootingPoint.position, shootingPoint.rotation);
        Projectile projectileScript = projectileInstance.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.Initialize(spaceEntity, shipStats.BaseDamage);
            projectileScript.SetDirection(direction);
        }
    }
}