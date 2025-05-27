using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerProgression : MonoBehaviour
{
    private ShipStats shipStats;

    /** Dostupn� vylep�en� atribut�. */
    public List<StatUpgradeOption> statUpgrades;

    /** Dostupn� vylep�en� zbran�. */
    public List<WeaponUpgradeOption> weaponUpgrades;

    /** Nese informaci o tom, jak� zbran� byly vylep�eny. */
    private Dictionary<WeaponUpgradeOption, int> weaponLevels = new Dictionary<WeaponUpgradeOption, int>();

    /** Nese informaci o tom, jak� atributy byly vylep�eny. */
    private Dictionary<StatType, int> statLevels = new Dictionary<StatType, int>();

    /** Maxim�ln� po�et vylep�en� atribut�. */
    [SerializeField] public int maxStatUpgrade = 5;

    /** Maxim�ln� po�et vylep�en� zbran�. */
    [SerializeField] public int maxWeaponUpgrade = 5;

    /** Mo�nosti vylep�en�. */
    private List<IUpgradeOption> options = new List<IUpgradeOption>();

    /** List vylep�en� co se uk�e na karti�k�ch. */
    List<IUpgradeOption> upgradeChoices = new List<IUpgradeOption>();

    /** Reprezentuje instace zbran� kter� ji� existuj�. */
    private List<GameObject> weaponInstances = new List<GameObject>();

    [SerializeField] private WeaponUpgradeOption? startingWeapon;

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
        //shipStats.XpNextLevelUp *= 1.1f; // Exponenci�ln� zvy�ujeme po�adavek na dal�� �rove�
        shipStats.Level++;
        ShowUpgradeChoices();
    }

    /** Uk�e mo�nosti vylep�en�. */
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
            Debug.Log("Nen� k v�b�ru ��dn� upgrade.");
            return;
        }

        int remaining = upgradeCards.Length - upgradeChoices.Count;
        if (remaining > 0)
        {
            upgradeChoices.AddRange(GetRandomUpgrades(options, remaining));
        }

        // Zobraz karti�ky
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

    /** Vr�t� mo�n� vylep�en� atribut�. */
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

    /** Vr�t� mo�n� vylep�en� zbran�. */
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


    /** Vr�t� po�et mo�n�ch evolv�. */
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

    /** Vr�t� list mo�n�ch zbran� k evolvu. */
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

    /** Vr�t� p��znak, zda je mo�n� zbra� evolvovat. */
    private bool CanEvolve(WeaponUpgradeOption weapon)
    {
        int weaponLevel = weaponLevels.ContainsKey(weapon) ? weaponLevels[weapon] : 0;
        int statLevel = statLevels.ContainsKey(weapon.requiredStat) ? statLevels[weapon.requiredStat] : 0;
        return weaponLevel >= maxWeaponUpgrade && statLevel >= maxStatUpgrade;
    }

    /** Vr�t� n�hodn� mo�n� vylep�en�. */
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
     * Aplikuje vylep�en�.
     * Pokud se jedn� o atribut, tak ho vylep��.
     * Pokud se jedn� o zbra�
     * Neexistuje -> vytvo�� se a m� level 1
     * Existuje -> vylep�� se a p�id� se level
     * Pokud je to mo�n�, tak Evolve
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

    /** Zav�e panel vylep�ov�n� a odpauzuje hru. */
    private void CloseUpgradePanel()
    {
        upgradePanel.SetActive(false);

        Time.timeScale = 1f;
    }
}
