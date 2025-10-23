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
    private static bool isEntityGrabbed = false;

    private PlayerShip grabbedPlayer;
    private AllyShip grabbedAlly;

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
        if (isEntityGrabbed) return;

        if (other.CompareTag("Player"))
        {
            if (grabbedPlayer != null) return;
            PlayerShip player = other.GetComponent<PlayerShip>();
            if (player == null) return;
            StartCoroutine(GrabAndThrowPlayer(player));
            return;
        }

        if (other.CompareTag("Player Ally"))
        {
            if (grabbedAlly != null) return;
            AllyShip ally = other.GetComponent<AllyShip>();
            if (ally == null) ally = other.GetComponentInParent<AllyShip>();
            if (ally == null) return;
            StartCoroutine(GrabAndThrowAlly(ally));
            return;
        }
    }

    private IEnumerator GrabAndThrowPlayer(PlayerShip player)
    {
        isEntityGrabbed = true;
        grabbedPlayer = player;

        // Znehybni hr��e
        player.DisableControlForDuration(grabDuration);
        player.TakeDamage(grabDamage);

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true; // zastav� fyziku b�hem dr�en�
        }

        // 1) Po dobu dr�en� dr� pozici hr��e na hitboxu
        float timer = 0f;
        while (timer < grabDuration)
        {
            player.transform.position = tentacleHitBox.transform.position;
            timer += Time.deltaTime;
            yield return null;
        }

        // 2) Uvolni hr��e
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector2.zero;

            Vector2 baseDir = transform.right;
            float angleDegrees = 270f;
            float angleRad = angleDegrees * Mathf.Deg2Rad;

            Vector2 rotatedDir = new Vector2(
                baseDir.x * Mathf.Cos(angleRad) - baseDir.y * Mathf.Sin(angleRad),
                baseDir.x * Mathf.Sin(angleRad) + baseDir.y * Mathf.Cos(angleRad)
            ).normalized;
            rb.AddForce(rotatedDir * throwForce, ForceMode2D.Force);

            // Do�asn� drag zpomalen�
            float originalDrag = rb.linearDamping;
            rb.linearDamping = 3f;

            yield return new WaitForSeconds(1.5f);

            rb.linearDamping = originalDrag;
            rb.linearVelocity = Vector2.zero;
        }

        yield return new WaitForSeconds(0.5f);
        isEntityGrabbed = false;
        grabbedPlayer = null;
    }

    private IEnumerator GrabAndThrowAlly(AllyShip ally)
    {
        isEntityGrabbed = true;
        grabbedAlly = ally;

        ally.TakeDamage(grabDamage);

        Rigidbody2D rb = ally.GetComponent<Rigidbody2D>();
        FleetShipBehavior behavior = ally.GetComponent<FleetShipBehavior>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true;
        }
        if (behavior != null) behavior.enabled = false;

        float timer = 0f;
        while (timer < grabDuration)
        {
            ally.transform.position = tentacleHitBox.transform.position;
            timer += Time.deltaTime;
            yield return null;
        }

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector2.zero;

            Vector2 baseDir = transform.right;
            float angleDegrees = 270f;
            float angleRad = angleDegrees * Mathf.Deg2Rad;

            Vector2 rotatedDir = new Vector2(
                baseDir.x * Mathf.Cos(angleRad) - baseDir.y * Mathf.Sin(angleRad),
                baseDir.x * Mathf.Sin(angleRad) + baseDir.y * Mathf.Cos(angleRad)
            ).normalized;
            rb.AddForce(rotatedDir * throwForce, ForceMode2D.Force);

            float originalDrag = rb.linearDamping;
            rb.linearDamping = 3f;

            yield return new WaitForSeconds(1.5f);

            rb.linearDamping = originalDrag;
            rb.linearVelocity = Vector2.zero;
        }

        if (behavior != null) behavior.enabled = true;

        yield return new WaitForSeconds(0.5f);
        isEntityGrabbed = false;
        grabbedAlly = null;
    }
}
