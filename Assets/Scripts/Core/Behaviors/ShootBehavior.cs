using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ShootBehavior : EnemyBehaviorBase
{
    /** Prefab projektilu. */
    public GameObject projectilePrefab;

    /** Èas kdy mùže znovu vystøelit. */
    private float nextFireTime = 0f;

    /** Smìr støelby. */
    private Vector3 direction;

    [Header("Pathfinding")]
    /** LayerMask, který ignorujeme. */
    [SerializeField] private LayerMask blockingVisionLayers;

    /** Aktuální cesta. */
    private List<PathNode> currentPath;

    /** Index akutální cesty. */
    private int pathIndex = 0;

    public override void Execute()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);
        bool hasSight = HasLineOfSight();

        if (!hasSight)
        {
            // Nevidím hráèe – najdi cestu k nìmu
            FollowPathToTarget();
            return;
        }

        // Vidím hráèe – natoè se na nìj
        RotateTowardsTarget();

        if (distance > shipStats.AttackRadius)
        {
            // Vidím, ale nejsem dost blízko – pøibliž se pøímo (ne pathfinding)
            Vector3 moveDirection = (target.position - transform.position).normalized;
            ChaseTarget(moveDirection);
        }
        else
        {
            // V dosahu a mám výhled – útoè
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

    /** Otáèí se smìrem cíli. */
    private void RotateTowardsTarget()
    {
        direction = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
    }

    /** Útoèí na hráèe v dosahu. */
    private void AttackPlayerInRange()
    {
        if (Vector3.Distance(transform.position, target.position) <= shipStats.AttackRadius && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + shipStats.FireRate;
            ShootProjectile(direction);
        }
    }

    /** Vytvoøí projektil na pozici shootingPoint a nastaví její smìr. */
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

    /** Pohybuje s entitou smìrem k targetu. */
    private void ChaseTarget(Vector3 direction)
    {
        transform.position += direction * shipStats.Speed * Time.deltaTime;
    }
}