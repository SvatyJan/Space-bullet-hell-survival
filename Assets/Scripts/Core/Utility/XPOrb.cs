using UnityEngine;

public class XPOrb : MonoBehaviour
{
    public float xpAmount = 1f; // Hodnota XP, kterou orb poskytuje
    private GameObject player;  // Odkaz na hr��e

    private void Start()
    {
        // Najdeme hr��e podle tagu
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (player == null) return;

        // Pokud je hr�� v attractionRadius, p�it�hneme XP k n�mu
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= player.GetComponent<ShipStats>().AttractionRadius)
        {
            Vector3 direction = (player.transform.position - transform.position).normalized;
            transform.position += direction * player.GetComponent<ShipStats>().AttractionSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Pokud se dotkne hr��e, p�id�me XP a zni��me orb
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerProgression>().AddXP(xpAmount);
            Destroy(gameObject);
        }
    }
}