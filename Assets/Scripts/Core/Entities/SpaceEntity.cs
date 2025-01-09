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

    public ShipStats getShipStats()
    {
        return shipStats;
    }

    /**
     * Odečte životy.
     * Pokud má entita méně životů než 0, tak je zničena.
     */
    public abstract void TakeDamage(float damage);
}