using UnityEngine;
using System.Collections.Generic;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;           // Rychlost st�ely
    private Vector3 direction;          // Sm�r pohybu st�ely
    public SpaceEntity owner;           // Odkaz na hr��e nebo nep��tele, kter� projektil vyst�elil
    [SerializeField]
    private List<string> collisionTags; // Seznam tag�, se kter�mi projektil m��e kolidovat

    private float projectileDamage;               // Po�kozen� projektilu

    // Nastaven� sm�ru pohybu st�ely
    public void SetDirection(Vector3 direction)
    {
        this.direction = direction.normalized;
    }

    // Nastaven� vlastnictv� a po�kozen�
    public void Initialize(SpaceEntity owner, float damage)
    {
        this.owner = owner;
        this.projectileDamage = damage;
    }

    private void Update()
    {
        // Pohyb st�ely
        transform.position += direction * speed * Time.deltaTime;
    }

    private void Start()
    {
        // Zni�en� st�ely po 5 sekund�ch
        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collisionTags.Contains(other.tag))
        {
            owner.TakeDamage(projectileDamage);
        }
    }
}