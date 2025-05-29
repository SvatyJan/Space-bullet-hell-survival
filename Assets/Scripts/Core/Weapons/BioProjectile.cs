using UnityEngine;
using System.Collections.Generic;

public class BioProjectile : MonoBehaviour
{
    [Header("Config")]
    /** Rychlost st�ely. */
    [SerializeField] private float speed = 10f;

    /** Doba, jak dlouho vydr�� projektil ne� se zni��. */
    [SerializeField] private float projectileDuration = 5f;

    /** Prefab efektu bio zbran�. */
    [SerializeField] private GameObject bioWeaponEffectPrefab;

    /** Seznam tag�, se kter�mi projektil m��e kolidovat. */
    [SerializeField] private List<string> collisionTags;

    [Header("Runtime")]
    /** Sm�r pohybu st�ely. */
    private Vector3 direction;

    /** Po�kozen� projektilu. */
    private float projectileDamage;

    /** Odkaz na hr��e nebo nep��tele, kter� projektil vyst�elil. */
    private SpaceEntity owner;

    private void Start()
    {
        Destroy(gameObject, projectileDuration);
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    public void Initialize(SpaceEntity owner, float damage)
    {
        this.owner = owner;
        this.projectileDamage = damage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!collisionTags.Contains(other.tag)) return;

        SpaceEntity target = other.GetComponent<SpaceEntity>();
        if (target == null || target == owner) return;

        ApplyDamageAndEffect(target);
        Destroy(gameObject);
    }

    private void ApplyDamageAndEffect(SpaceEntity target)
    {
        float? critChance = owner?.GetComponent<ShipStats>()?.CriticalChance;
        target.TakeDamage(projectileDamage, critChance);

        if (bioWeaponEffectPrefab != null)
        {
            GameObject effect = Instantiate(bioWeaponEffectPrefab, target.transform.position, Quaternion.identity, target.transform);

            BioWeaponEffect effectScript = effect.GetComponent<BioWeaponEffect>();
            if (effectScript != null)
            {
                effectScript.ApplyBioWeaponEffect(target, owner);
            }
        }
    }
}
