using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ShootBehavior : EnemyBehaviorBase
{
    /** Prefab projektilu. */
    public GameObject projectilePrefab;

    /** �as kdy m��e znovu vyst�elit. */
    private float nextFireTime = 0f;

    /** Sm�r st�elby. */
    private Vector3 direction;

    [Header("Pathfinding")]
    /** LayerMask, kter� ignorujeme. */
    [SerializeField] private LayerMask blockingVisionLayers;

    /** Aktu�ln� cesta. */
    private List<PathNode> currentPath;

    /** Index akut�ln� cesty. */
    private int pathIndex = 0;

    public override void Execute()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);
        bool hasSight = HasLineOfSight();

        if (!hasSight)
        {
            // Nevid�m hr��e � najdi cestu k n�mu
            FollowPathToTarget();
            return;
        }

        // Vid�m hr��e � nato� se na n�j
        RotateTowardsTarget();

        if (distance > shipStats.AttackRadius)
        {
            // Vid�m, ale nejsem dost bl�zko � p�ibli� se p��mo (ne pathfinding)
            Vector3 moveDirection = (target.position - transform.position).normalized;
            ChaseTarget(moveDirection);
        }
        else
        {
            // V dosahu a m�m v�hled � �to�
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
                direction = Vector3.zero;
                return;
            }
        }

        if (currentPath != null && pathIndex < currentPath.Count)
        {
            float cellSize = pathfinding.GetGrid().GetCellSize();
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
        else
        {
            direction = Vector3.zero;
        }
    }

    /** Ot��� se sm�rem c�li. */
    private void RotateTowardsTarget()
    {
        direction = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
    }

    /** �to�� na hr��e v dosahu. */
    private void AttackPlayerInRange()
    {
        if (Vector3.Distance(transform.position, target.position) <= shipStats.AttackRadius && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + shipStats.FireRate;
            ShootProjectile(direction);
        }
    }

    /** Vytvo�� projektil na pozici shootingPoint a nastav� jej� sm�r. */
    private void ShootProjectile(Vector3 direction)
    {
        GameObject projectileInstance = Instantiate(projectilePrefab, shootingPoint.position, shootingPoint.rotation);
        Projectile projectileScript = projectileInstance.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.Initialize(spaceEntity, shipStats.BaseDamage);
            projectileScript.SetDirection(direction);
        }
    }

    /** Pohybuje s entitou sm�rem k targetu. */
    private void ChaseTarget(Vector3 direction)
    {
        transform.position += direction * shipStats.Speed * Time.deltaTime;
    }
}