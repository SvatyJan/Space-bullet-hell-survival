using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class PlayerProgression : MonoBehaviour
{
    private ShipStats shipStats;
    public List<StatUpgradeOption> statUpgrades;     // Dostupn� vylep�en� stat�
    public List<WeaponUpgradeOption> weaponUpgrades; // Dostupn� vylep�en� zbran�

    [Header("UI Elements")]
    public GameObject upgradePanel;        // Upgrade menu
    public GameObject[] upgradeCards;      // Karti�ky pro jednotliv� mo�nosti vylep�en�
    public TMP_Text[] upgradeDescriptions; // Texty na karti�k�ch (TextMeshPro)

    private void Start()
    {
        shipStats = GetComponent<ShipStats>();
        if (shipStats == null)
        {
            Debug.LogError("PlayerProgression requires ShipStats component!");
        }

        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
        }
    }

    public void AddXP(float xpAmount)
    {
        shipStats.XP += xpAmount;
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        while (shipStats.XP >= shipStats.XpNextLevelUp)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        shipStats.XP -= shipStats.XpNextLevelUp;
        shipStats.XpNextLevelUp *= 1.5f; // Zvy�ujeme po�adavek na dal�� �rove� (exponenci�ln�)
        shipStats.Level++;
        Debug.Log("Level Up to " + shipStats.Level + "! Choose an upgrade!");
        ShowUpgradeChoices();
    }

    private void ShowUpgradeChoices()
    {
        // Zastav�me hru
        Time.timeScale = 0f;

        // Zobraz�me panel
        upgradePanel.SetActive(true);

        List<UpgradeOption> options = new List<UpgradeOption>();

        // P�id�me dostupn� statov� upgrady
        foreach (var statUpgrade in statUpgrades)
        {
            if (shipStats.CanAddStatUpgrade(statUpgrade.statType))
            {
                options.Add(statUpgrade);
            }
        }

        // P�id�me dostupn� zbra�ov� upgrady
        foreach (var weaponUpgrade in weaponUpgrades)
        {
            if (shipStats.CanAddWeapon(weaponUpgrade.weaponName))
            {
                options.Add(weaponUpgrade);
            }
        }

        // Vybereme 3 n�hodn� vylep�en�
        List<UpgradeOption> upgradeChoices = GetRandomUpgrades(options, 3);

        // Nastav�me popisy a callbacky na karti�ky
        for (int i = 0; i < upgradeChoices.Count; i++)
        {
            int index = i; // Nutn� pro spr�vn� zachycen� closure
            upgradeCards[i].SetActive(true); // Ujist�me se, �e je karti�ka viditeln�
            upgradeDescriptions[i].text = $"{upgradeChoices[i].name}\n{upgradeChoices[i].description}";
            upgradeCards[i].GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
            upgradeCards[i].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => ApplyUpgrade(upgradeChoices[index]));
            upgradeCards[i].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(CloseUpgradePanel);
        }
    }

    private List<UpgradeOption> GetRandomUpgrades(List<UpgradeOption> options, int count)
    {
        List<UpgradeOption> randomUpgrades = new List<UpgradeOption>();
        for (int i = 0; i < count && options.Count > 0; i++)
        {
            int index = Random.Range(0, options.Count);
            randomUpgrades.Add(options[index]);
            options.RemoveAt(index);
        }
        return randomUpgrades;
    }


    public void ApplyUpgrade(UpgradeOption upgrade)
    {
        upgrade.Apply(shipStats); // Aplikujeme vybran� vylep�en� na lo�
        Debug.Log($"Applied upgrade: {upgrade.name}");
    }

    private void CloseUpgradePanel()
    {
        // Skryjeme panel
        upgradePanel.SetActive(false);

        // Obnov�me hru
        Time.timeScale = 1f;
    }
}
