using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgradeOption", menuName = "Upgrades/UpgradeOption")]
public class UpgradeOption : ScriptableObject
{
    //TODO:
    public string description;        // Popis vylepšení
    public float speedIncrease = 0f;  // Pøíklad zvýšení rychlosti
    public float damageIncrease = 0f; // Pøíklad zvýšení poškození
    public float healthIncrease = 0f; // Pøíklad zvýšení zdraví

    public void Apply(ShipStats stats)
    {
        stats.Speed += speedIncrease;
        stats.BaseDamage += damageIncrease;
        stats.Health += healthIncrease;
    }
}