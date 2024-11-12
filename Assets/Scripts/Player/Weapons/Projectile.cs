using UnityEngine;
public class Projectile : MonoBehaviour
{
    public float speed = 10f;  // Rychlost støely
    private Vector3 direction; // Smìr pohybu støely

    // Nastavení smìru pohybu støely
    public void SetDirection(Vector3 direction)
    {
        this.direction = direction.normalized;
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void Start()
    {
        // Znièení støely po 5 sekundách, pokud je potøeba
        Destroy(gameObject, 5f);
    }
}