using UnityEngine;

public interface IWeapon
{
    void Fire();
    void Upgrade();
    void Downgrade();
    void Evolve();
    public void ReleaseProjectileFromPool(GameObject Projectile);
}