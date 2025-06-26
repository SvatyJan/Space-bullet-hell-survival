using UnityEngine;

public class ForceField : MonoBehaviour, IWeapon
{
    [Header("Attributes")]
    /** Základní poškození, které štít způsobí při kontaktu s nepřítelem. */
    [SerializeField] private float baseDamage = 5f;

    /** Základní doba, po kterou je štít neaktivní po zásahu. */
    [SerializeField] private float cooldownTime = 5f;

    /** Čas potřebný pro plnou aktivaci štítu. */
    [SerializeField] private float activationTime = 1f;

    [Header("Runtime")]
    /** Časovač cooldownu mezi jednotlivými aktivacemi. */
    private float cooldownTimer = 0f;

    /** Časovač růstu štítu při aktivaci. */
    private float activationTimer = 0f;

    /** Označuje, zda je štít právě aktivní. */
    private bool isActive = false;

    /** Označuje, zda se štít právě animuje (roste). */
    private bool isGrowing = false;

    [Header("References")]
    /** Odkaz na komponentu se statistikami hráče. */
    private ShipStats shipStats;

    /** Collider štítu. */
    private Collider2D collider2D;

    /** Renderer štítu. */
    private SpriteRenderer spriteRenderer;

    /** Výchozí velikost štítu pro výpočet animace růstu. */
    private Vector3 initialScale;

    private Animator animator;

    private void Awake()
    {
        shipStats = GetComponentInParent<ShipStats>();
        collider2D = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        collider2D.enabled = false;
        spriteRenderer.enabled = false;
        initialScale = transform.localScale;
    }

    private void Update()
    {
        float adjustedCooldown = cooldownTime * shipStats.FireRate;

        if (cooldownTimer > 0f)
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

    private void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
    }

    private void ActivateShield()
    {
        isActive = true;
        collider2D.enabled = true;
        spriteRenderer.enabled = true;
        isGrowing = true;
        activationTimer = 0f;
        animator.SetTrigger("activate");
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
        cooldownTimer = cooldownTime * shipStats.FireRate;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActive) return;

        if (collision.CompareTag("Enemy Projectile"))
        {
            Destroy(collision.gameObject);
            animator.SetTrigger("destroy");
            DeactivateShieldAndAddCooldown();
        }
        else if (collision.CompareTag("Enemy"))
        {
            SpaceEntity enemy = collision.GetComponent<SpaceEntity>();
            if (enemy != null)
            {
                float totalDamage = baseDamage + shipStats.BaseDamage;
                float critChance = shipStats.CriticalChance;
                enemy.TakeDamage(totalDamage, critChance);
                animator.SetTrigger("destroy");
                DeactivateShieldAndAddCooldown();
            }
        }
    }

    public void Fire()
    {
        // Štít je automatický – Fire se zde nevolá.
    }

    public void Upgrade()
    {
        baseDamage += 5f;
        cooldownTime -= 0.5f;
        initialScale *= 1.1f;
        ReinitializeShield();
    }

    public void Evolve()
    {
        // Zvětší výchozí velikost štítu a ihned ji aplikuje
        initialScale *= 1.5f;
        ReinitializeShield();
    }

    private void ReinitializeShield()
    {
        if (isActive)
        {
            // Restartuje animaci růstu a aplikuje novou velikost
            isGrowing = true;
            activationTimer = 0f;
            transform.localScale = initialScale * 0.2f;
        }
        else
        {
            transform.localScale = initialScale;
        }
    }
}
