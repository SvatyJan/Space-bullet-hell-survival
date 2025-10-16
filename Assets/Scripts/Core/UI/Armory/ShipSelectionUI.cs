using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class ShipSelectionUI : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private List<ShipEntry> ships = new List<ShipEntry>();

    [Header("Single Card (no instantiation)")]
    [SerializeField] private ShipCard card;
    [SerializeField] private TMPro.TMP_Text externalStats;

    private int index = 0;

    private void Start()
    {
        if (ships.Count == 0 || card == null)
        {
            Debug.LogError("[ShipSelectionUI] Chybí ships nebo card (Selected Ship Card).");
            return;
        }

        string savedId = SelectedShip.LoadOrDefault(ships[0].id);
        int savedIdx = ships.FindIndex(s => s.id == savedId);
        index = Mathf.Max(0, savedIdx);

        card.onSelect = SelectCurrent;

        Refresh();
    }

    private void Refresh()
    {
        var e = ships[index];
        string stats = BuildStatsString(e.prefab);

        card.Setup(e.displayName, e.icon, stats, true);

        if (externalStats != null)
            externalStats.text = stats;
    }

    // ---- UI callbacks ----
    public void Next()
    {
        if (ships.Count == 0) return;
        index = (index + 1) % ships.Count;
        Refresh();
    }

    public void Prev()
    {
        if (ships.Count == 0) return;
        index = (index - 1 + ships.Count) % ships.Count;
        Refresh();
    }

    public void SelectCurrent() => ConfirmSelection();

    public void ConfirmSelection()
    {
        if (ships.Count == 0) return;
        SelectedShip.Save(ships[index].id);
        Debug.Log($"[ShipSelectionUI] Selected: {ships[index].id}");
    }

    private string BuildStatsString(GameObject shipPrefab)
    {
        ShipStats shipStats = shipPrefab.GetComponent<ShipStats>();

        float health = shipStats.CurrentHealth;
        float speed = shipStats.Speed;
        float acceleration = shipStats.Acceleration;
        float rotation = shipStats.RotationSpeed;
        int projectilesCount = shipStats.ProjectilesCount;
        int weaponSlots = shipStats.maxWeapons;
        int attributeSlots = shipStats.maxStatUpgrades;

        var sb = new StringBuilder();
        sb.AppendLine($"HP: {health:0}");
        sb.AppendLine($"Speed: {speed:0.##}");
        sb.AppendLine($"Accel: {acceleration:0.##}");
        sb.AppendLine($"Rotation: {rotation:0.##}");
        sb.AppendLine($"Projectiles count: {projectilesCount}");
        sb.AppendLine($"Weapon slots: {weaponSlots}");
        sb.Append($"Attribute slots: {attributeSlots}");
        return sb.ToString();
    }
}
