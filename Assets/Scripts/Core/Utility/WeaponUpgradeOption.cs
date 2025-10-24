using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponUpgradeOption", menuName = "Upgrades/WeaponUpgradeOption")]
public class WeaponUpgradeOption : ScriptableObject, IUpgradeOption
{
    /** Popis vylepšení. */
    public string description;

    [SerializeField] public Sprite icon;

    [SerializeField] public List<StatUpgradeOption> evolvesRequired;

    /** Název zbranì. */
    public string weaponName;

    /** Prefab zbranì. */
    public GameObject weaponPrefab;

    /** Pøiøazení popisu vylepšení. */
    string IUpgradeOption.description => description;
    Sprite IUpgradeOption.icon => icon;
    List<IUpgradeOption>? IUpgradeOption.evolvesRequired => evolvesRequired?.ConvertAll(i => (IUpgradeOption)i);
    public StatType requiredStat;

    private GameObject activeInstance;

    /** Aplikuje zmìny. */
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

    /** Vrátí aktivní instaci zbranì. */
    public GameObject? GetActiveInstance()
    {
        return activeInstance;
    }
}