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

    private float phaseTimer;
    private int currentPhase;

    private Animator animator;
    private Rigidbody2D rb;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        phaseTimer = phaseDuration;
        currentPhase = 0;
    }

    public override void Execute()
    {
        if (target == null) return;

        RotateEyeToTarget();

        phaseTimer -= Time.deltaTime;
        if (phaseTimer <= 0)
        {
            currentPhase = (currentPhase + 1) % 3;
            phaseTimer = phaseDuration;
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
        switch (currentPhase)
        {
            case 0:
                LaserAttack();
                break;
            case 1:
                SpikeAttack();
                break;
            case 2:
                TentacleAttack();
                break;
        }
    }

    private void LaserAttack()
    {
        // TODO: Implement laser firing from eye or sides
        Debug.Log("Laser Attack Phase");
    }

    private void SpikeAttack()
    {
        // TODO: Enable colliders or animate spikes for damage
        Debug.Log("Spike Attack Phase");
    }

    private void TentacleAttack()
    {
        // TODO: Trigger tentacle attack animations independently
        Debug.Log("Tentacle Attack Phase");
    }
}
