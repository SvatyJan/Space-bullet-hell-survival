using UnityEngine;

public class WallBreaker : MonoBehaviour, IWeapon
{
    public float activationVelocity = 5f;
    public float damage = 5f;
    public float cooldownTime = 2f;

    private ShipStats shipStats;
    private Collider2D collider2D;
    private SpriteRenderer spriteRenderer;
    private bool isActive = false;
    private float cooldownTimer = 0f;

    private void Awake()
    {
        shipStats = GetComponentInParent<ShipStats>();
        collider2D = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        collider2D.enabled = false;
        spriteRenderer.enabled = false;
    }

    private void Update()
    {
        float shipVelocity = shipStats.Velocity.magnitude;

        if (shipVelocity <= activationVelocity)
        {
            DeactivateShield();
        }

        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
            return;
        }


        if (!isActive && shipVelocity >= activationVelocity)
        {
            ActivateShield();
        }
    }

    private void ActivateShield()
    {
        isActive = true;
        collider2D.enabled = true;
        spriteRenderer.enabled = true;
    }

    private void DeactivateShield()
    {
        isActive = false;
        collider2D.enabled = false;
        spriteRenderer.enabled = false;
    }

    private void DeactivateShieldAndAddCooldown()
    {
        isActive = false;
        collider2D.enabled = false;
        spriteRenderer.enabled = false;
        cooldownTimer = cooldownTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"WallBreaker collided with {collision.gameObject.name}");

        if (isActive && collision.CompareTag("Enemy Projectile"))
        {
            DeactivateShieldAndAddCooldown();
            Destroy(collision.gameObject);
            Debug.Log("WallBreaker destroyed an enemy projectile.");
        }

        if (isActive && collision.CompareTag("Enemy"))
        {
            SpaceEntity enemy = collision.GetComponent<SpaceEntity>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log($"WallBreaker hit {enemy.gameObject.name} for {damage} damage.");
            }
            DeactivateShieldAndAddCooldown();
        }
    }


    public void Fire()
    {
        // Není potřeba manuální aktivace
    }

    public void Upgrade()
    {
        activationVelocity -= 0.5f; // Snížení požadované rychlosti pro aktivaci
        damage += 2f; // Zvýšení poškození
    }

    public void Evolve()
    {
        transform.localScale *= 1.5f; // Zvýšení velikosti štítu
    }
}
