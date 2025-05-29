using UnityEngine;

public class Nova : MonoBehaviour, IWeapon
{
    [Header("Prefabs")]
    /** Prefab výbuchu Novy. */
    [SerializeField] private GameObject novaExplosionPrefab;

    [Header("Timing")]
    /** Interval mezi výbuchy. */
    [SerializeField] private float baseFireRate = 60f;

    /** Èas pro pøíští aktivaci. */
    [SerializeField] private float nextFireTime = 0f;

    [Header("Attributes")]
    /** Základní poškození zbranì. */
    [SerializeField] private float baseDamage = 10f;

    [Header("References")]
    /** Odkaz na vlastníka zbranì. */
    private SpaceEntity owner;

    /** Odkaz na statistiky vlastníka. */
    private ShipStats shipStats;

    /** Odkaz na PlayerProgression pro pøidávání XP. */
    private PlayerProgression playerProgression;

    private void Start()
    {
        owner = GetComponentInParent<SpaceEntity>();
        shipStats = owner.GetComponent<ShipStats>();
        baseDamage += shipStats.BaseDamage;
        playerProgression = GetComponentInParent<PlayerProgression>();

        nextFireTime = Time.time;
    }

    public void Fire()
    {
        if (Time.time >= nextFireTime)
        {
            float cooldown = baseFireRate * shipStats.FireRate;
            nextFireTime = Time.time + Mathf.Max(2f, cooldown);

            GameObject explosion = Instantiate(novaExplosionPrefab, owner.transform.position, Quaternion.identity);
            NovaExplosion novaExplosion = explosion.GetComponent<NovaExplosion>();

            if (novaExplosion != null)
            {
                float damage = baseDamage + shipStats.BaseDamage;
                float crit = shipStats.CriticalChance;
                novaExplosion.Initialize(owner, damage, crit, playerProgression);
            }
        }
    }

    public void Upgrade()
    {
        baseFireRate = Mathf.Max(8f, baseFireRate - 8f);
        baseDamage += 10f;
    }

    public void Evolve()
    {
        baseFireRate = 30f;
    }
}
