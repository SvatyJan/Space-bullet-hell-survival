using UnityEngine;

public class XPOrb : MonoBehaviour
{
    public float xpAmount = 1f; // Hodnota XP, kterou orb poskytuje

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Pokud se dotkne hr��e, p�id�me XP a zni��me orb
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerProgression>().AddXP(xpAmount);
            ShipController.Instance.AddExperience(xpAmount);
            Destroy(gameObject);
        }
    }
}