using UnityEngine;

public class XPOrb : MonoBehaviour
{
    /** Hodnota XP, kterou orb poskytuje. */
    public float xpAmount = 1f;

    /** Pokud se dotkne hr��e, p�id�me XP a zni��me orb. */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerProgression>().AddXP(xpAmount);
            Destroy(gameObject);
        }
    }
}