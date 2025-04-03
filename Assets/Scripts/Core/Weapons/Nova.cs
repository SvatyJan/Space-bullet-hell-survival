using UnityEngine;

public class Nova : MonoBehaviour, IWeapon
{
    /** Prefab v�buchu Novy. */
    [SerializeField] private GameObject novaExplosionPrefab;

    /** Interval mezi v�buchy. */
    [SerializeField] private float fireRate = 60f;

    /** �as pro p��t� aktivaci. */
    [SerializeField] private float nextFireTime = 0f;

    /** Po�kozen� zbran�. */
    private float baseDamage;

    /** Odkaz na vlastn�ka zbran�. */
    public SpaceEntity owner;

    private void Start()
    {
        owner = GetComponentInParent<SpaceEntity>();
        baseDamage = owner.GetComponent<ShipStats>().BaseDamage;

        nextFireTime = Time.time;
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
        fireRate = Mathf.Max(8f, fireRate - 8f);
        baseDamage += 10f;
    }

    public void Evolve()
    {
        fireRate = 30f;
    }
}
