using UnityEngine;

public class XPOrb : MonoBehaviour
{
    /** Hodnota XP, kterou orb poskytuje. */
    public float xpAmount = 1f;

    /** Pokud se dotkne hráèe, pøidáme XP a znièíme orb. */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerProgression>().AddXP(xpAmount);
            Destroy(gameObject);
        }
    }
}