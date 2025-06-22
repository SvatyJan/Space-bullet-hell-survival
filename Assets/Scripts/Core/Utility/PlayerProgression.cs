using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class PlayerProgression : MonoBehaviour
{
    public ShipStats shipStats;

    /** Dostupná vylepšení atributù. */
    public List<StatUpgradeOption> statUpgrades;

    /** Dostupná vylepšení zbraní. */
    public List<WeaponUpgradeOption> weaponUpgrades;

    /** Nese informaci o tom, jaké zbranì byly vylepšeny. */
    public Dictionary<WeaponUpgradeOption, int> weaponLevels = new Dictionary<WeaponUpgradeOption, int>();

    /** Nese informaci o tom, jaké atributy byly vylepšeny. */
    public Dictionary<StatType, int> statLevels = new Dictionary<StatType, int>();

    /** Maximální poèet vylepšení atributù. */
    [SerializeField] public int maxStatUpgrade = 5;

    /** Maximální poèet vylepšení zbraní. */
    [SerializeField] public int maxWeaponUpgrade = 5;

    /** Poèet zobrazených kartièek vylepšení. */
    [SerializeField] public int upgradesCount = 3;

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

    [Header("Upgrade UI Prefab")]
    /** Prefab kartièky vylepšení. */
    [SerializeField] private GameObject upgradeCardPrefab;
    /** Prefab rodièe kartièek vylepšení. */
    [SerializeField] private Transform upgradeCardParent;
    /** Reference na upgradeDisplayUI. */
    [SerializeField] private PlayerUpgradeDisplayUI upgradeDisplayUI;

    [Header("Observer")]
    public System.Action OnUpgradesChanged;

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

                    AWeapon aWeapon = weaponGO.GetComponent<AWeapon>();
                    if (aWeapon != null)
                    {
                        Debug.Log("Existuje Awepon: " + aWeapon);
                        var slot = upgradeDisplayUI.GetNextAvailableWeaponSlot();
                        if (slot != null)
                        {
                            aWeapon.SetSlotUI(slot);
                        }
                    }
                }
            }
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
        shipStats.XpNextLevelUp *= 1.1f; // Exponenciálnì zvyšujeme požadavek na další úroveò
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
        List<IUpgradeOption> evolves = GetAvailableEvolves(upgradesCount);
        foreach (var evo in evolves)
        {
            if (!upgradeChoices.Contains(evo))
                upgradeChoices.Add(evo);
        }

        if (options.Count == 0 && upgradeChoices.Count == 0)
        {
            Time.timeScale = 1f;
            Debug.Log("Není k výbìru žádný upgrade.");
            return;
        }

        int remaining = upgradesCount - upgradeChoices.Count;
        if (remaining > 0)
        {
            upgradeChoices.AddRange(GetRandomUpgrades(options, remaining));
        }

        foreach (Transform child in upgradeCardParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var upgrade in upgradeChoices)
        {
            GameObject cardGO = Instantiate(upgradeCardPrefab, upgradeCardParent);
            UpgradeCardUI card = cardGO.GetComponent<UpgradeCardUI>();

            bool isEvolved = false;

            if (upgrade is WeaponUpgradeOption weaponUpgrade)
            {
                isEvolved = CanEvolve(weaponUpgrade);
            }

            card.SetUpgradeData(upgrade, () =>
            {
                ApplyUpgrade(upgrade);
                CloseUpgradePanel();
            }, isEvolved);
        }


        upgradePanel.SetActive(true);
    }


    /** Vrátí možné vylepšení atributù. */
    private void CheckAvailableStats()
    {
        int currentStatCount = 0;

        foreach (var kvp in statLevels)
        {
            if (kvp.Value > 0)
            {
                currentStatCount++;
            }
        }

        bool hasMaxStats = currentStatCount >= shipStats.maxStatUpgrades;

        foreach (StatUpgradeOption statUpgrade in statUpgrades)
        {
            bool hasStat = statLevels.ContainsKey(statUpgrade.statType);
            int level = hasStat ? statLevels[statUpgrade.statType] : 0;

            if (hasStat)
            {
                if (level < maxStatUpgrade)
                {
                    options.Add(statUpgrade); // mùžu vylepšit
                }
            }
            else if (!hasMaxStats)
            {
                options.Add(statUpgrade); // mùžu pøidat nový stat
            }
        }
    }


    /** Vrátí možné vylepšení zbraní. */
    private void CheckAvailableWeapons()
    {
        int currentWeaponCount = 0;

        foreach (var kvp in weaponLevels)
        {
            if (kvp.Value > 0)
            {
                currentWeaponCount++;
            }
        }

        bool hasMaxWeapons = currentWeaponCount >= shipStats.maxWeapons;

        foreach (WeaponUpgradeOption weaponUpgrade in weaponUpgrades)
        {
            bool hasWeapon = weaponLevels.ContainsKey(weaponUpgrade);
            int level = hasWeapon ? weaponLevels[weaponUpgrade] : 0;
            bool canEvolve = CanEvolve(weaponUpgrade);

            if (hasWeapon)
            {
                if (level < maxWeaponUpgrade || canEvolve)
                {
                    options.Add(weaponUpgrade); // upgrade
                }
            }
            else if (!hasMaxWeapons)
            {
                options.Add(weaponUpgrade); // nová zbraò, pokud je místo
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

        HashSet<string> existingNames = new HashSet<string>();
        foreach (var opt in upgradeChoices)
        {
            existingNames.Add(opt.name);
        }

        List<IUpgradeOption> filtered = new List<IUpgradeOption>();
        foreach (var opt in options)
        {
            if (!existingNames.Contains(opt.name))
            {
                filtered.Add(opt);
            }
        }

        for (int i = 0; i < count && filtered.Count > 0; i++)
        {
            int index = UnityEngine.Random.Range(0, filtered.Count);
            randomUpgrades.Add(filtered[index]);
            filtered.RemoveAt(index);
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

                    AWeapon aWeapon = weaponGO.GetComponent<AWeapon>();
                    if (aWeapon != null)
                    {
                        Debug.Log("Existuje Awepon: " + aWeapon);
                        var slot = upgradeDisplayUI.GetNextAvailableWeaponSlot();
                        if (slot != null)
                        {
                            aWeapon.SetSlotUI(slot);
                        }
                    }
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
            else if (!weaponLevels.ContainsKey(weapon))
            {
                GameObject weaponGO = weapon.Apply(shipStats);
                if (weaponGO != null)
                {
                    weaponInstances.Add(weaponGO);
                    weaponLevels[weapon] = 1;
                }
            }
            else
            {
                GameObject weaponGO = weapon.GetActiveInstance();
                if (weaponGO == null)
                {
                    Debug.LogWarning($"WeaponUpgradeOption '{weapon.name}' has no activeInstance.");
                    return;
                }

                IWeapon weaponComponent = weaponGO.GetComponent<IWeapon>();
                if (weaponComponent != null)
                {
                    weaponComponent.Upgrade();
                    weaponLevels[weapon]++;
                }
                else
                {
                    Debug.LogWarning($"Weapon '{weapon.name}' upgrade failed - no IWeapon component found.");
                }
            }
        }
        OnUpgradesChanged?.Invoke();
    }

    /** Zavøe panel vylepšování a odpauzuje hru. */
    private void CloseUpgradePanel()
    {
        upgradePanel.SetActive(false);
        StartCoroutine(SmoothResumeGame());
    }

    /** Plynulý návrat na plnou rychlost. */
    private IEnumerator SmoothResumeGame()
    {
        Time.timeScale = 0.3f;

        yield return new WaitForSecondsRealtime(1f);

        Time.timeScale = 1f;
    }
}
