using UnityEngine;

public class FollowBehavior : EnemyBehaviorBase
{
    [SerializeField] private Transform target;
    [SerializeField] private float followSpeed = 3f;

    public override void Execute(SpaceEntity ship)
    {
        if (target == null) return;

        Vector3 direction = (target.position - ship.transform.position).normalized;
        ship.transform.position += direction * followSpeed * Time.deltaTime;
    }
}