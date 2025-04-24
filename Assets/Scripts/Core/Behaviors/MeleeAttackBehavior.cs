using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackBehavior : EnemyBehaviorBase
{
    /** Sm�r. */
    private Vector3 direction;

    /** �as posledn�ho �toku. */
    private float lastAttackTime = 0f;

    /** Aktu�ln� cesta. */
    private List<PathNode> currentPath;

    /** Index akut�ln� cesty. */
    private int pathIndex = 0;

    /** LayerMask, kter� ignorujeme. */
    [SerializeField] private LayerMask blockingVisionLayers;

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
            // cesta skon�ila
            direction = Vector3.zero;
        }
    }

    /** Ot��� se sm�rem targetu. */
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