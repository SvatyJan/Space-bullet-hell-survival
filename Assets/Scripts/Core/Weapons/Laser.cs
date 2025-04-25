using System;
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

    public SpaceEntity owner;

    private void Awake()
    {
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

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
        {
            laserFirePoint = transform;
        }
    }

    public void Fire()
    {
        Vector2 startPosition = laserFirePoint.position;
        Vector2 endPosition = startPosition + (Vector2)laserFirePoint.up * defaultRayDistance;

        RaycastHit2D hit = Physics2D.Raycast(startPosition, laserFirePoint.up, defaultRayDistance, hitLayers);

        if (hit.collider != null && damageableTags.Contains(hit.collider.tag))
        {
            endPosition = hit.point;

            if (!lineRenderer.enabled)
                lineRenderer.enabled = true;

            DrawLaserRay(startPosition, endPosition);

            SpaceEntity target = hit.collider.GetComponent<SpaceEntity>();
            if (target != null)
            {
                target.TakeDamage((damagePerSecond + owner.GetComponent<ShipStats>().BaseDamage) * Time.deltaTime);

                if (target.GetComponent<ShipStats>().CurrentHealth <= 0)
                {
                    lineRenderer.enabled = false;
                }
            }
            else
            {
                lineRenderer.enabled = false;
            }
        }
        else
        {
            if (lineRenderer.enabled)
                lineRenderer.enabled = false;
        }
    }

    private void DrawLaserRay(Vector2 startPosition, Vector2 endPosition)
    {
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);
    }

    public void Upgrade()
    {
        damagePerSecond += 1f;
        defaultRayDistance += 1f;
    }
    public void Evolve()
    {
        damagePerSecond += 5f;
        defaultRayDistance += 5f;
    }
}
