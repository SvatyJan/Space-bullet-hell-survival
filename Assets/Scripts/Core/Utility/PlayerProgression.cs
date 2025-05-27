using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerProgression : MonoBehaviour
{
    private ShipStats shipStats;

    /** Dostupná vylepšení atributù. */
    public List<StatUpgradeOption> statUpgrades;

    /** Dostupná vylepšení zbraní. */
    public List<WeaponUpgradeOption> weaponUpgrades;

    /** Nese informaci o tom, jaké zbranì byly vylepšeny. */
    private Dictionary<WeaponUpgradeOption, int> weaponLevels = new Dictionary<WeaponUpgradeOption, int>();

    /** Nese informaci o tom, jaké atributy byly vylepšeny. */
    private Dictionary<StatType, int> statLevels = new Dictionary<StatType, int>();

    /** Maximální poèet vylepšení atributù. */
    [SerializeField] public int maxStatUpgrade = 5;

    /** Maximální poèet vylepšení zbraní. */
    [SerializeField] public int maxWeaponUpgrade = 5;

    /** Možnosti vylepšení. */
    private List<IUpgradeOption> options = new List<IUpgradeOption>();

    /** List vylepšení co se ukáže na kartièkách. */
    List<IUpgradeOption> upgradeChoices = new List<IUpgradeOption>();

    /** Reprezentuje instace zbraní které již existují. */
    private List<GameObject> weaponInstances = new List<GameObject>();

    [SerializeField] private WeaponUpgradeOption? startingWeapon;

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

        if (startingWeapon != null)
        {
            GameObject weaponGO = startingWeapon.Apply(shipStats);
            if (weaponGO != null)
            {
                weaponInstances.Add(weaponGO);
                weaponLevels[startingWeapon] = 1;

                if (!weaponUpgrades.Contains(startingWeapon))
                {
                    weaponUpgrades.Add(startingWeapon);
                    options.Add(startingWeapon);
                }
            }
        }

        foreach (StatUpgradeOption statUpgrade in statUpgrades) { options.Add(statUpgrade); }
        foreach(WeaponUpgradeOption weaponUpgrade in weaponUpgrades) { options.Add(weaponUpgrade); }
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
        //shipStats.XpNextLevelUp *= 1.1f; // Exponenciálnì zvyšujeme požadavek na další úroveò
        shipStats.Level++;
        ShowUpgradeChoices();
    }

    /** Ukáže možnosti vylepšení. */
    private void ShowUpgradeChoices()
    {
        Time.timeScale = 0f;
        upgradeChoices.Clear();
        options.Clear();

        CheckAvailableStats();

        CheckAvailableWeapons();

        int availableEvolves = CheckAvailableEvolves();

        List<IUpgradeOption> evolves = GetAvailableEvolves(upgradeCards.Length);
        upgradeChoices.AddRange(evolves);

        if (options.Count == 0 && upgradeChoices.Count == 0)
        {
            Time.timeScale = 1f;
            Debug.Log("Není k výbìru žádný upgrade.");
            return;
        }

        int remaining = upgradeCards.Length - upgradeChoices.Count;
        if (remaining > 0)
        {
            upgradeChoices.AddRange(GetRandomUpgrades(options, remaining));
        }

        // Zobraz kartièky
        upgradePanel.SetActive(true);
        for (int i = 0; i < upgradeCards.Length; i++)
        {
            if (i < upgradeChoices.Count)
            {
                var upgrade = upgradeChoices[i];
                int index = i;

                upgradeCards[i].SetActive(true);
                var button = upgradeCards[i].GetComponent<Button>();
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => ApplyUpgrade(upgradeChoices[index]));
                button.onClick.AddListener(CloseUpgradePanel);

                if (upgrade is StatUpgradeOption attribute)
                {
                    int level = statLevels.ContainsKey(attribute.statType) ? statLevels[attribute.statType] : 0;
                    upgradeDescriptions[i].text = $"{upgrade.name} (Lv {level}/{maxStatUpgrade})\n{upgrade.description}";
                    if (!statLevels.ContainsKey(attribute.statType))
                    {
                        statLevels.Add(attribute.statType, level);
                    }
                    else
                    {
                        statLevels.Remove(attribute.statType);
                        statLevels.Add(attribute.statType, level);
                    }
                }
                else if (upgrade is WeaponUpgradeOption weapon)
                {
                    int level = weaponLevels.ContainsKey(weapon) ? weaponLevels[weapon] : 0;

                    if (CanEvolve(weapon))
                    {
                        upgradeDescriptions[i].text = $"EVOLVE \n{upgrade.name}!";
                    }
                    else
                    {
                        upgradeDescriptions[i].text = $"{upgrade.name} (Lv {level}/{maxWeaponUpgrade})\n{upgrade.description}";
                    }

                    if (!weaponLevels.ContainsKey(weapon))
                    {
                        weaponLevels.Add(weapon, level);
                    }
                    else
                    {
                        weaponLevels.Remove(weapon);
                        weaponLevels.Add(weapon, level);
                    }
                        
                }
            }
            else
            {
                upgradeCards[i].SetActive(false);
            }
        }
    }

    /** Vrátí možné vylepšení atributù. */
    private void CheckAvailableStats()
    {
        foreach (StatUpgradeOption statUpgrade in statUpgrades)
        {
            if (statLevels.ContainsKey(statUpgrade.statType) && statLevels[statUpgrade.statType] >= maxStatUpgrade)
            {
                continue;
            }
            options.Add(statUpgrade);
        }
    }

    /** Vrátí možné vylepšení zbraní. */
    private void CheckAvailableWeapons()
    {
        foreach (WeaponUpgradeOption weaponUpgrade in weaponUpgrades)
        {
            if (!weaponLevels.ContainsKey(weaponUpgrade))
            {
                options.Add(weaponUpgrade);
            }
            else if (weaponLevels[weaponUpgrade] < maxWeaponUpgrade)
            {
                options.Add(weaponUpgrade);
            }
        }
    }


    /** Vrátí poèet možných evolvù. */
    private int CheckAvailableEvolves()
    {
        int count = 0;
        foreach (WeaponUpgradeOption weapon in weaponUpgrades)
        {
            if (CanEvolve(weapon))
                count++;
        }
        return count;
    }

    /** Vrátí list možných zbraní k evolvu. */
    private List<IUpgradeOption> GetAvailableEvolves(int maxCount = int.MaxValue)
    {
        List<IUpgradeOption> evolves = new List<IUpgradeOption>();
        foreach (WeaponUpgradeOption weapon in weaponUpgrades)
        {
            if (CanEvolve(weapon))
            {
                evolves.Add(weapon);
                if (evolves.Count >= maxCount)
                    break;
            }
        }
        return evolves;
    }

    /** Vrátí pøíznak, zda je možné zbraò evolvovat. */
    private bool CanEvolve(WeaponUpgradeOption weapon)
    {
        int weaponLevel = weaponLevels.ContainsKey(weapon) ? weaponLevels[weapon] : 0;
        int statLevel = statLevels.ContainsKey(weapon.requiredStat) ? statLevels[weapon.requiredStat] : 0;
        return weaponLevel >= maxWeaponUpgrade && statLevel >= maxStatUpgrade;
    }

    /** Vrátí náhodné možné vylepšení. */
    private List<IUpgradeOption> GetRandomUpgrades(List<IUpgradeOption> options, int count)
    {
        List<IUpgradeOption> randomUpgrades = new List<IUpgradeOption>();
        for (int i = 0; i < count && options.Count > 0; i++)
        {
            int index = UnityEngine.Random.Range(0, options.Count);
            randomUpgrades.Add(options[index]);
            options.RemoveAt(index);
        }
        return randomUpgrades;
    }

    /** 
     * Aplikuje vylepšení.
     * Pokud se jedná o atribut, tak ho vylepší.
     * Pokud se jedná o zbraò
     * Neexistuje -> vytvoøí se a má level 1
     * Existuje -> vylepší se a pøidá se level
     * Pokud je to možné, tak Evolve
     */
    public void ApplyUpgrade(IUpgradeOption upgrade)
    {
        if (upgrade is StatUpgradeOption stat)
        {
            if (!statLevels.ContainsKey(stat.statType))
                statLevels[stat.statType] = 0;

            statLevels[stat.statType]++;
            stat.Apply(shipStats);
        }
        else if (upgrade is WeaponUpgradeOption weapon)
        {
            if (weaponLevels.ContainsKey(weapon) && weaponLevels[weapon] == 0)
            {
                GameObject weaponGO = weapon.Apply(shipStats);
                if (weaponGO != null)
                {
                    weaponInstances.Add(weaponGO);
                    weaponLevels[weapon] = 1;
                }
            }
            else if(weaponLevels.ContainsKey(weapon) && CanEvolve(weapon))
            {
                GameObject weaponGO = weapon.GetActiveInstance();
                IWeapon IWeapon = weaponGO.GetComponent<IWeapon>();
                if(IWeapon != null)
                {
                    IWeapon.Evolve();
                    weaponUpgrades.Remove(weapon);
                }
                else
                {
                    Debug.LogWarning("Weapon '" + weapon + "'evolve was unsuccesful!");
                }
            }
            else
            {
                GameObject weaponGO = weapon.GetActiveInstance();
                IWeapon IWeapon = weaponGO.GetComponent<IWeapon>();
                if (IWeapon != null)
                {
                    IWeapon.Upgrade();
                    weaponLevels[weapon]++;
                }
                else
                {
                    Debug.LogWarning("Weapon '" + weapon + "' upgrade was unsuccesful!");
                }
            }
        }
    }

    /** Zavøe panel vylepšování a odpauzuje hru. */
    private void CloseUpgradePanel()
    {
        upgradePanel.SetActive(false);

        Time.timeScale = 1f;
    }
}
