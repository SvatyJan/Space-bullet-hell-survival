using UnityEngine;

public class Nova : MonoBehaviour, IWeapon
{
    /** Prefab v�buchu Novy. */
    public GameObject novaExplosionPrefab;

    /** Interval mezi v�buchy. */
    public float fireRate = 60f;

    /** �as pro p��t� aktivaci. */
    private float nextFireTime = 0f;

    /** Po�kozen� zbran�. */
    private float baseDamage;

    /** Odkaz na vlastn�ka zbran�. */
    public SpaceEntity owner;

    private void Start()
    {
        owner = GetComponentInParent<SpaceEntity>();
        baseDamage = owner.GetComponent<ShipStats>().BaseDamage;
    }

    public void Fire()
    {
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;

            GameObject explosion = Instantiate(novaExplosionPrefab, owner.transform.position, Quaternion.identity);
            NovaExplosion novaExplosion = explosion.GetComponent<NovaExplosion>();

            if (novaExplosion != null)
            {
                novaExplosion.Initialize(owner.GetComponent<ShipStats>().BaseDamage);
            }
        }
    }

    public void Upgrade()
    {
        fireRate = Mathf.Max(8f, fireRate - 8f); // Sni�uje cooldown, ale minim�ln� na 8 sekund
        baseDamage += 10f;
    }

    public void Evolve()
    {
        fireRate = 30f; // Evolve sni�uje cooldown na 30s
    }
}
