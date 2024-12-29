using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponUpgradeOption", menuName = "Upgrades/WeaponUpgradeOption")]
public class WeaponUpgradeOption : ScriptableObject, UpgradeOption
{
    public string description;      // Popis upgradu
    public string weaponName;       // Název zbranì
    public GameObject weaponPrefab; // Prefab zbranì

    string UpgradeOption.description => description;

    public void Apply(ShipStats stats)
    {
        if (stats.CanAddWeapon(weaponName))
        {
            stats.AddWeapon(weaponName);

            // Najdeme podobjekt Weapons
            Transform weaponsContainer = stats.transform.Find("Weapons");
            if (weaponsContainer != null)
            {
                // Instanciujeme zbraò jako dítì objektu Weapons
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