using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackBehavior : EnemyBehaviorBase
{
    /** Smìr. */
    private Vector3 direction;

    /** Èas posledního útoku. */
    private float lastAttackTime = 0f;

    [Header("Pathfinding")]
    /** LayerMask, který ignorujeme. */
    [SerializeField] private LayerMask blockingVisionLayers;

    /** Aktuální cesta. */
    private List<PathNode> currentPath;

    /** Index akutální cesty. */
    private int pathIndex = 0;

    /** Interval èasu hledání cesty. */
    [SerializeField] private float pathCooldown = 0.5f;

    /** Pomocná promìnná pro uložení èasu naposledy hledání cesty. */
    private float lastPathTime = 0f;

    /** Pøíznak zda èeká na cestu èi nikoliv. */
    private bool waitingForPath = false;

    public override void Execute()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        // Pokud je hráè mimo dosah, nebo je v dosahu ale není vidìt – následuj ho
        if (distance > shipStats.DetectionRadius || !HasLineOfSight())
        {
            FollowPathToTarget();
        }
        else
        {
            // Je blízko a vidím ho – útoè
            RotateTowardsTarget();
            MoveForward();
            AttackPlayerInRange();
            currentPath = null;
        }
    }

    /** Vrací pøíznak, zda má výhled na cíl nebo ne. */
    private bool HasLineOfSight()
    {
        Vector2 directionToTarget = (target.position - transform.position).normalized;
        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, blockingVisionLayers);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Player"))
            {
                // èistý výhled
                return true;
            }
            else
            {
                // nìco jiného v cestì
                return false;
            }
        }

        return false;
    }

    /** Hledá cestu k cíli. */
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

    /** Otáèí se smìrem cíli. */
    private void RotateTowardsTarget()
    {
        direction = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
    }

    /** Pohybuje se smìrem dopøedu. */
    private void MoveForward()
    {
        transform.position += direction * shipStats.Speed * Time.deltaTime;
    }

    /** Útoèí na hráèe pokud je v dosahu. */
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