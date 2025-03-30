using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Rocket : MonoBehaviour
{
    public float speed = 15f;
    public float maxDistance = 10f;
    public float explosionRadius = 2f;
    private Vector3 direction;
    public SpaceEntity owner;
    [SerializeField] private List<string> collisionTags;
    private float rocketDamage;
    private Vector3 startPosition;

    public void SetDirection(Vector3 direction)
    {
        this.direction = direction.normalized;
    }

    public void Initialize(SpaceEntity owner, float damage)
    {
        this.owner = owner;
        this.rocketDamage = damage;
        this.startPosition = transform.position;
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
        {
            Explode();
        }
    }

    private void Start()
    {
        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collisionTags.Contains(other.tag))
        {
            SpaceEntity target = other.GetComponent<SpaceEntity>();
            if (target != null && target != owner)
            {
                Explode();
            }
        }
    }

    private void Explode()
    {
        Vector3 explosionPosition = transform.position;

        Collider2D[] hits = Physics2D.OverlapCircleAll(explosionPosition, explosionRadius);

        foreach (Collider2D hit in hits)
        {
            SpaceEntity target = hit.GetComponent<SpaceEntity>();
            if (target != null && target != owner)
            {
                target.TakeDamage(rocketDamage);
            }
        }

        StartCoroutine(DrawExplosionIndicator(explosionPosition));

        Destroy(gameObject);
    }

    private IEnumerator DrawExplosionIndicator(Vector3 explosionPosition)
    {
        GameObject explosionEffect = new GameObject("ExplosionIndicator");
        explosionEffect.transform.position = explosionPosition;

        LineRenderer lineRenderer = explosionEffect.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.loop = true;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.useWorldSpace = true;

        int segments = 50;
        float angleStep = 360f / segments;
        Vector3[] positions = new Vector3[segments];

        for (int i = 0; i < segments; i++)
        {
            float angle = Mathf.Deg2Rad * i * angleStep;
            positions[i] = explosionPosition + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * explosionRadius;
        }

        lineRenderer.positionCount = segments;
        lineRenderer.SetPositions(positions);

        Destroy(explosionEffect, 1f);
        yield return new WaitForSeconds(1f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);
        Gizmos.DrawSphere(transform.position, explosionRadius);
    }
}
