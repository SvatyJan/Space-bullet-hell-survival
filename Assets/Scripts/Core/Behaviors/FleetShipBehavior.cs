using UnityEngine;
using UnityEngine.Pool;

public class FleetShipBehavior : MonoBehaviour
{
    [SerializeField] private float aimAngleOffset = 0f;
    [SerializeField] private float projectileAngleOffset = 0f;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] public float speed = 5f;
    [SerializeField] public float rotationSpeed = 180f;
    [SerializeField] public float attackRange = 5f;
    [SerializeField] public float fireRate = 1f;
    [SerializeField] public GameObject projectilePrefab;
    [SerializeField] public Transform firePoint;

    private Transform followTarget;
    private Fleet fleetController;
    private float fireCooldown = 0f;

    [Header("Object pooling")]
    private ObjectPool<GameObject> ProjectilePool;

    public void SetFollowTarget(Transform target)
    {
        followTarget = target;
    }

    public void SetFleetController(Fleet controller)
    {
        fleetController = controller;
    }

    private void Start()
    {
        CreateProjectilePool();
    }

    private void Update()
    {
        Execute();
    }

    public void Execute()
    {
        MoveToFollowPoint();
        AttackNearestEnemy();
    }

    private void MoveToFollowPoint()
    {
        if (followTarget == null) return;
        Vector3 direction = (followTarget.position - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, followTarget.position, speed * Time.deltaTime);
        if (direction.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            float newAngle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, angle, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, 0, newAngle);
        }
    }

    private void AttackNearestEnemy()
    {
        if (fireCooldown > 0)
        {
            fireCooldown -= Time.deltaTime;
            return;
        }

        GameObject nearestEnemy = FindNearestEnemy();
        if (nearestEnemy != null)
        {
            Vector2 dir = ((Vector2)nearestEnemy.transform.position - (Vector2)(firePoint ? firePoint.position : transform.position)).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion targetRot = Quaternion.Euler(0f, 0f, angle + aimAngleOffset);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);

            Shoot(dir, angle);
            fireCooldown = 1f / fireRate;
        }
    }

    private GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float minDist = attackRange;
        foreach (GameObject enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = enemy;
            }
        }
        return closest;
    }

    private void Shoot(Vector2 dir, float baseAngleDeg)
    {
        if (projectilePrefab == null || firePoint == null) return;

        Quaternion rot = Quaternion.Euler(0f, 0f, baseAngleDeg + projectileAngleOffset);
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, rot);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = dir.normalized * bulletSpeed;
    }

    private void OnDestroy()
    {
        if (fleetController != null)
        {
            fleetController.RespawnShip(gameObject);
        }
    }

    private void CreateProjectilePool()
    {
        ProjectilePool = new ObjectPool<GameObject>(
            () => { return Instantiate(projectilePrefab); },
        projectile => { projectile.gameObject.SetActive(true); },
        projectile => { projectile.gameObject.SetActive(false); },
        projectile => { Destroy(projectile); },
        false, 10, 20
        );
    }

    public void ReleaseProjectileFromPool(GameObject projectile)
    {
        ProjectilePool.Release(projectile);
    }
}
