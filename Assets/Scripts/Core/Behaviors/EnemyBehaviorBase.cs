using UnityEngine;

public abstract class EnemyBehaviorBase : MonoBehaviour
{
    protected ShipStats shipStats;
    public abstract void Execute();

    private void Start()
    {
        ShipStats shipStats = GetComponent<ShipStats>();
    }
}