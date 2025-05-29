using UnityEngine;

public class Nova : MonoBehaviour, IWeapon
{
    [Header("Prefabs")]
    /** Prefab v�buchu Novy. */
    [SerializeField] private GameObject novaExplosionPrefab;

    [Header("Timing")]
    /** Interval mezi v�buchy. */
    [SerializeField] private float baseFireRate = 60f;

    /** �as pro p��t� aktivaci. */
    [SerializeField] private float nextFireTime = 0f;

    [Header("Attributes")]
    /** Z�kladn� po�kozen� zbran�. */
    [SerializeField] private float baseDamage = 10f;

    [Header("References")]
    /** Odkaz na vlastn�ka zbran�. */
    private SpaceEntity owner;

    /** Odkaz na statistiky vlastn�ka. */
    private ShipStats shipStats;

    /** Odkaz na PlayerProgression pro p�id�v�n� XP. */
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
