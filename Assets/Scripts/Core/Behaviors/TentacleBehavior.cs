using System.Collections;
using UnityEngine;

public class TentacleBehavior : MonoBehaviour
{
    [Header("Attack")]
    public float attackCooldown = 3f;
    public float grabDuration = 1f;
    public int grabDamage = 20;
    public float throwForce = 6f;

    [Header("Grab Control")]
    [SerializeField] private GameObject tentacleHitBox;

    private Animator animator;
    private float lastAttackTime = -Mathf.Infinity;
    private static bool isPlayerGrabbed = false;

    private PlayerShip grabbedPlayer;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void TryAttack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            animator.SetTrigger("attacking");
        }
    }

    public void HandleGrab(Collider2D other)
    {
        if (isPlayerGrabbed || !other.CompareTag("Player")) return;
        if (grabbedPlayer != null) return;

        PlayerShip player = other.GetComponent<PlayerShip>();
        if (player == null) return;

        StartCoroutine(GrabAndThrowPlayer(player));
    }

    private IEnumerator GrabAndThrowPlayer(PlayerShip player)
    {
        isPlayerGrabbed = true;
        grabbedPlayer = player;

        // Znehybni hráèe
        player.DisableControlForDuration(grabDuration);
        player.TakeDamage(grabDamage);

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true; // zastaví fyziku bìhem držení
        }

        // 1) Po dobu držení drž pozici hráèe na hitboxu
        float timer = 0f;
        while (timer < grabDuration)
        {
            player.transform.position = tentacleHitBox.transform.position;
            timer += Time.deltaTime;
            yield return null;
        }

        // 2) Uvolni hráèe
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.velocity = Vector2.zero;

            Vector2 baseDir = transform.right;
            float angleDegrees = 270f;
            float angleRad = angleDegrees * Mathf.Deg2Rad;

            Vector2 rotatedDir = new Vector2(
                baseDir.x * Mathf.Cos(angleRad) - baseDir.y * Mathf.Sin(angleRad),
                baseDir.x * Mathf.Sin(angleRad) + baseDir.y * Mathf.Cos(angleRad)
            ).normalized;
            rb.AddForce(rotatedDir * throwForce, ForceMode2D.Force);

            // Doèasný drag zpomalení
            float originalDrag = rb.drag;
            rb.drag = 3f;

            yield return new WaitForSeconds(1.5f);

            rb.drag = originalDrag;
            rb.velocity = Vector2.zero;
        }

        yield return new WaitForSeconds(0.5f);
        isPlayerGrabbed = false;
        grabbedPlayer = null;
    }
}
