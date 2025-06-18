using UnityEngine;

public class SpikeBehavior : MonoBehaviour
{
    private GuardianBehavior guardianBehavior;

    private void Awake()
    {
        guardianBehavior = GetComponentInParent<GuardianBehavior>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerShip player = other.GetComponent<PlayerShip>();
            if (player != null)
            {
                guardianBehavior?.OnSpikeHitPlayer(player);
            }
        }
    }
}
