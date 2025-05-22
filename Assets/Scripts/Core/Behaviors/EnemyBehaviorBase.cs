using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBehaviorBase : MonoBehaviour
{
    protected ShipStats shipStats;
    protected Transform target;
    protected SpaceEntity spaceEntity;
    protected Transform shootingPoint;

    protected List<PathNode> currentPath;
    protected int pathIndex = 0;
    protected float lastPathTime = 0f;
    protected Vector3 lastTargetPosition;
    protected bool waitingForPath = false;

    [SerializeField] protected float pathCooldown = 0.5f;
    [SerializeField] protected LayerMask blockingVisionLayers;

    protected virtual void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player")?.transform;
        shootingPoint ??= transform;
        shipStats = GetComponent<ShipStats>();
        spaceEntity = GetComponent<SpaceEntity>();
    }

    public virtual void Execute()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > shipStats.DetectionRadius || !HasLineOfSight())
        {
            FollowPathToTarget();
        }
        else
        {
            currentPath = null;
            ActWhenTargetReached();
        }
    }

    protected virtual void ActWhenTargetReached()
    {
        // Defaultní chování – implementace v dědičných třídách
    }

    protected void FollowPathToTarget()
    {
        if (waitingForPath || (currentPath != null && pathIndex < currentPath.Count))
        {
            MoveAlongPath();
            return;
        }

        if (Time.time - lastPathTime < pathCooldown)
            return;

        var grid = PathfindingManager.Instance.pathfinding.GetGrid();
        grid.GetXY(transform.position, out int startX, out int startY);
        grid.GetXY(target.position, out int endX, out int endY);

        var endNode = grid.GetGridObject(endX, endY);
        if (endNode == null || !endNode.isWalkable)
            return;

        waitingForPath = true;
        lastPathTime = Time.time;
        lastTargetPosition = target.position;

        PathfindingManager.Instance.RequestPath(startX, startY, endX, endY, OnPathFound);
    }

    protected void OnPathFound(List<PathNode> path)
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

    protected Vector3 direction;

    protected void MoveAlongPath()
    {
        if (currentPath == null || pathIndex >= currentPath.Count)
            return;

        float cellSize = PathfindingManager.Instance.pathfinding.GetGrid().GetCellSize();
        Vector3 nodePosition = new Vector3(currentPath[pathIndex].x, currentPath[pathIndex].y) * cellSize + Vector3.one * cellSize / 2;
        direction = (nodePosition - transform.position).normalized;

        if (direction.magnitude > 0.01f)
        {
            transform.position += direction * shipStats.Speed * Time.deltaTime;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }

        if (Vector3.Distance(transform.position, nodePosition) < 0.1f)
        {
            pathIndex++;
        }
    }

    protected bool HasLineOfSight()
    {
        Vector2 dirToTarget = (target.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, target.position);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToTarget, distance, blockingVisionLayers);

        return hit.collider != null && hit.collider.CompareTag("Player");
    }
}
