using UnityEngine;

public class MeleeAttackBehavior : EnemyBehaviorBase
{
    private Vector3 direction;
    private float lastAttackTime = 0f;

    public override void Execute()
    {
        if (target == null) return;
        LookAtTarget();
        FlyTowards();
        AttackInRange();
    }

    /** Otáèí se smìrem targetu. */
    private void LookAtTarget()
    {
        direction = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
    }

    private void FlyTowards()
    {
        transform.position += direction * shipStats.Speed * Time.deltaTime;
    }

    private void AttackInRange()
    {
        // Pokud je cooldown hotový, zkontrolujeme, zda je hráè v dosahu
        if (Time.time >= lastAttackTime + shipStats.FireRate)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(shootingPoint.position, shipStats.AttackRadius);
            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    SpaceEntity player = hit.GetComponent<SpaceEntity>();
                    if (player != null)
                    {
                        player.TakeDamage(shipStats.BaseDamage); 
                        lastAttackTime = Time.time;
                    }
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (shootingPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(shootingPoint.position, shipStats.AttackRadius);
        }
    }
}