using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackBehavior : EnemyBehaviorBase
{
    private Vector3 direction;
    private float lastAttackTime = 0f;

    private List<PathNode> currentPath;
    private int pathIndex = 0;

    /** LayerMask, který ignorujeme. */
    [SerializeField] private LayerMask blockingVisionLayers;

    public override void Execute()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);
        bool canSeePlayer = HasLineOfSight();

        //Debug.Log("Distance too far: " + (distance > shipStats.DetectionRadius));
        //Debug.Log("Can see player: " + canSeePlayer);

        // Pokud je hráè mimo dosah, nebo je v dosahu ale není vidìt – následuj ho
        if (distance > shipStats.DetectionRadius || !canSeePlayer)
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
            Debug.Log($"Raycast hit: {hit.collider.name}");

            if (hit.collider.CompareTag("Player"))
            {
                return true; // èistý výhled
            }
            else
            {
                return false; // nìco jiného v cestì
            }
        }

        return false;
    }




    /** Hledá cestu k cíli. */
    private void FollowPathToTarget()
    {
        Pathfinding pathfinding = PathfindingManager.Instance.pathfinding;
        pathfinding.GetGrid().GetXY(transform.position, out int startX, out int startY);
        pathfinding.GetGrid().GetXY(target.position, out int endX, out int endY);

        //Debug.Log($"Start Node: {startX},{startY} | End Node: {endX},{endY}");

        if (currentPath == null || pathIndex >= currentPath.Count)
        {
            currentPath = pathfinding.FindPath(startX, startY, endX, endY);
            pathIndex = 0;

            if (currentPath == null || currentPath.Count == 0)
            {
                direction = Vector3.zero;
                return;
            }
        }

        if (currentPath != null && pathIndex < currentPath.Count)
        {
            float cellSize = pathfinding.GetGrid().GetCellSize();
            Vector3 nodePosition = new Vector3(currentPath[pathIndex].x, currentPath[pathIndex].y) * cellSize + Vector3.one * cellSize/2;
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
        else
        {
            direction = Vector3.zero; // cesta skonèila
        }
    }



    /** Otáèí se smìrem targetu. */
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