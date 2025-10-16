using UnityEngine;

public class FleetShipBehavior : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 180f;
    public float attackRange = 5f;
    public float fireRate = 1f;
    public GameObject projectilePrefab;
    public Transform firePoint;
    private Transform followTarget;
    private Fleet fleetController;
    private float fireCooldown = 0f;

    public void SetFollowTarget(Transform target)
    {
        followTarget = target;
    }

    public void SetFleetController(Fleet controller)
    {
        fleetController = controller;
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
            Vector3 shootDirection = (nearestEnemy.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, angle), rotationSpeed * Time.deltaTime);
            Shoot(nearestEnemy.transform.position);
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

    private void Shoot(Vector3 targetPos)
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.velocity = (targetPos - firePoint.position).normalized * 10f;
    }

    private void OnDestroy()
    {
        if (fleetController != null)
        {
            fleetController.RespawnShip(gameObject);
        }
    }
}
