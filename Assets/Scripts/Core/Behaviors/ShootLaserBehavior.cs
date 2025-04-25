using System.Collections.Generic;
using UnityEngine;

public class ShootLaserBehavior : EnemyBehaviorBase
{
    /** Smìr støelby. */
    private Vector3 shootDirection;

    [SerializeField] private float defaultRayDistance = 10f;
    [SerializeField] private List<string> damageableTags;
    [SerializeField] private float damagePerSecond;

    [SerializeField] private LayerMask hitLayers;

    [Header("Pathfinding")]
    [SerializeField] private LayerMask blockingVisionLayers;
    private List<PathNode> currentPath;
    private int pathIndex = 0;

    [SerializeField] private Transform laserFirePoint;
    private LineRenderer lineRenderer;
    public SpaceEntity owner;

    private void Awake()
    {
        if (laserFirePoint == null) laserFirePoint = transform;

        if (lineRenderer == null) lineRenderer = gameObject.AddComponent<LineRenderer>();

        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.5f;
        lineRenderer.enabled = false;
    }

    public override void Execute()
    {
        if (target == null)
        {
            DisableLaser();
            return;
        }

        float distance = Vector3.Distance(transform.position, target.position);
        bool hasSight = HasLineOfSight();

        if (!hasSight)
        {
            DisableLaser();
            FollowPathToTarget();
            return;
        }

        AimAtTarget();

        if (distance > shipStats.AttackRadius)
        {
            Vector3 moveDir = (target.position - transform.position).normalized;
            DisableLaser();
            ChaseTarget(moveDir);
        }
        else
        {
            TryShootLaser();
            currentPath = null;
        }
    }

    private bool HasLineOfSight()
    {
        Vector2 directionToTarget = (target.position - transform.position).normalized;
        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, blockingVisionLayers);

        return hit.collider != null && hit.collider.CompareTag("Player");
    }

    private void FollowPathToTarget()
    {
        Pathfinding pathfinding = PathfindingManager.Instance.pathfinding;
        pathfinding.GetGrid().GetXY(transform.position, out int startX, out int startY);
        pathfinding.GetGrid().GetXY(target.position, out int endX, out int endY);

        if (currentPath == null || pathIndex >= currentPath.Count)
        {
            currentPath = pathfinding.FindPath(startX, startY, endX, endY);
            pathIndex = 0;

            if (currentPath == null || currentPath.Count == 0)
            {
                shootDirection = Vector3.zero;
                return;
            }
        }

        if (currentPath != null && pathIndex < currentPath.Count)
        {
            float cellSize = pathfinding.GetGrid().GetCellSize();
            Vector3 nodePosition = new Vector3(currentPath[pathIndex].x, currentPath[pathIndex].y) * cellSize + Vector3.one * cellSize / 2;
            shootDirection = (nodePosition - transform.position).normalized;

            if (shootDirection.magnitude > 0.01f)
            {
                transform.position += shootDirection * shipStats.Speed * Time.deltaTime;
                float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle - 90);
            }

            if (Vector3.Distance(transform.position, nodePosition) < 0.1f)
            {
                pathIndex++;
            }
        }
        else
        {
            shootDirection = Vector3.zero;
        }
    }

    private void AimAtTarget()
    {
        shootDirection = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    private bool IsTargetInRange()
    {
        return Vector3.Distance(transform.position, target.position) <= shipStats.AttackRadius;
    }

    private void TryShootLaser()
    {
        Vector2 startPosition = laserFirePoint.position;
        RaycastHit2D hit = Physics2D.Raycast(startPosition, laserFirePoint.up, defaultRayDistance, hitLayers);

        if (hit.collider != null)
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

        DrawLaserRay(startPosition, endPosition);
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

    private void DrawLaserRay(Vector2 startPosition, Vector2 endPosition)
    {
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);
    }

    private void ChaseTarget(Vector3 direction)
    {
        transform.position += direction * shipStats.Speed * Time.deltaTime;
    }
}
