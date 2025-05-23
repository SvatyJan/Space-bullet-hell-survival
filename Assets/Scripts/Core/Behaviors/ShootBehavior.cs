using UnityEngine;

public class ShootBehavior : EnemyBehaviorBase
{
    [Header("Shooting")]
    public GameObject projectilePrefab;
    private float nextFireTime = 0f;

    public override void Execute()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);
        bool hasSight = HasLineOfSight();

        if (!hasSight)
        {
            FollowPathToTarget(); // Base implementace
            return;
        }

        RotateTowardsTarget();

        if (distance > shipStats.AttackRadius)
        {
            ChaseTarget();
        }
        else
        {
            ActWhenTargetReached();
            currentPath = null;
        }
    }

    protected override void ActWhenTargetReached()
    {
        AttackPlayerInRange();
    }

    private void RotateTowardsTarget()
    {
        direction = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    private void AttackPlayerInRange()
    {
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + shipStats.FireRate;
            ShootProjectile(direction);
        }
    }

    private void ShootProjectile(Vector3 dir)
    {
        GameObject projectileInstance = Instantiate(projectilePrefab, shootingPoint.position, shootingPoint.rotation);
        Projectile projectileScript = projectileInstance.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.Initialize(spaceEntity, shipStats.BaseDamage);
            projectileScript.SetDirection(dir);
        }
    }

    private void ChaseTarget()
    {
        Vector3 moveDir = (target.position - transform.position).normalized;
        transform.position += moveDir * shipStats.Speed * Time.deltaTime;
    }
}
