using UnityEngine;

public class ShootBehavior : EnemyBehaviorBase
{
    public GameObject projectilePrefab; // Prefab st�ely
    private float nextFireTime = 0f;    // �as do dal�� st�elby
    private Vector3 direction;          // Sm�r k c�li

    public override void Execute()
    {
        if (target == null) return;

        // V�po�et sm�ru k hr��i
        direction = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

        // Vzd�lenost mezi Spitterem a hr��em
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget <= shipStats.AttackRadius)
        {
            // Hr�� je v dosahu �toku
            if (Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + shipStats.FireRate;
                ShootProjectile(direction);
            }
        }
        else
        {
            // Hr�� je mimo dosah �toku -> Spitter se pohybuje sm�rem k hr��i
            ChaseTarget(direction);
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

    private void ChaseTarget(Vector3 direction)
    {
        // Pohyb sm�rem k hr��i
        transform.position += direction * shipStats.Speed * Time.deltaTime;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f); // Polopr�hledn� �erven�
        Gizmos.DrawSphere(transform.position, shipStats.AttackRadius);
    }
}