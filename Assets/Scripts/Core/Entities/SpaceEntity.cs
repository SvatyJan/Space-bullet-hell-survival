using UnityEngine;
public abstract class SpaceEntity : MonoBehaviour, IController
{
    [SerializeField] protected ShipStats shipStats; // Statistiky lodi (např. rychlost, zdraví)
    public Transform shootingPoint;                // Bod, odkud střílí projektily

    [SerializeField] public IWeapon[] weapons; // Připojené zbraně

    private void Start()
    {
        shipStats = GetComponent<ShipStats>();
    }

    public abstract void Controll(); // Metoda pro řízení entity, implementovaná hráčem nebo počítačem

    public void FireWeapons()
    {
        IWeapon[] weapons = GetComponents<IWeapon>();
        foreach (var weapon in weapons)
        {
            weapon?.Fire();
        }
    }

    public ShipStats getShipStats()
    {
        return shipStats;
    }

    public abstract void TakeDamage(float damage);
}