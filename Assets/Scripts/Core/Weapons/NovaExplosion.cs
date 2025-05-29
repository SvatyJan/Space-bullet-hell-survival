using System.Collections.Generic;
using UnityEngine;

public class NovaExplosion : MonoBehaviour
{
    [Header("Explosion Settings")]
    /** Maxim�ln� velikost exploze. */
    [SerializeField] private float maxSize = 15f;

    /** Rychlost, jakou se exploze roz�i�uje. */
    [SerializeField] private float expansionSpeed = 10f;

    /** Z�kladn� po�kozen� zp�soben� exploz�. */
    [SerializeField] private float baseDamage = 20f;

    [Header("XP Attraction")]
    /** Polom�r, ve kter�m exploze p�itahuje XP orby. */
    [SerializeField] private float xpAttractRadius = 3f;

    /** Rychlost, jakou se XP orby p�ibli�uj� k explozi. */
    [SerializeField] private float xpPullSpeed = 5f;

    [Header("Runtime")]
    /** Vypo�ten� v�sledn� po�kozen�. */
    private float damage;

    /** Kritick� �ance p�enesen� ze ShipStats. */
    private float criticalChance;

    /** Odkaz na vlastn�ka exploze (hr�� nebo entita). */
    private SpaceEntity owner;

    [Header("Collision")]
    /** Tagy entit, na kter� m��e exploze p�sobit. */
    [SerializeField] private List<string> collisionTags;

    [Header("References")]
    /** Odkaz na PlayerProgression pro p�id�v�n� XP. */
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
