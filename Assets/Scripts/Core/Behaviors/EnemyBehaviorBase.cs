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
        target = GameObject.FindGameObjectWithTag("Player").transform;
        shipStats = GetComponent<ShipStats>();
        spaceEntity = GetComponent<SpaceEntity>();

        // Zkontrolujeme, zda je shootingPoint nastaven; pokud ne, použijeme pozici nepřítele
        if (shootingPoint == null)
        {
            shootingPoint = spaceEntity.shootingPoint;
        }
    }
}