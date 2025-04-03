using System.Collections.Generic;
using UnityEngine;

public class ShootLaserBehavior : EnemyBehaviorBase
{
    /** Sm�r st�elby. */
    private Vector3 shootDirection;

    [SerializeField] private float defaultRayDistance = 10f;
    [SerializeField] private List<string> damageableTags;
    [SerializeField] private float damagePerSecond;

    [SerializeField] private LayerMask hitLayers;
    [SerializeField] private Transform laserFirePoint;
    private LineRenderer lineRenderer;
    public SpaceEntity owner;

    private void Awake()
    {
        if(laserFirePoint == null)
        {
            laserFirePoint = transform;
        }

        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.5f;
        lineRenderer.enabled = false;
    }

    public override void Execute()
    {
        if (target == null)
        {
            DisableLaser();
            return;
        }

        AimAtTarget();

        if (IsTargetInRange())
        {
            TryShootLaser();
        }
        else
        {
            DisableLaser();
            ChaseTarget(shootDirection);
        }
    }

    /** Zam��� se na hr��e. */
    private void AimAtTarget()
    {
        shootDirection = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    /** Vr�t� `true`, pokud je c�l v dosahu �toku. */
    private bool IsTargetInRange()
    {
        return Vector3.Distance(transform.position, target.position) <= shipStats.AttackRadius;
    }

    /** Pokus� se vyst�elit laser. */
    private void TryShootLaser()
    {
        Vector2 startPosition = laserFirePoint.position;
        RaycastHit2D hit = Physics2D.Raycast(startPosition, laserFirePoint.up, defaultRayDistance, hitLayers);

        if (hit.collider != null)
        {
            EnableLaser(startPosition, hit.point);
            DealDamage(hit.collider);
        }
        else
        {
            DisableLaser();
        }
    }

    /** Aktivuje a vykresl� laserov� paprsek. */
    private void EnableLaser(Vector2 startPosition, Vector2 endPosition)
    {
        if (!lineRenderer.enabled)
            lineRenderer.enabled = true;

        DrawLaserRay(startPosition, endPosition);
    }

    /** Vypne laser, pokud nen� aktivn�. */
    private void DisableLaser()
    {
        if (lineRenderer.enabled)
            lineRenderer.enabled = false;
    }

    /** Ud�l� po�kozen� c�li, pokud je platn�. */
    private void DealDamage(Collider2D collider)
    {
        SpaceEntity target = collider.GetComponent<SpaceEntity>();

        if (target != null)
        {
            target.TakeDamage(damagePerSecond * Time.deltaTime);

            // Pokud je c�l mrtv�, vypne laser
            if (target.GetComponent<ShipStats>().CurrentHealth <= 0)
            {
                DisableLaser();
            }
        }
    }


    private void DrawLaserRay(Vector2 startPosition, Vector2 endPosition)
    {
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);
    }

    /** Pohybuje s entitou sm�rem k targetu. */
    private void ChaseTarget(Vector3 direction)
    {
        transform.position += direction * shipStats.Speed * Time.deltaTime;
    }
}
