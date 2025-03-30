using UnityEngine;

public class ShootBehavior : EnemyBehaviorBase
{
    /** Prefab projektilu. */
    public GameObject projectilePrefab;

    /** Èas kdy mùže znovu vystøelit. */
    private float nextFireTime = 0f;

    /** Smìr støelby. */
    private Vector3 shootDirection;

    public override void Execute()
    {
        if (target == null) return;

        // Výpoèet smìru k hráèi
        shootDirection = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

        // Vzdálenost mezi entitou a targetem.
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

    /** Vytvoøí projektil na pozici shootingPoint a nastaví její smìr. */
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

    /** Pohybuje s entitou smìrem k targetu. */
    private void ChaseTarget(Vector3 direction)
    {
        transform.position += direction * shipStats.Speed * Time.deltaTime;
    }
}