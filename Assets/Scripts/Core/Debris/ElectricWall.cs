using UnityEngine;

public class ElectricWall : MonoBehaviour
{
    [SerializeField] private ParticleSystem collisionSparkEffect;
    [SerializeField] private float cooldownDuration = 1f;
    [SerializeField] private float cooldown = 0f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        if (Time.time < cooldown)
            return;

        var visualFeedbackEffect = Instantiate(collisionSparkEffect, collision.transform.position, Quaternion.identity);

        cooldown = Time.time + cooldownDuration;

        Destroy(visualFeedbackEffect, 1f);
    }
}
