using UnityEngine;
public class Projectile : MonoBehaviour
{
    public float speed = 10f;  // Rychlost st�ely
    private Vector3 direction; // Sm�r pohybu st�ely

    // Nastaven� sm�ru pohybu st�ely
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
        // Zni�en� st�ely po 5 sekund�ch, pokud je pot�eba
        Destroy(gameObject, 5f);
    }
}