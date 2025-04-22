using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBehaviorBase : MonoBehaviour
{
    protected ShipStats shipStats;
    protected Transform target;
    protected SpaceEntity spaceEntity;
    protected Transform shootingPoint;

    public abstract void Execute();

    private void Start()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }

        if(shootingPoint == null)
        {
            shootingPoint = transform;
        }

        shipStats = GetComponent<ShipStats>();
        spaceEntity = GetComponent<SpaceEntity>();
    }
}