using UnityEngine;
using System.Collections.Generic;

public class GameBootstrap : MonoBehaviour
{
    [Header("Data (stejn� seznam jako v menu)")]
    [SerializeField] private List<ShipEntry> ships = new List<ShipEntry>();

    [Header("Spawn")]
    [SerializeField] private Transform spawnPoint;

    private void Awake()
    {
        if (ships.Count == 0)
        {
            Debug.LogError("[GameBootstrap] Chyb� seznam lod�.");
            return;
        }

        string id = SelectedShip.LoadOrDefault(ships[0].id);
        var entry = ships.Find(s => s.id == id) ?? ships[0];

        if (entry.prefab == null)
        {
            Debug.LogError($"[GameBootstrap] Prefab chyb� pro {entry.id}");
            return;
        }

        var pos = spawnPoint ? spawnPoint.position : Vector3.zero;
        var rot = spawnPoint ? spawnPoint.rotation : Quaternion.identity;

        var go = Instantiate(entry.prefab, pos, rot);
    }
}
