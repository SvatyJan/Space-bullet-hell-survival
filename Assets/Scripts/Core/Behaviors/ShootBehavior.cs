using UnityEngine;

public class ShootBehavior : EnemyBehaviorBase
{
    public GameObject projectilePrefab; // Prefab støely
    private float nextFireTime = 0f;    // Èas do další støelby
    private Vector3 direction;          // Smìr k cíli

    public override void Execute()
    {
        if (target == null) return;

        // Výpoèet smìru k hráèi
        direction = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

        // Vzdálenost mezi Spitterem a hráèem
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget <= shipStats.AttackRadius)
        {
            // Hráè je v dosahu útoku
            if (Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + shipStats.FireRate;
                ShootProjectile(direction);
            }
        }
        else
        {
            // Hráè je mimo dosah útoku -> Spitter se pohybuje smìrem k hráèi
            ChaseTarget(direction);
        }
    }

    private void ShootProjectile(Vector3 direction)
    {
        // Vytvoøíme støelu na pozici shootingPoint a nastavíme její smìr
        GameObject projectileInstance = Instantiate(projectilePrefab, shootingPoint.position, shootingPoint.rotation);
        Projectile projectileScript = projectileInstance.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.Initialize(spaceEntity, shipStats.BaseDamage);
            projectileScript.SetDirection(direction);
        }
    }

    private void ChaseTarget(Vector3 direction)
    {
        // Pohyb smìrem k hráèi
        transform.position += direction * shipStats.Speed * Time.deltaTime;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f); // Poloprùhledná èervená
        Gizmos.DrawSphere(transform.position, shipStats.AttackRadius);
    }
}