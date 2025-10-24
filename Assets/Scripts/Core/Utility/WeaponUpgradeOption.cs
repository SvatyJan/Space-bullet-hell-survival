using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponUpgradeOption", menuName = "Upgrades/WeaponUpgradeOption")]
public class WeaponUpgradeOption : ScriptableObject, IUpgradeOption
{
    /** Popis vylep�en�. */
    public string description;

    [SerializeField] public Sprite icon;

    [SerializeField] public List<StatUpgradeOption> evolvesRequired;

    /** N�zev zbran�. */
    public string weaponName;

    /** Prefab zbran�. */
    public GameObject weaponPrefab;

    /** P�i�azen� popisu vylep�en�. */
    string IUpgradeOption.description => description;
    Sprite IUpgradeOption.icon => icon;
    List<IUpgradeOption>? IUpgradeOption.evolvesRequired => evolvesRequired?.ConvertAll(i => (IUpgradeOption)i);
    public StatType requiredStat;

    private GameObject activeInstance;

    /** Aplikuje zm�ny. */
    public GameObject? Apply(ShipStats stats)
    {
        Transform weaponsContainer = stats.GetComponent<PlayerShip>().Weapons.transform;
        if (weaponsContainer != null)
        {
            GameObject weaponInstance = Instantiate(weaponPrefab, weaponsContainer);
            weaponInstance.transform.parent = weaponsContainer.transform;
            activeInstance = weaponInstance;
            // Debug.Log($"Weapon {weaponName} added to ship.");
            return weaponInstance;
        }
        else
        {
            Debug.LogWarning("Weapons container not found on player ship. Weapon cannot be added.");
        }

        Debug.Log("Vracim null?");
        return null;
    }

    /** Vr�t� aktivn� instaci zbran�. */
    public GameObject? GetActiveInstance()
    {
        return activeInstance;
    }
}