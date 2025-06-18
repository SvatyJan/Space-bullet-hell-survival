using UnityEngine;

public class TentacleGrabHitbox : MonoBehaviour
{
    private TentacleBehavior parentTentacle;

    private void Awake()
    {
        parentTentacle = GetComponentInParent<TentacleBehavior>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        parentTentacle?.HandleGrab(other);
    }
}
