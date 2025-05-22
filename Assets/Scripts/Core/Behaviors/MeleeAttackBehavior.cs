using UnityEngine;

public class MeleeAttackBehavior : EnemyBehaviorBase
{
    private float lastAttackTime = 0f;

    protected override void ActWhenTargetReached()
    {
        RotateTowardsTarget();
        MoveForward();
        AttackPlayerInRange();
    }

    private void RotateTowardsTarget()
    {
        direction = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    private void MoveForward()
    {
        transform.position += direction * shipStats.Speed * Time.deltaTime;
    }

    private void AttackPlayerInRange()
    {
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
}
