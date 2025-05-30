using UnityEngine;

public class Orb : MonoBehaviour
{
    [Header("Orbit Settings")]
    /** Polomìr obìhu kolem lodi. */
    [SerializeField] private float orbitRadius = 5f;

    /** Rychlost otáèení kolem lodi (ve stupních za sekundu). */
    [SerializeField] private float orbitSpeed = 180f;

    /** Poèáteèní úhel orbity. */
    private float angle;

    /** Základní poškození zpùsobené kolizí. */
    [SerializeField] private float baseDamage = 5f;

    /** Referenèní odkaz na loï, kolem které orb obíhá. */
    private Transform ship;

    /** Pøíznak, zda je orb v evolvovaném stavu. */
    [SerializeField] private bool isEvolved = false;

    [Header("XP Attraction (only when evolved)")]
    /** Polomìr, ve kterém orb pøitahuje XP pøi evoluci. */
    [SerializeField] private float xpAttractRadius = 5f;

    /** Rychlost, jakou se XP pøitahují smìrem k orbu. */
    [SerializeField] private float xpPullSpeed = 5f;

    /** Odkaz na komponentu, která spravuje XP hráèe. */
    private PlayerProgression playerProgression;

    private void Update()
    {
        if (ship == null) return;

        RotateAroundCenter();

        if (isEvolved)
        {
            AttractXpOrbs();
        }
    }

    public void Initialize(Transform ship, float orbitSpeed, float orbitRadius, float startAngle, PlayerProgression playerProgression, bool evolved = false)
    {
        this.ship = ship;
        this.orbitSpeed = orbitSpeed;
        this.orbitRadius = orbitRadius;
        this.angle = startAngle;
        this.isEvolved = evolved;
        this.playerProgression = playerProgression;
    }

    private void RotateAroundCenter()
    {
        angle += orbitSpeed * Time.deltaTime;
        angle %= 360f;

        float radians = angle * Mathf.Deg2Rad;
        float x = Mathf.Cos(radians) * orbitRadius;
        float y = Mathf.Sin(radians) * orbitRadius;

        transform.position = ship.position + new Vector3(x, y, 0);
    }

    private void AttractXpOrbs()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, xpAttractRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("XP"))
            {
                Transform xp = hit.transform;
                xp.position = Vector3.MoveTowards(xp.position, transform.position, xpPullSpeed * Time.deltaTime);

                if (Vector3.Distance(xp.position, transform.position) < 0.3f)
                {
                    XPOrb xpOrb = xp.GetComponent<XPOrb>();
                    if (xpOrb != null)
                    {
                        float amount = xpOrb.xpAmount; 
                        playerProgression.AddXP(amount);
                        Destroy(xpOrb.gameObject);
                    }
                }
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy Projectile"))
        {
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("Enemy"))
        {
            SpaceEntity enemy = collision.GetComponent<SpaceEntity>();
            if (enemy != null)
            {
                enemy.TakeDamage(baseDamage);
            }
        }
    }
}
