using UnityEngine;
using System.Collections.Generic;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;           // Rychlost st�ely
    private Vector3 direction;          // Sm�r pohybu st�ely
    public SpaceEntity owner;           // Odkaz na hr��e nebo nep��tele, kter� projektil vyst�elil
    [SerializeField]
    private List<string> collisionTags; // Seznam tag�, se kter�mi projektil m��e kolidovat

    private float projectileDamage;     // Po�kozen� projektilu

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
        // Zkontroluj, zda tag objektu je v seznamu povolen�ch koliz�
        if (collisionTags.Contains(other.tag))
        {
            // Najdi komponentu SpaceEntity na objektu, se kter�m jsme kolidovali
            SpaceEntity target = other.GetComponent<SpaceEntity>();
            if (target != null && target != owner) // Zajist�me, �e nezran�me vlastn�ka
            {
                target.TakeDamage(projectileDamage); // Ud�l�me po�kozen� c�li
                Destroy(gameObject); // Zni��me st�elu po z�sahu
            }
        }
    }
}