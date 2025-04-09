using UnityEngine;

public interface IUpgradeOption
{
    string name { get; }
    string description { get; }
    GameObject? Apply(ShipStats stats);
}
