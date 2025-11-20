using UnityEngine;
using System.Collections.Generic;

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
        if (hitHandled) return;
        if (useFrameLock && lastHitFrame == Time.frameCount) return;
        lastHitFrame = Time.frameCount;

        if (collisionTags != null && collisionTags.Count > 0 && !collisionTags.Contains(other.tag)) return;

        SpaceEntity target = other.GetComponentInParent<SpaceEntity>();
        if (target == null || target == owner) return;

        hitHandled = true;
        ApplyDamageAndEffect(target);
        Destroy(gameObject);
    }

    private void ApplyDamageAndEffect(SpaceEntity target)
    {
        float? critChance = owner?.GetComponent<ShipStats>()?.CriticalChance;
        target.TakeDamage(projectileDamage, critChance);

        if (bioWeaponEffectPrefab == null) return;

        var effect = Instantiate(bioWeaponEffectPrefab, target.transform.position, Quaternion.identity, target.transform);
        var effectScript = effect.GetComponent<BioWeaponEffect>();
        if (effectScript != null)
        {
            effectScript.ApplyBioWeaponEffect(target, owner);
        }
    }
}
