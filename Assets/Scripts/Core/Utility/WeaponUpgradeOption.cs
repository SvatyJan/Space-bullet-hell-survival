using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponUpgradeOption", menuName = "Upgrades/WeaponUpgradeOption")]
public class WeaponUpgradeOption : ScriptableObject, UpgradeOption
{
    /** Popis vylep�en�. */
    public string description;

    /** N�zev zbran�. */
    public string weaponName;

    /** Prefab zbran�. */
    public GameObject weaponPrefab;

    /** P�i�azen� popisu vylep�en�. */
    string UpgradeOption.description => description;

    /** Aplikuje zm�ny. */
    public void Apply(ShipStats stats)
    {
        if (stats.CanAddWeapon(weaponName))
        {
            stats.AddWeapon(weaponName);

            // Najdeme podobjekt Weapons
            Transform weaponsContainer = stats.transform.Find("Weapons");
            if (weaponsContainer != null)
            {
                // Instanciujeme zbra� jako d�t� objektu Weapons
                Instantiate(weaponPrefab, weaponsContainer);
            }
            else
            {
                Debug.LogWarning("Weapons container not found on player ship. Weapon cannot be added.");
            }

            Debug.Log($"Weapon {weaponName} added to ship.");
        }
    }
}