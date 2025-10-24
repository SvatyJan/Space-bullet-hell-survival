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
            PlayerProgression playerXp = collision.GetComponent<PlayerProgression>();
            if(playerXp == null)
            {
                return;
            }

            // Pokud je hr�� ve f�zi level up, tak nem��eme p�idat dal�� XP kv�li race condition.
            if(playerXp.isPlayerLevelingUp == true)
            {
                return;
            }

            playerXp.AddXP(xpAmount);

            Destroy(gameObject);
        }
    }
}