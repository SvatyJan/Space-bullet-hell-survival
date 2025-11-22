using UnityEngine;

public class BioWeaponEffect : StatusEffectBase
{
    [Header("Effect Settings")]
    public float bioWeaponEffectDamage = 2f;
    public float bioWeaponEffectDuration = 5f;
    public float tickInterval = 1f;

    [Header("Explosion Settings")]
    [SerializeField] private GameObject bioProjectilePrefab;

    private SpaceEntity owner;
    private SpaceEntity target;
    private int baseProjectileCount = 6;

    private float elapsedTime;
    private float tickTimer;
    private bool effectActive;

    private bool followTarget = false;

    public void StartEffect(SpaceEntity enemy, SpaceEntity ownerOfProjectile)
    {
        owner = ownerOfProjectile;
        target = enemy;
        baseProjectileCount = BioLauncher.baseProjectileCount;

        elapsedTime = 0f;
        tickTimer = 0f;
        effectActive = true;
        followTarget = true;
    }

    private void Update()
    {
        if (!effectActive || target == null) return;

        elapsedTime += Time.deltaTime;
        tickTimer += Time.deltaTime;

        if (followTarget)
        {
            transform.position = target.transform.position;
        }

        ShipStats stats = target.GetComponent<ShipStats>();
        if (stats == null || stats.CurrentHealth <= 0 || elapsedTime > bioWeaponEffectDuration)
        {
            EndEffect();
            return;
        }

        if (tickTimer >= tickInterval)
        {
            tickTimer = 0f;
            target.TakeDamage(bioWeaponEffectDamage);
        }
    }

    private void EndEffect()
    {
        effectActive = false;
        followTarget = false;
        Explode();
        EffectSpawnManager.Instance.NotifyEffectEnded();
        EffectPoolManager.Instance.Release(gameObject);

        owner = null;
        target = null;
    }

    public void Explode()
    {
        for (int i = 0; i < baseProjectileCount; i++)
        {
            float angle = (360f / baseProjectileCount) * i;
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            Quaternion rotation = Quaternion.LookRotation(Vector3.forward, direction);

            GameObject proj = Instantiate(bioProjectilePrefab, transform.position, rotation);
            BioProjectile projScript = proj.GetComponent<BioProjectile>();

            if (projScript != null)
            {
                float damage = owner.GetComponent<ShipStats>().BaseDamage;
                BioLauncher bioLauncher = owner.GetComponentInChildren<BioLauncher>();
                projScript.Initialize(bioLauncher, owner, damage);
                projScript.SetDirection(direction);
            }
        }
    }
}
