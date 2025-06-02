using UnityEngine;

public interface IUpgradeOption
{
    string name { get; }
    string description { get; }
    Sprite? icon { get; }
    GameObject? Apply(ShipStats stats);
}
