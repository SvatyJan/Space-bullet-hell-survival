using UnityEngine;

public class ElectricWall : MonoBehaviour
{
    [SerializeField] private ParticleSystem collisionSparkEffect;
    [SerializeField] private float cooldownDuration = 0.75f;
    [SerializeField] private float cooldown = 0f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        if (Time.time < cooldown)
            return;

        ParticleSystem effectInstance = Instantiate(collisionSparkEffect, collision.transform.position, Quaternion.identity);
        Destroy(effectInstance.gameObject, 1f);

        GameUIEffects.ShowDamageBlur(0.75f);

        cooldown = Time.time + cooldownDuration;
    }
}
