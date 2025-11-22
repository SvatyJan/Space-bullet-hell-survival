using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BioProjectile : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float projectileDuration = 5f;
    [SerializeField] private GameObject bioWeaponEffectPrefab;
    [SerializeField] private List<string> collisionTags;

    [Header("Hit Safety")]
    [SerializeField] private bool useFrameLock = true;

    private Vector3 direction;
    private float projectileDamage;
    private SpaceEntity owner;

    private bool hitHandled;
    private int lastHitFrame = -1;

    /** Potøebujeme referenci zbranì pro object pooling. */
    private IWeapon weapon;

    private void Start()
    {
        StartCoroutine(AutoRelease());
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    public void Initialize(IWeapon weapon, SpaceEntity owner, float damage)
    {
        this.weapon = weapon;
        this.owner = owner;
        this.projectileDamage = damage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hitHandled) return;
        if (useFrameLock && lastHitFrame == Time.frameCount) return;
        lastHitFrame = Time.frameCount;

        if (collisionTags != null && collisionTags.Count > 0 && !collisionTags.Contains(other.tag)) return;

        SpaceEntity target = other.GetComponentInParent<SpaceEntity>();

        if (target != null && target != owner)
        {
            hitHandled = true;
            ApplyDamageAndEffect(target);
        }

        weapon.ReleaseProjectileFromPool(this.gameObject);
    }

    private void ApplyDamageAndEffect(SpaceEntity target)
    {
        float? critChance = owner?.GetComponent<ShipStats>()?.CriticalChance;
        target.TakeDamage(projectileDamage, critChance);

        if (bioWeaponEffectPrefab == null) return;

        GameObject effectObj = EffectPoolManager.Instance.Get(bioWeaponEffectPrefab, target.transform.position);
        // Toto zamezuje klamání instancování parenta. Effekt se bude v hierarchii ukazovat pod nepøítelem, ale není jeho skuteèný parent!
        effectObj.transform.SetParent(null);

        BioWeaponEffect effect = effectObj.GetComponent<BioWeaponEffect>();
        if (effect != null)
        {
            effect.StartEffect(target, owner);
        }
    }

    private IEnumerator AutoRelease()
    {
        yield return new WaitForSeconds(projectileDuration);
        weapon.ReleaseProjectileFromPool(gameObject);
    }
}
