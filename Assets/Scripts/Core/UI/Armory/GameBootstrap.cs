using UnityEngine;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine.UI;
using TMPro;

public class GameBootstrap : MonoBehaviour
{
    [Header("Ships (stejný seznam jako v Armory)")]
    [SerializeField] private List<ShipEntry> ships = new List<ShipEntry>();

    [Header("Spawn")]
    [SerializeField] private Transform spawnPoint;

    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera vcam;

    [Header("Controls")]
    [SerializeField] private ShipController shipController;

    [Header("UI scene objects")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image healthFillImage;
    [SerializeField] private Image healthBackgroundImage;

    [SerializeField] private Slider xpSlider;
    [SerializeField] private Image xpFillImage;
    [SerializeField] private Image xpBackgroundImage;
    [SerializeField] private TextMeshProUGUI xpLevel;

    [SerializeField] private GameObject endScreenMenu;

    [SerializeField] private GameObject upgradePanelGameObject;
    [SerializeField] private GameObject upgradeCardPrefab;
    [SerializeField] private Transform upgradeCardParent;
    [SerializeField] private PlayerUpgradeDisplayUI playerUpgradeDisplayUI;

    [Header("Spawners")]
    [SerializeField] private TimedEnemySpawner timedEnemySpawner;

    private void Awake()
    {
        if (ships.Count == 0)
        {
            Debug.LogError("[GameBootstrap] Chybí seznam lodí.");
            return;
        }

        string id = SelectedShip.LoadOrDefault(ships[0].id);
        ShipEntry entry = ships.Find(s => s.id == id) ?? ships[0];

        if (entry.prefab == null)
        {
            Debug.LogError($"[GameBootstrap] Prefab chybí pro {entry.id}");
            return;
        }

        Vector3 pos = spawnPoint ? spawnPoint.position : Vector3.zero;
        Quaternion rot = spawnPoint ? spawnPoint.rotation : Quaternion.identity;

        GameObject playerGameObject = Instantiate(entry.prefab, pos, rot);
        var shipStats = playerGameObject.GetComponent<ShipStats>();
        if (shipStats == null)
        {
            Debug.LogError("[GameBootstrap] Na prefab hráèe chybí komponenta ShipStats.");
            return;
        }

        if (vcam != null) vcam.Follow = playerGameObject.transform;
        if (shipController != null) shipController.controllingObject = playerGameObject;

        var healthbar = playerGameObject.GetComponent<Healthbar>();
        if (healthbar != null)
        {
            healthbar.SetHealthSlider(healthSlider);
            healthbar.SetFillImage(healthFillImage);
            healthbar.SetBackgroundImage(healthBackgroundImage);
            healthbar.SetShipStats(shipStats);
        }
        else
        {
            Debug.LogWarning("[GameBootstrap] healthbarGO není pøiøazen.");
        }

        var xpBar = playerGameObject.GetComponent<XpBar>();
        if (xpBar != null)
        {
            xpBar.SetXpSlider(xpSlider);
            xpBar.SetFillImage(xpFillImage);
            xpBar.SetBackgroundImage(xpBackgroundImage);
            xpBar.SetLevelText(xpLevel);
            xpBar.SetShipStats(shipStats);
        }
        else
        {
            Debug.LogWarning("[GameBootstrap] Na xpbarGO není komponenta XpBar.");
        }

        if(timedEnemySpawner != null)
        {
            timedEnemySpawner.SetPlayer(playerGameObject.transform);
        }
        else
        {
            Debug.LogWarning("timedEnemySpawner není set");
        }

        if (endScreenMenu != null)
        {
            playerGameObject.GetComponent<PlayerShip>().SetEndScreenMenu(endScreenMenu);
            playerGameObject.GetComponent<PlayerProgression>().SetEndScreenMenu(endScreenMenu);
        }
        else
        {
            Debug.LogWarning("endScreenMenu není set");
        }

        var progression = playerGameObject.GetComponent<PlayerProgression>();
        if (progression != null)
        {
            progression.SetUpgradePanel(upgradePanelGameObject);

            progression.SetUpgradeCardPrefab(upgradeCardPrefab);
            progression.SetUpgradeCardParent(upgradeCardParent);
            progression.SetUpgradeDisplayUI(playerUpgradeDisplayUI);
            playerUpgradeDisplayUI.SetProgression(progression);
            progression.SetEndScreenMenu(endScreenMenu);
        }
    }
}
