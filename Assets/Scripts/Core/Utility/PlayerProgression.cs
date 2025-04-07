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
        ShowUpgradeChoices();
    }

    /** Uk�e mo�nosti vylep�en�. */
    private void ShowUpgradeChoices()
    {
        Time.timeScale = 0f;
        upgradePanel.SetActive(true);

        List<UpgradeOption> options = new List<UpgradeOption>();

        // 1. Stat upgrady - jen pokud nen� na max levelu a hr�� nem� max po�et atribut�
        foreach (var statUpgrade in statUpgrades)
        {
            if (shipStats.CanAddStatUpgrade(statUpgrade.statType) && !shipStats.IsStatMaxed(statUpgrade.statType))
            {
                options.Add(statUpgrade);
            }
        }

        // 2. Weapon upgrady - jen pokud:
        // - hr�� nem� pln� po�et zbran� a nem� tu zbra�
        // - nebo u� ji m�, ale nen� na max levelu
        // - nebo m� zbra� a jej� kombinuj�c� stat na max, a m��e prov�st evoluci
        foreach (var weaponUpgrade in weaponUpgrades)
        {
            if (shipStats.HasWeapon(weaponUpgrade.weaponName))
            {
                int weaponLevel = shipStats.GetWeaponLevel(weaponUpgrade.weaponName);
                bool isWeaponMaxed = weaponLevel >= 5;

                if (!isWeaponMaxed)
                {
                    options.Add(weaponUpgrade);
                }
                else if (isWeaponMaxed && shipStats.CanEvolveWeapon(weaponUpgrade.weaponName))
                {
                    options.Add(weaponUpgrade); // Evoluce nab�dky
                }
            }
            else if (!shipStats.HasMaxWeapons())
            {
                options.Add(weaponUpgrade);
            }
        }

        // N�hodn� v�b�r 3 mo�nost�
        List<UpgradeOption> upgradeChoices = GetRandomUpgrades(options, 3);

        // Nastaven� UI
        for (int i = 0; i < upgradeCards.Length; i++)
        {
            if (i < upgradeChoices.Count)
            {
                int index = i;
                upgradeCards[i].SetActive(true);
                upgradeDescriptions[i].text = $"{upgradeChoices[i].name}\n{upgradeChoices[i].description}";
                var button = upgradeCards[i].GetComponent<UnityEngine.UI.Button>();
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => ApplyUpgrade(upgradeChoices[index]));
                button.onClick.AddListener(CloseUpgradePanel);
            }
            else
            {
                upgradeCards[i].SetActive(false); // Skryj pr�zdn� karti�ky
            }
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
