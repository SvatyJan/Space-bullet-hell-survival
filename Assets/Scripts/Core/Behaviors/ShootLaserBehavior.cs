using System.Collections.Generic;
using UnityEngine;

public class ShootLaserBehavior : EnemyBehaviorBase
{
    [Header("Laser Settings")]
    [SerializeField] private float defaultRayDistance = 10f;
    [SerializeField] private float damagePerSecond;
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

        if (laserFirePoint == null) laserFirePoint = transform;

        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
            lineRenderer = gameObject.AddComponent<LineRenderer>();

        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.5f;
        lineRenderer.enabled = false;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public override void Execute()
    {
        // Nastavení parametrù pro Animator
        float currentSpeed = rb.velocity.magnitude;
        animator.SetFloat("speed", currentSpeed);
        animator.SetBool("attacking", isAttacking);

        if (target == null)
        {
            isAttacking = false;
            DisableLaser();
            return;
        }

        float distance = Vector3.Distance(transform.position, target.position);
        bool hasSight = HasLineOfSight();

        if (!hasSight)
        {
            isAttacking = false;
            DisableLaser();
            FollowPathToTarget(); // base implementation
            return;
        }

        AimAtTarget();

        if (distance > shipStats.AttackRadius)
        {
            isAttacking = false;
            DisableLaser();
            ChaseTarget();
        }
        else
        {
            isAttacking = true;
            ActWhenTargetReached();
            currentPath = null;
        }
    }

    protected override void ActWhenTargetReached()
    {
        TryShootLaser();
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

    private void EnableLaser(Vector2 startPosition, Vector2 endPosition)
    {
        if (!lineRenderer.enabled)
            lineRenderer.enabled = true;

        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);
    }

    private void DisableLaser()
    {
        if (lineRenderer.enabled)
            lineRenderer.enabled = false;
    }

    private void DealDamage(Collider2D collider)
    {
        SpaceEntity target = collider.GetComponent<SpaceEntity>();
        if (target != null)
        {
            target.TakeDamage(damagePerSecond * Time.deltaTime);

            if (target.GetComponent<ShipStats>().CurrentHealth <= 0)
            {
                DisableLaser();
            }
        }
    }
}
