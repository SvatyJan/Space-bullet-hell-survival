using UnityEngine;

public class FleetShipBehavior : MonoBehaviour
{
    public float speed = 5f;
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
        MoveToFollowPoint();
        AttackNearestEnemy();
    }

    private void MoveToFollowPoint()
    {
        if (followTarget == null) return;
        transform.position = Vector3.Lerp(transform.position, followTarget.position, speed * Time.deltaTime);
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