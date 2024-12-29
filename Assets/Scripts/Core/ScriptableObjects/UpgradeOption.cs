public interface UpgradeOption
{
    string name { get; }
    string description { get; }
    void Apply(ShipStats stats);
}
