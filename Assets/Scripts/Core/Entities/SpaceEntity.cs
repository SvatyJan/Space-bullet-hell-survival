using UnityEngine;
public abstract class SpaceEntity : MonoBehaviour, IController
{
    [SerializeField] protected ShipStats shipStats; // Statistiky lodi (např. rychlost, zdraví)
    public Transform shootingPoint;                 // Bod, odkud střílí projektily


    private void Start()
    {
        shipStats = GetComponent<ShipStats>();
    }

    public abstract void Controll(); // Metoda pro řízení entity, implementovaná hráčem nebo počítačem

    public ShipStats getShipStats()
    {
        return shipStats;
    }

    public abstract void TakeDamage(float damage);
}