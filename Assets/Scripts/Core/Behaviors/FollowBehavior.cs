using UnityEngine;

public class FollowBehavior : EnemyBehaviorBase
{
    private Transform target;

    private Vector3 direction;

    protected new ShipStats shipStats;
    private SpaceEntity ship;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        shipStats = GetComponent<ShipStats>();
        ship = GetComponent<SpaceEntity>();
    }

    public override void Execute()
    {
        if (target == null && this.direction != null) return;

        // Otáèení smìrem k hráèi
        this.direction = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

        ship.transform.position += direction * shipStats.Speed * Time.deltaTime;
    }
}