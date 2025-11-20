using UnityEngine;
using UnityEngine.Pool;

public class SpitterBehavior : EnemyBehaviorBase
{
    [Header("Shooting")]
    public GameObject projectilePrefab;

    private float nextFireTime = 0f;
    private Animator animator;
    private Rigidbody2D rb;
    private bool isAttacking = false;

    [Header("Object pooling")]
    private ObjectPool<GameObject> ProjectilePool;

    protected override void Start()
    {
        base.Start();
        CreateProjectilePool();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public override void Execute()
    {
        UpdateAnimatorParameters();

        if (target == null)
        {
            SetAttacking(false);
            return;
        }

        float distance = Vector3.Distance(transform.position, target.position);

        if (!HasLineOfSight())
        {
            SetAttacking(false);
            FollowPathToTarget();
            return;
        }

        RotateTowardsTarget();

        if (distance > shipStats.AttackRadius)
        {
            SetAttacking(false);
            ChaseTarget();
        }
        else
        {
            SetAttacking(true);
            ActWhenTargetReached();
            currentPath = null;
        }
    }

    protected override void ActWhenTargetReached()
    {
        AttackPlayerInRange();
    }

    private void UpdateAnimatorParameters()
    {
        float speed = rb != null ? rb.linearVelocity.magnitude : 0f;
        animator?.SetFloat("speed", speed);
        animator?.SetBool("attacking", isAttacking);
    }

    private void SetAttacking(bool value)
    {
        isAttacking = value;
        animator?.SetBool("attacking", isAttacking);
    }

    private void RotateTowardsTarget()
    {
        direction = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    private void AttackPlayerInRange()
    {
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + shipStats.FireRate;
            ShootProjectile(direction);
        }
    }

    private void ShootProjectile(Vector3 dir)
    {
        GameObject projectileInstance = Instantiate(projectilePrefab, shootingPoint.position, shootingPoint.rotation);
        SpitterProjectile projectileScript = projectileInstance.GetComponent<SpitterProjectile>();
        if (projectileScript != null)
        {
            projectileScript.Initialize(this, spaceEntity, shipStats.BaseDamage);
            projectileScript.SetDirection(dir);
        }
    }

    private void ChaseTarget()
    {
        Vector3 moveDir = (target.position - transform.position).normalized;
        transform.position += moveDir * shipStats.Speed * Time.deltaTime;
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
