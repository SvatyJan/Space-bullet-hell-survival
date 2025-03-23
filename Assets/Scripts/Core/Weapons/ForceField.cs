using UnityEngine;

public class ForceField : MonoBehaviour, IWeapon
{
    public float damage = 5f;
    public float cooldownTime = 5f;
    public float activationTime = 1f;

    private ShipStats shipStats;
    private Collider2D collider2D;
    private SpriteRenderer spriteRenderer;
    private bool isActive = false;
    private bool isGrowing = false;
    private float cooldownTimer = 0f;
    private float activationTimer = 0f;

    /** Počáteční velikost štítu. */
    private Vector3 initialScale; 

    private void Awake()
    {
        shipStats = GetComponentInParent<ShipStats>();
        collider2D = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        collider2D.enabled = false;
        spriteRenderer.enabled = false;
        initialScale = transform.localScale;
    }

    private void Update()
    {
        float shipVelocity = shipStats.Velocity.magnitude;

        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
            return;
        }


        if (!isActive)
        {
            ActivateShield();
        }

        if (isGrowing)
        {
            activationTimer += Time.deltaTime;
            float lerpFactor = Mathf.Clamp01(activationTimer / activationTime);

            transform.localScale = Vector3.Lerp(initialScale * 0.2f, initialScale, lerpFactor);

            if (lerpFactor >= 1f)
            {
                isGrowing = false;
            }
        }
    }

    private void ActivateShield()
    {
        isActive = true;
        collider2D.enabled = true;
        spriteRenderer.enabled = true;
        isGrowing = true;
        activationTimer = 0f;
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
        damage += 2f; // Zvýšení poškození
        cooldownTime -= 0.5f;
    }

    public void Evolve()
    {
        transform.localScale *= 1.5f; // Zvýšení velikosti štítu
    }
}
