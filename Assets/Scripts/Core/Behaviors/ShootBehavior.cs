using UnityEngine;

public class ShootBehavior : EnemyBehaviorBase
{
    /** Prefab projektilu. */
    public GameObject projectilePrefab;

    /** �as kdy m��e znovu vyst�elit. */
    private float nextFireTime = 0f;

    /** Sm�r st�elby. */
    private Vector3 shootDirection;

    public override void Execute()
    {
        if (target == null) return;

        // V�po�et sm�ru k hr��i
        shootDirection = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

        // Vzd�lenost mezi entitou a targetem.
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget <= shipStats.AttackRadius)
        {
            if (Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + shipStats.FireRate;
                ShootProjectile(shootDirection);
            }
        }
        else
        {
            ChaseTarget(shootDirection);
        }
    }

    /** Vytvo�� projektil na pozici shootingPoint a nastav� jej� sm�r. */
    private void ShootProjectile(Vector3 direction)
    {
        GameObject projectileInstance = Instantiate(projectilePrefab, shootingPoint.position, shootingPoint.rotation);
        Projectile projectileScript = projectileInstance.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.Initialize(spaceEntity, shipStats.BaseDamage);
            projectileScript.SetDirection(direction);
        }
    }

    /** Pohybuje s entitou sm�rem k targetu. */
    private void ChaseTarget(Vector3 direction)
    {
        transform.position += direction * shipStats.Speed * Time.deltaTime;
    }
}