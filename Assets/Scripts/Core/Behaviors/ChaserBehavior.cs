using UnityEngine;

public class ChaserBehavior : EnemyBehaviorBase
{
    private float lastAttackTime = 0f;
    private Animator animator;
    private Rigidbody2D rb;
    private bool isAttacking = false;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public override void Execute()
    {
        UpdateAnimatorParameters();

        if (target == null)
        {
            SetAttacking(false);
            return;
        }

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > shipStats.DetectionRadius || !HasLineOfSight())
        {
            SetAttacking(false);
            FollowPathToTarget();
        }
        else
        {
            currentPath = null;
            ActWhenTargetReached();
        }
    }

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
                    SetAttacking(true);
                    SpaceEntity player = hit.GetComponent<SpaceEntity>();
                    if (player != null)
                    {
                        player.TakeDamage(shipStats.BaseDamage);
                        lastAttackTime = Time.time;
                    }
                }
                else
                {
                    SetAttacking(false);
                }
            }
        }
    }

    private void UpdateAnimatorParameters()
    {
        float currentSpeed = rb != null ? rb.linearVelocity.magnitude : 0f;
        animator?.SetFloat("speed", currentSpeed);
        animator?.SetBool("attacking", isAttacking);
    }

    private void SetAttacking(bool value)
    {
        isAttacking = value;
        animator?.SetBool("attacking", isAttacking);
    }
}
