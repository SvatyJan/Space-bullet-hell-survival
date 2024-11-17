using UnityEngine;
using System.Collections.Generic;
using System.Data;
using System;

public class Projectile : MonoBehaviour
{
    /** Odkaz na staty hr��e nebo nep��tele, kter� projekt vyst�elil. */
    private ShipStats shipStats;

    /** Odkaz na hr��e nebo nep��tele, kter� projektil vyst�elil. */
    public SpaceEntity owner;

    /** Rychlost st�ely. */
    [SerializeField] public float speed = 10f;

    /** Seznam tag�, se kter�mi projektil m��e kolidovat. */
    [SerializeField] private List<string> collisionTags;

    /** Po�kozen� projektilu. */
    [SerializeField] private float projectileDamage = 10f;

    /** Doba, jak dlouho vydr�� projektil ne� se zni��. */
    [SerializeField] private float projectileDuration = 5f;

    /** Sm�r pohybu st�ely. */
    private Vector3 direction;

    private void Start()
    {
        Destroy(gameObject, projectileDuration);
    }

    private void Update()
    {
        // Pohyb st�ely
        transform.position += direction * speed * Time.deltaTime;
    }

    // Nastaven� sm�ru pohybu st�ely
    public void SetDirection(Vector3 direction)
    {
        this.direction = direction.normalized;
    }

    // Nastaven� vlastnictv� a po�kozen�
    public void Initialize(SpaceEntity owner, float damage)
    {
        this.owner = owner;
        this.projectileDamage = projectileDamage + damage;
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