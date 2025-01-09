using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class PlayerProgression : MonoBehaviour
{
    private ShipStats shipStats;

    /** Dostupn� vylep�en� atribut�. */
    public List<StatUpgradeOption> statUpgrades;

    /** Dostupn� vylep�en� zbran�. */
    public List<WeaponUpgradeOption> weaponUpgrades;

    [Header("UI Elements")]
    /** Upgrade menu. */
    public GameObject upgradePanel;

    /** Karti�ky pro jednotliv� mo�nosti vylep�en�. */
    public GameObject[] upgradeCards;

    /** Texty na karti�k�ch. */
    public TMP_Text[] upgradeDescriptions;

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

    /** P�id� zku�enosti. */
    public void AddXP(float xpAmount)
    {
        shipStats.XP += xpAmount;
        CheckLevelUp();
    }


    /** Kontroluje zda m��e zv��it �rove�. */
    private void CheckLevelUp()
    {
        while (shipStats.XP >= shipStats.XpNextLevelUp)
        {
            LevelUp();
        }
    }

    /** Zvy�uje �rove�. */
    private void LevelUp()
    {
        shipStats.XP -= shipStats.XpNextLevelUp;
        shipStats.XpNextLevelUp *= 1.5f; // Zvy�ujeme po�adavek na dal�� �rove� (exponenci�ln�)
        shipStats.Level++;
        Debug.Log("Level Up to " + shipStats.Level + "! Choose an upgrade!");
        ShowUpgradeChoices();
    }

    /** Uk�e mo�nosti vylep�en�. */
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
            int index = i;
            upgradeCards[i].SetActive(true);
            upgradeDescriptions[i].text = $"{upgradeChoices[i].name}\n{upgradeChoices[i].description}";
            upgradeCards[i].GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
            upgradeCards[i].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => ApplyUpgrade(upgradeChoices[index]));
            upgradeCards[i].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(CloseUpgradePanel);
        }
    }

    /** Vr�t� n�hodn� mo�n� vylep�en�. */
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

    /** Aplikuje vylep�en�. */
    public void ApplyUpgrade(UpgradeOption upgrade)
    {
        upgrade.Apply(shipStats);
        Debug.Log($"Applied upgrade: {upgrade.name}");
    }

    /** Zav�e panel vylep�ov�n� a odpauzuje hru. */
    private void CloseUpgradePanel()
    {
        upgradePanel.SetActive(false);

        Time.timeScale = 1f;
    }
}
