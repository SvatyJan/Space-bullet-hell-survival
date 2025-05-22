using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackBehavior : EnemyBehaviorBase
{
    /** Sm�r. */
    private Vector3 direction;

    /** �as posledn�ho �toku. */
    private float lastAttackTime = 0f;

    [Header("Pathfinding")]
    /** LayerMask, kter� ignorujeme. */
    [SerializeField] private LayerMask blockingVisionLayers;

    /** Aktu�ln� cesta. */
    private List<PathNode> currentPath;

    /** Index akut�ln� cesty. */
    private int pathIndex = 0;

    /** Interval �asu hled�n� cesty. */
    [SerializeField] private float pathCooldown = 0.5f;

    /** Pomocn� prom�nn� pro ulo�en� �asu naposledy hled�n� cesty. */
    private float lastPathTime = 0f;

    /** P��znak zda �ek� na cestu �i nikoliv. */
    private bool waitingForPath = false;

    public override void Execute()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        // Pokud je hr�� mimo dosah, nebo je v dosahu ale nen� vid�t � n�sleduj ho
        if (distance > shipStats.DetectionRadius || !HasLineOfSight())
        {
            FollowPathToTarget();
        }
        else
        {
            // Je bl�zko a vid�m ho � �to�
            RotateTowardsTarget();
            MoveForward();
            AttackPlayerInRange();
            currentPath = null;
        }
    }

    /** Vrac� p��znak, zda m� v�hled na c�l nebo ne. */
    private bool HasLineOfSight()
    {
        Vector2 directionToTarget = (target.position - transform.position).normalized;
        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, blockingVisionLayers);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Player"))
            {
                // �ist� v�hled
                return true;
            }
            else
            {
                // n�co jin�ho v cest�
                return false;
            }
        }

        return false;
    }

    /** Hled� cestu k c�li. */
    private void FollowPathToTarget()
    {
        if (waitingForPath || (currentPath != null && pathIndex < currentPath.Count))
        {
            MoveAlongPath();
            return;
        }

        if (Time.time - lastPathTime < pathCooldown)
            return;

        Pathfinding pathfinding = PathfindingManager.Instance.pathfinding;
        pathfinding.GetGrid().GetXY(transform.position, out int startX, out int startY);
        pathfinding.GetGrid().GetXY(target.position, out int endX, out int endY);

        var endNode = pathfinding.GetGrid().GetGridObject(endX, endY);
        if (endNode == null || !endNode.isWalkable)
        {
            direction = Vector3.zero;
            return;
        }

        waitingForPath = true;
        lastPathTime = Time.time;

        PathfindingManager.Instance.RequestPath(startX, startY, endX, endY, OnPathFound);
    }

    private void MoveAlongPath()
    {
        if (currentPath == null || pathIndex >= currentPath.Count)
        {
            direction = Vector3.zero;
            return;
        }

        float cellSize = PathfindingManager.Instance.pathfinding.GetGrid().GetCellSize();
        Vector3 nodePosition = new Vector3(currentPath[pathIndex].x, currentPath[pathIndex].y) * cellSize + Vector3.one * cellSize / 2;
        direction = (nodePosition - transform.position).normalized;

        if (direction.magnitude > 0.01f)
        {
            transform.position += direction * shipStats.Speed * Time.deltaTime;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
        }

        if (Vector3.Distance(transform.position, nodePosition) < 0.1f)
        {
            pathIndex++;
        }
    }

    private void OnPathFound(List<PathNode> path)
    {
        waitingForPath = false;

        if (path == null || path.Count == 0)
        {
            direction = Vector3.zero;
            currentPath = null;
            return;
        }

        currentPath = path;
        pathIndex = 0;
    }

    /** Ot��� se sm�rem c�li. */
    private void RotateTowardsTarget()
    {
        direction = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
    }

    /** Pohybuje se sm�rem dop�edu. */
    private void MoveForward()
    {
        transform.position += direction * shipStats.Speed * Time.deltaTime;
    }

    /** �to�� na hr��e pokud je v dosahu. */
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

    private void OnDrawGizmos()
    {
        if (shootingPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(shootingPoint.position, shipStats.DetectionRadius);
        }
    }
}