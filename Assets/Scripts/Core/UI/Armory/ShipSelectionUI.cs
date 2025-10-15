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

    // ---- Helpers ----
    private string BuildStatsString(GameObject shipPrefab)
    {
        if (!shipPrefab) return "No prefab";
        var stats = shipPrefab.GetComponent<ShipStats>();
        if (!stats) return "Stats: N/A";

        float hp = GetField(stats, "MaxHealth", 100f);
        float speed = GetField(stats, "Speed", 10f);
        float accel = GetField(stats, "Acceleration", 10f);
        float rot = GetField(stats, "RotationSpeed", 120f);
        int weaponSlots = (int)GetField(stats, "MaxWeapons", 2f);

        var sb = new StringBuilder();
        sb.AppendLine($"HP: {hp:0}");
        sb.AppendLine($"Speed: {speed:0.##}");
        sb.AppendLine($"Accel: {accel:0.##}");
        sb.AppendLine($"Rotation: {rot:0.##}");
        sb.Append($"Weapon slots: {weaponSlots}");
        return sb.ToString();
    }

    private float GetField(object obj, string name, float defVal)
    {
        var t = obj.GetType();
        var prop = t.GetProperty(name);
        if (prop != null && prop.CanRead)
        {
            if (prop.PropertyType == typeof(float)) return (float)prop.GetValue(obj);
            if (prop.PropertyType == typeof(int)) return (int)prop.GetValue(obj);
        }
        var field = t.GetField(name);
        if (field != null)
        {
            if (field.FieldType == typeof(float)) return (float)field.GetValue(obj);
            if (field.FieldType == typeof(int)) return (int)field.GetValue(obj);
        }
        return defVal;
    }
}
