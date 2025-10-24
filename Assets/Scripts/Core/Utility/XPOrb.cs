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
            PlayerProgression playerXp = collision.GetComponent<PlayerProgression>();
            if(playerXp == null)
            {
                return;
            }

            // Pokud je hráè ve fázi level up, tak nemùžeme pøidat další XP kvùli race condition.
            if(playerXp.isPlayerLevelingUp == true)
            {
                return;
            }

            playerXp.AddXP(xpAmount);

            Destroy(gameObject);
        }
    }
}