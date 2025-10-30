using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour, IWeapon
{
    [SerializeField] private float defaultRayDistance = 10f;
    [SerializeField] private Transform laserFirePoint;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private LayerMask hitLayers;
    [SerializeField] private List<string> damageableTags;
    [SerializeField] private float damagePerSecond = 5f;
    [SerializeField] private float waveAmplitude = 0.5f;
    [SerializeField] private float waveFrequency = 2f;

    [SerializeField] private float segmentTargetRadius = 2f;
    [SerializeField] private float segmentBendStrength = 0.5f;

    public SpaceEntity owner;

    private bool evolved = false;

    private void Awake()
    {
        if (lineRenderer == null)
            lineRenderer = gameObject.AddComponent<LineRenderer>();

        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.5f;
        lineRenderer.enabled = false;
    }

    private void Start()
    {
        owner = GetComponentInParent<SpaceEntity>();
        if (owner == null)
        {
            Debug.LogError("Laser: Nelze najít vlastníka zbranì! Zbraò nebude fungovat.");
            return;
        }

        if (laserFirePoint == null)
            laserFirePoint = transform;
    }

    public void Fire()
    {
        if (!lineRenderer.enabled)
            lineRenderer.enabled = true;

        if (evolved)
            FireCurvedLaser();
        else
            FireStraightLaser();
    }

    private void FireStraightLaser()
    {
        Vector2 start = laserFirePoint.position;
        Vector2 direction = laserFirePoint.up;
        Vector2 end = start + direction * defaultRayDistance;

        RaycastHit2D hit = Physics2D.Raycast(start, direction, defaultRayDistance, hitLayers);

        if (hit.collider != null)
        {
            end = hit.point;

            if (damageableTags.Contains(hit.collider.tag))
            {
                SpaceEntity target = hit.collider.GetComponent<SpaceEntity>();
                if (target != null)
                {
                    target.TakeDamage((damagePerSecond + owner.GetComponent<ShipStats>().BaseDamage) * Time.deltaTime);

                    if (target.GetComponent<ShipStats>().CurrentHealth <= 0)
                        lineRenderer.enabled = false;
                }
            }
        }
        else
        {
            lineRenderer.enabled = false;
        }

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

    private void FireCurvedLaser()
    {
        Vector3 start = laserFirePoint.position;
        Vector3 dir = laserFirePoint.up;

        int segmentCount = Mathf.CeilToInt(defaultRayDistance);
        lineRenderer.positionCount = segmentCount;

        // Uchováme si, které cíle byly už použity
        HashSet<Collider2D> usedTargets = new HashSet<Collider2D>();

        for (int i = 0; i < segmentCount; i++)
        {
            float t = i / (float)(segmentCount - 1);
            Vector3 point = start + dir * defaultRayDistance * t;

            // Hledáme cíle v okolí segmentu
            Collider2D[] targets = Physics2D.OverlapCircleAll(point, segmentTargetRadius, hitLayers);

            Transform closest = null;
            float closestDist = float.MaxValue;

            foreach (var col in targets)
            {
                if (damageableTags.Contains(col.tag) && !usedTargets.Contains(col))
                {
                    float dist = Vector2.Distance(point, col.transform.position);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closest = col.transform;
                    }
                }
            }

            if (closest != null)
            {
                // Ohneme segment smìrem k cíli
                Vector3 toTarget = (closest.position - point).normalized;
                point += toTarget * segmentBendStrength;
                usedTargets.Add(closest.GetComponent<Collider2D>());
            }

            lineRenderer.SetPosition(i, point);

            // Poškození mezi segmenty
            if (i > 0)
            {
                Vector3 prev = lineRenderer.GetPosition(i - 1);
                RaycastHit2D hit = Physics2D.Linecast(prev, point, hitLayers);
                if (hit.collider != null && damageableTags.Contains(hit.collider.tag))
                {
                    SpaceEntity target = hit.collider.GetComponent<SpaceEntity>();
                    if (target != null)
                    {
                        target.TakeDamage((damagePerSecond + owner.GetComponent<ShipStats>().BaseDamage) * Time.deltaTime);
                    }
                }
            }
        }
    }

    public void Upgrade()
    {
        damagePerSecond += 1f;
        defaultRayDistance += 1f;
    }

    public void Downgrade()
    {
        damagePerSecond -= 1f;
        defaultRayDistance -= 1f;
    }

    public void Evolve()
    {
        damagePerSecond += 5f;
        defaultRayDistance += 5f;
        evolved = true;
    }
}
