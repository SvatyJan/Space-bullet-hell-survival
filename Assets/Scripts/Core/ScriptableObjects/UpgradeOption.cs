using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgradeOption", menuName = "Upgrades/UpgradeOption")]
public class UpgradeOption : ScriptableObject
{
    //TODO:
    public string description;        // Popis vylep�en�
    public float speedIncrease = 0f;  // P��klad zv��en� rychlosti
    public float damageIncrease = 0f; // P��klad zv��en� po�kozen�
    public float healthIncrease = 0f; // P��klad zv��en� zdrav�

    public void Apply(ShipStats stats)
    {
        stats.Speed += speedIncrease;
        stats.BaseDamage += damageIncrease;
        stats.Health += healthIncrease;
    }
}