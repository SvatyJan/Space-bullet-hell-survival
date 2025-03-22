using System.Collections;
using UnityEngine;

public class BioWeaponEffect : MonoBehaviour
{
    public float bioWeaponEffectDamage = 2f;
    public float bioWeaponEffectDuration = 5f;
    public float tickInterval = 1f;

    /** Odkaz na hráèe nebo nepøítele, který projektil vystøelil. */
    private SpaceEntity owner;

    /** Prefab projektilù po explozi. */
    [SerializeField] private GameObject bioProjectilePrefab;

    private SpaceEntity target;

    public void ApplyBioWeaponEffect(SpaceEntity enemy, SpaceEntity ownerOfProjectile)
    {
        owner = ownerOfProjectile;
        target = enemy;
        StartCoroutine(PoisonDamageRoutine());
    }

    private IEnumerator PoisonDamageRoutine()
    {
        float elapsed = 0f;
        while (elapsed < bioWeaponEffectDuration && target != null)
        {
            if (target.GetComponent<ShipStats>().CurrentHealth <= 0) break;

            target.TakeDamage(bioWeaponEffectDamage);
            elapsed += tickInterval;
            yield return new WaitForSeconds(tickInterval);
        }
        Destroy(gameObject);
    }

    public void Explode()
    {
        int projectileCount = 5;
        float spreadAngle = 360f;

        for (int i = 0; i < projectileCount; i++)
        {
            float angle = Random.Range(0f, spreadAngle);
            Vector3 newDirection = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);

            GameObject newProjectile = Instantiate(bioProjectilePrefab, transform.position, Quaternion.identity);
            BioProjectile newProjectileScript = newProjectile.GetComponent<BioProjectile>();

            if (newProjectileScript != null)
            {
                newProjectileScript.SetDirection(newDirection);
                newProjectileScript.Initialize(owner, owner.GetComponent<ShipStats>().BaseDamage);
            }
        }
    }
}
