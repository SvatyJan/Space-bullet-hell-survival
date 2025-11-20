using System.Collections.Generic;
using UnityEngine;

public class NovaExplosion : MonoBehaviour
{
    [Header("Explosion Settings")]
    /** Maximální velikost exploze. */
    [SerializeField] private float maxSize = 15f;

    /** Rychlost, jakou se exploze rozšiøuje. */
    [SerializeField] private float expansionSpeed = 10f;

    /** Základní poškození zpùsobené explozí. */
    [SerializeField] private float baseDamage = 20f;

    [Header("XP Attraction")]
    /** Polomìr, ve kterém exploze pøitahuje XP orby. */
    [SerializeField] private float xpAttractRadius = 3f;

    /** Rychlost, jakou se XP orby pøibližují k explozi. */
    [SerializeField] private float xpPullSpeed = 5f;

    [Header("Runtime")]
    /** Vypoètené výsledné poškození. */
    private float damage;

    /** Kritická šance pøenesená ze ShipStats. */
    private float criticalChance;

    /** Odkaz na vlastníka exploze (hráè nebo entita). */
    private SpaceEntity owner;

    [Header("Collision")]
    /** Tagy entit, na které mùže exploze pùsobit. */
    [SerializeField] private List<string> collisionTags;

    [Header("References")]
    /** Odkaz na PlayerProgression pro pøidávání XP. */
    private PlayerProgression playerProgression;


    public void Initialize(SpaceEntity owner, float totalDamage, float critChance, PlayerProgression playerProgression)
    {
        this.owner = owner;
        this.damage = totalDamage;
        this.criticalChance = critChance;
        this.playerProgression = playerProgression;
        transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        if (transform.localScale.x < maxSize)
        {
            float scaleIncrease = expansionSpeed * Time.deltaTime;
            transform.localScale += new Vector3(scaleIncrease, scaleIncrease, 0);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("XP"))
        {
            XPOrb xpOrb = collision.GetComponent<XPOrb>();
            if (xpOrb != null && owner != null)
            {
                ShipStats shipStats = owner.GetComponent<ShipStats>();
                if (shipStats != null)
                {
                    playerProgression.AddXP(xpOrb.xpAmount);
                    Destroy(collision.gameObject);
                }
            }
            return;
        }

        if (collisionTags.Contains(collision.tag))
        {
            SpaceEntity enemy = collision.GetComponent<SpaceEntity>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, criticalChance);
            }
            else
            {
                Destroy(collision.gameObject);
            }
        }
    }

}
