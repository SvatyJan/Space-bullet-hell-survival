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
            Debug.LogError("Laser: Nelze naj�t vlastn�ka zbran�! Zbra� nebude fungovat.");
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

        // Pokud je z�sah a c�l je po�kozovateln�
        if (hit.collider != null && damageableTags.Contains(hit.collider.tag))
        {
            endPosition = hit.point;

            // Aktivujeme laser pouze pokud n�co tref�me
            if (!lineRenderer.enabled)
                lineRenderer.enabled = true;

            DrawLaserRay(startPosition, endPosition);

            SpaceEntity target = hit.collider.GetComponent<SpaceEntity>();
            if (target != null)
            {
                target.TakeDamage((damagePerSecond + owner.GetComponent<ShipStats>().BaseDamage) * Time.deltaTime);

                // Pokud nep��tel je zni�en, deaktivujeme laser
                if (target.GetComponent<ShipStats>().CurrentHealth <= 0)
                {
                    lineRenderer.enabled = false;
                }
            }
        }
        else
        {
            // Pokud nic netref�me, laser se skryje
            if (lineRenderer.enabled)
                lineRenderer.enabled = false;
        }
    }

    private void DrawLaserRay(Vector2 startPosition, Vector2 endPosition)
    {
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);
    }

    public void Upgrade() { throw new System.NotImplementedException(); }
    public void Evolve() { throw new System.NotImplementedException(); }
}
