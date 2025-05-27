using UnityEngine;
using System.Collections.Generic;

public class BioProjectile : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float projectileDuration = 5f;
    [SerializeField] private GameObject bioWeaponEffectPrefab;
    [SerializeField] private List<string> collisionTags;

    [Header("Runtime")]
    private Vector3 direction;
    private float projectileDamage;
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
        target.TakeDamage(projectileDamage);

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
