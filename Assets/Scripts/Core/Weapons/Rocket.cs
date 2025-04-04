using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Rocket : MonoBehaviour
{
    public float speed = 5f;
    public float maxDistance = 10f;
    public float explosionRadius = 2f;
    public float rotateSpeed = 200f;

    private GameObject target;
    public SpaceEntity owner;
    [SerializeField] private List<string> collisionTags;
    private float rocketDamage;
    private Vector3 startPosition;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        Destroy(gameObject, 5f);
    }

    public void SetTarget(GameObject target)
    {
        this.target = target;
    }

    public void Initialize(SpaceEntity owner, float damage)
    {
        this.owner = owner;
        this.rocketDamage = damage;
    }

    private void FixedUpdate()
    {
        if (target == null) return;

        Vector2 direction = ((Vector2)target.transform.position - rb.position).normalized;

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        float angle = Mathf.MoveTowardsAngle(rb.rotation, targetAngle, rotateSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(angle);

        rb.velocity = transform.up * speed;

        if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
        {
            Explode();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collisionTags.Contains(other.tag))
        {
            SpaceEntity hitEntity = other.GetComponent<SpaceEntity>();
            if (hitEntity != null && hitEntity != owner)
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
            SpaceEntity hitEntity = hit.GetComponent<SpaceEntity>();
            if (hitEntity != null && hitEntity != owner)
            {
                hitEntity.TakeDamage(rocketDamage);
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
