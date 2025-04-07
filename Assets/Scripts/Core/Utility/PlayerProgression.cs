using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class PlayerProgression : MonoBehaviour
{
    private ShipStats shipStats;

    /** Dostupná vylepšení atributù. */
    public List<StatUpgradeOption> statUpgrades;

    /** Dostupná vylepšení zbraní. */
    public List<WeaponUpgradeOption> weaponUpgrades;

    [Header("UI Elements")]
    /** Upgrade menu. */
    public GameObject upgradePanel;

    /** Kartièky pro jednotlivé možnosti vylepšení. */
    public GameObject[] upgradeCards;

    /** Texty na kartièkách. */
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

    /** Pøidá zkušenosti. */
    public void AddXP(float xpAmount)
    {
        shipStats.XP += xpAmount;
        CheckLevelUp();
    }


    /** Kontroluje zda mùže zvýšit úroveò. */
    private void CheckLevelUp()
    {
        while (shipStats.XP >= shipStats.XpNextLevelUp)
        {
            LevelUp();
        }
    }

    /** Zvyšuje úroveò. */
    private void LevelUp()
    {
        shipStats.XP -= shipStats.XpNextLevelUp;
        shipStats.XpNextLevelUp *= 1.5f; // Zvyšujeme požadavek na další úroveò (exponenciálnì)
        shipStats.Level++;
        ShowUpgradeChoices();
    }

    /** Ukáže možnosti vylepšení. */
    private void ShowUpgradeChoices()
    {
        Time.timeScale = 0f;
        upgradePanel.SetActive(true);

        List<UpgradeOption> options = new List<UpgradeOption>();

        // 1. Stat upgrady - jen pokud není na max levelu a hráè nemá max poèet atributù
        foreach (var statUpgrade in statUpgrades)
        {
            if (shipStats.CanAddStatUpgrade(statUpgrade.statType) && !shipStats.IsStatMaxed(statUpgrade.statType))
            {
                options.Add(statUpgrade);
            }
        }

        // 2. Weapon upgrady - jen pokud:
        // - hráè nemá plný poèet zbraní a nemá tu zbraò
        // - nebo už ji má, ale není na max levelu
        // - nebo má zbraò a její kombinující stat na max, a mùže provést evoluci
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
                    options.Add(weaponUpgrade); // Evoluce nabídky
                }
            }
            else if (!shipStats.HasMaxWeapons())
            {
                options.Add(weaponUpgrade);
            }
        }

        // Náhodný výbìr 3 možností
        List<UpgradeOption> upgradeChoices = GetRandomUpgrades(options, 3);

        // Nastavení UI
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
                upgradeCards[i].SetActive(false); // Skryj prázdné kartièky
            }
        }
    }


    /** Vrátí náhodné možné vylepšení. */
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

    /** Aplikuje vylepšení. */
    public void ApplyUpgrade(UpgradeOption upgrade)
    {
        upgrade.Apply(shipStats);
        Debug.Log($"Applied upgrade: {upgrade.name}");
    }

    /** Zavøe panel vylepšování a odpauzuje hru. */
    private void CloseUpgradePanel()
    {
        upgradePanel.SetActive(false);

        Time.timeScale = 1f;
    }
}
