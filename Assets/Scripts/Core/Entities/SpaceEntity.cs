using System;
using UnityEngine;
public abstract class SpaceEntity : MonoBehaviour, IController
{
    [SerializeField] protected ShipStats shipStats; // Statistiky lodi (např. rychlost, zdraví)
    public Transform shootingPoint;                // Bod, odkud střílí projektily

    [SerializeField] public IWeapon[] weapons; // Připojené zbraně

    public abstract void Controll(); // Metoda pro řízení entity, implementovaná hráčem nebo počítačem

    public void FireWeapons()
    {
        IWeapon[] weapons = GetComponents<IWeapon>();
        foreach (var weapon in weapons)
        {
            weapon?.Fire();
        }
    }

    public void TakeDamage(float damage)
    {
        shipStats.Health -= damage;
        if (shipStats.Health <= 0)
        {
            DestroyEntity();
        }
    }

    protected void DestroyEntity()
    {
        Debug.Log(gameObject.name + " is destroyed.");
        Destroy(gameObject);
    }    
}