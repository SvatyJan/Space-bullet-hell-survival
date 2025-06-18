using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardianBehavior : EnemyBehaviorBase
{
    [Header("Guardian Parts")]
    public Transform eye;
    public Transform innerCircle;
    public Transform outerCircle;
    public Transform tentacleParent;
    public Transform spikesParent;

    [Header("Attack Settings")]
    public float phaseDuration = 5f;
    public float eyeRotationSpeed = 5f;

    [Header("Tentacle Attack Settings")]
    public float tentacleTriggerDistance = 3.5f;

    private float phaseTimer;
    private int currentPhase;

    private Animator animator;
    private List<Animator> tentacleAnimators = new List<Animator>();
    private Rigidbody2D rb;

    private enum GuardianPhase { Laser, Spike, Tentacle }
    private GuardianPhase currentPhaseType;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        tentacleAnimators.Clear();
        foreach (Transform child in tentacleParent)
        {
            Animator a = child.GetComponent<Animator>();
            if (a != null) tentacleAnimators.Add(a);
        }

        phaseTimer = phaseDuration;
        currentPhase = 0;
    }

    public override void Execute()
    {
        if (target == null) return;

        RotateEyeToTarget();

        phaseTimer -= Time.deltaTime;
        if (phaseTimer <= 0f)
        {
            currentPhase = (currentPhase + 1) % 3;
            phaseTimer = phaseDuration;

            currentPhaseType = (GuardianPhase)currentPhase;
        }

        ExecuteCurrentPhase();
    }


    private void RotateEyeToTarget()
    {
        Vector3 direction = target.position - eye.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle + 90f);
        eye.rotation = Quaternion.Lerp(eye.rotation, targetRotation, Time.deltaTime * eyeRotationSpeed);
    }

    private void ExecuteCurrentPhase()
    {
        switch (currentPhaseType)
        {
            case GuardianPhase.Laser:
                LaserAttack();
                break;
            case GuardianPhase.Spike:
                SpikeAttack();
                break;
            case GuardianPhase.Tentacle:
                TentacleAttack();
                break;
        }
    }


    private void LaserAttack()
    {
        DisableTentacles();
        DisableSpikes();

        // TODO: aktivuj laser výstøel, pøiprav efekt
        Debug.Log("Laser Attack Phase");
    }


    private void SpikeAttack()
    {
        MoveTowardsPlayer();
        DisableTentacles();

        foreach (Transform spike in spikesParent)
        {
            var collider = spike.GetComponent<PolygonCollider2D>();
            var animator = spike.GetComponent<Animator>();

            if (collider != null) collider.enabled = true;
            if (animator != null) animator.SetBool("attacking", true);
        }

        StartCoroutine(DisableSpikesAfterDelay(phaseDuration));
    }

    private IEnumerator DisableSpikesAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (Transform spike in spikesParent)
        {
            PolygonCollider2D collider = spike.GetComponent<PolygonCollider2D>();
            if (collider != null)
                collider.enabled = false;

            Animator spikeAnimator = spike.GetComponent<Animator>();
            if (spikeAnimator != null)
                spikeAnimator.SetBool("attacking", false);
        }
    }

    public void OnSpikeHitPlayer(PlayerShip player)
    {
        player.TakeDamage(shipStats.BaseDamage);
    }

    private void TentacleAttack()
    {
        if (target == null) return;

        // 1) Pøibližuj se
        MoveTowardsPlayer();

        // 2) Hledej nejbližší chapadlo
        Transform closestTentacle = null;
        float closestDistance = float.MaxValue;

        foreach (Transform tentacle in tentacleParent)
        {
            float dist = Vector2.Distance(tentacle.position, target.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestTentacle = tentacle;
            }
        }

        // 3) Spus útok, pokud jsme dost blízko
        if (closestTentacle != null && closestDistance <= tentacleTriggerDistance)
        {
            TentacleBehavior tentacleBehavior = closestTentacle.GetComponent<TentacleBehavior>();
            if (tentacleBehavior != null)
            {
                tentacleBehavior.TryAttack();
            }
        }
    }

    private void DisableSpikes()
    {
        foreach (Transform spike in spikesParent)
        {
            var collider = spike.GetComponent<PolygonCollider2D>();
            var animator = spike.GetComponent<Animator>();

            if (collider != null) collider.enabled = false;
            if (animator != null) animator.SetBool("attacking", false);
        }
    }

    private void DisableTentacles()
    {
        foreach (Transform tentacle in tentacleParent)
        {
            var animator = tentacle.GetComponent<Animator>();
            if (animator != null)
                animator.SetBool("attacking", false);
        }
    }

    private void MoveTowardsPlayer()
    {
        if (target == null) return;

        Vector2 direction = (target.position - transform.position).normalized;
        transform.position += (Vector3)(direction * shipStats.Speed * Time.deltaTime);
    }

}
