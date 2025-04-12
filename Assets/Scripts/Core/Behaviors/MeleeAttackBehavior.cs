using UnityEngine;
using UnityEngine.AI;

public class MeleeAttackBehavior : EnemyBehaviorBase
{
    //private Vector3 direction;
    private NavMeshAgent agent;
    private float lastAttackTime = 0f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
            Debug.LogError("NavMeshAgent component missing on " + gameObject.name);

        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    public override void Execute()
    {
        if (target == null) return;
        //RotateTowardsTarget();
        MoveForward();
        AttackPlayerInRange();
    }

    /** Otáèí se smìrem targetu. */
    /*private void RotateTowardsTarget()
    {
        direction = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
    }*/

    /** Pohybuje se smìrem dopøedu. */
    private void MoveForward()
    {
        //transform.position += direction * shipStats.Speed * Time.deltaTime;
        if (agent.isOnNavMesh)
        {
            agent.SetDestination(target.position);
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} není na NavMesh! Nemohu použít SetDestination.");
        }

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

    private void OnDrawGizmosSelected()
    {
        if (shootingPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(shootingPoint.position, shipStats.AttackRadius);
        }
    }
}