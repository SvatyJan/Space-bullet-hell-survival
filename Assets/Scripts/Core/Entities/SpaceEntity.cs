using UnityEngine;
public abstract class SpaceEntity : MonoBehaviour, IController
{
    /** Atributy lodě. */
    [SerializeField] protected ShipStats shipStats;

    /** Bod, odkud entita střílí projektily */
    public Transform shootingPoint;

    private void Start()
    {
        shipStats = GetComponent<ShipStats>();

        if (shootingPoint == null)
        {
            shootingPoint = transform;
        }
    }

    /** Ovládání, implementované chování. */
    public abstract void Controll();

    /**
     * Odečte životy.
     * Pokud má entita méně životů než 0, tak je zničena.
     * Critical strike
     * 0-100 je 1,5x multiplier
     * 100-200 je 2x multiplier
     */
    public abstract void TakeDamage(float damage, float? criticalStrike = null);

    public ShipStats getShipStats()
    {
        return shipStats;
    }
}