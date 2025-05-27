using System.Collections;
using UnityEngine;

public class BioWeaponEffect : MonoBehaviour
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

    public void ApplyBioWeaponEffect(SpaceEntity enemy, SpaceEntity ownerOfProjectile)
    {
        owner = ownerOfProjectile;
        target = enemy;

        baseProjectileCount = BioLauncher.baseProjectileCount;

        StartCoroutine(PoisonDamageRoutine());
    }

    private IEnumerator PoisonDamageRoutine()
    {
        float elapsed = 0f;

        while (elapsed < bioWeaponEffectDuration && target != null)
        {
            ShipStats stats = target.GetComponent<ShipStats>();
            if (stats == null) break;

            if (stats.CurrentHealth <= 0)
            {
                Explode();
                break;
            }

            target.TakeDamage(bioWeaponEffectDamage);
            elapsed += tickInterval;
            yield return new WaitForSeconds(tickInterval);
        }

        Destroy(gameObject);
    }

    public void Explode()
    {
        for (int i = 0; i < baseProjectileCount; i++)
        {
            float angle = (360f / baseProjectileCount) * i;
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            GameObject proj = Instantiate(bioProjectilePrefab, transform.position, Quaternion.identity);
            BioProjectile projScript = proj.GetComponent<BioProjectile>();

            if (projScript != null)
            {
                float damage = owner.GetComponent<ShipStats>().BaseDamage;
                projScript.Initialize(owner, damage);
                projScript.SetDirection(direction);
            }
        }
    }
}
