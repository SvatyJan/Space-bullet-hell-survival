using UnityEngine;

public class Nova : MonoBehaviour, IWeapon
{
    /** Prefab výbuchu Novy. */
    public GameObject novaExplosionPrefab;

    /** Interval mezi výbuchy. */
    public float fireRate = 60f;

    /** Èas pro pøíští aktivaci. */
    private float nextFireTime = 0f;

    /** Poškození zbranì. */
    private float baseDamage;

    /** Odkaz na vlastníka zbranì. */
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
        fireRate = Mathf.Max(8f, fireRate - 8f); // Snižuje cooldown, ale minimálnì na 8 sekund
        baseDamage += 10f;
    }

    public void Evolve()
    {
        fireRate = 30f; // Evolve snižuje cooldown na 30s
    }
}
