using UnityEngine;

public class Orb : MonoBehaviour
{
    public float orbitRadius = 5f;
    public float orbitSpeed = 180f;
    public float baseDamage = 5f;
    public Transform ship;

    private float angle;

    private void Update()
    {
        if (ship == null) return;

        angle += orbitSpeed * Time.deltaTime;
        angle %= 360f;

        float radians = angle * Mathf.Deg2Rad;
        float x = Mathf.Cos(radians) * orbitRadius;
        float y = Mathf.Sin(radians) * orbitRadius;

        transform.position = ship.position + new Vector3(x, y, 0);
    }

    public void Initialize(Transform ship, float orbitSpeed, float orbitRadius, float startAngle)
    {
        this.ship = ship;
        this.orbitSpeed = orbitSpeed;
        this.orbitRadius = orbitRadius;
        this.angle = startAngle;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy Projectile"))
        {
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("Enemy"))
        {
            SpaceEntity enemy = collision.GetComponent<SpaceEntity>();
            if (enemy != null)
            {
                enemy.TakeDamage(baseDamage);
            }
        }
    }
}
