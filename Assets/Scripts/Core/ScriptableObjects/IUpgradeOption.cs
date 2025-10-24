using System.Collections.Generic;
using UnityEngine;

public interface IUpgradeOption
{
    string name { get; }
    string description { get; }
    Sprite? icon { get; }
    List<IUpgradeOption>? evolvesRequired { get; }
    GameObject? Apply(ShipStats stats);
}
