using System.Collections.Generic;
using UnityEngine;

public class EyeBehavior : EnemyBehaviorBase
{
    [Header("Laser Settings")]
    [SerializeField] private float defaultRayDistance = 10f;
    [SerializeField] private float damagePerSecond = 10f;
    [SerializeField] private List<string> damageableTags;
    [SerializeField] private LayerMask hitLayers;
    [SerializeField] private Transform laserFirePoint;

    private Rigidbody2D rb;
    private LineRenderer lineRenderer;
    private Animator animator;

    private bool isAttacking = false;

    protected override void Start()
    {
        base.Start();

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (laserFirePoint == null)
            laserFirePoint = transform;

        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.5f;
        lineRenderer.enabled = false;
    }

    public override void Execute()
    {
        if (target == null)
        {
            SetState(false);
            return;
        }

        AimAtTarget();
        UpdateAnimatorParameters();

        float distance = Vector3.Distance(transform.position, target.position);

        if (!HasLineOfSight())
        {
            SetState(false);
            FollowPathToTarget();
            return;
        }

        if (distance > shipStats.AttackRadius)
        {
            SetState(false);
            ChaseTarget();
        }
        else
        {
            SetState(true);
            ActWhenTargetReached();
            currentPath = null;
        }
    }

    protected override void ActWhenTargetReached()
    {
        TryShootLaser();
    }

    private void SetState(bool attacking)
    {
        isAttacking = attacking;
        animator?.SetBool("attacking", isAttacking);

        if (!isAttacking)
            DisableLaser();
    }

    private void UpdateAnimatorParameters()
    {
        animator?.SetFloat("speed", rb.velocity.magnitude);
    }

    private void AimAtTarget()
    {
        direction = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    private void ChaseTarget()
    {
        Vector3 moveDir = (target.position - transform.position).normalized;
        transform.position += moveDir * shipStats.Speed * Time.deltaTime;
    }

    private void TryShootLaser()
    {
        Vector2 startPosition = laserFirePoint.position;
        RaycastHit2D hit = Physics2D.Raycast(startPosition, laserFirePoint.up, defaultRayDistance, hitLayers);

        if (hit.collider != null && damageableTags.Contains(hit.collider.tag))
        {
            EnableLaser(startPosition, hit.point);
            DealDamage(hit.collider);
        }
        else
        {
            DisableLaser();
        }
    }

    private void EnableLaser(Vector2 start, Vector2 end)
    {
        if (!lineRenderer.enabled)
            lineRenderer.enabled = true;

        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

    private void DisableLaser()
    {
        if (lineRenderer.enabled)
            lineRenderer.enabled = false;
    }

    private void DealDamage(Collider2D hitCollider)
    {
        SpaceEntity targetEntity = hitCollider.GetComponent<SpaceEntity>();
        if (targetEntity == null) return;

        targetEntity.TakeDamage(damagePerSecond * Time.deltaTime);

        if (targetEntity.GetComponent<ShipStats>().CurrentHealth <= 0)
            DisableLaser();
    }
}
