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

    [Header("Upgrade UI Prefab")]
    [SerializeField] private GameObject upgradeCardPrefab;
    [SerializeField] private Transform upgradeCardParent;


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

        // Nen� pot�eba
        //foreach (StatUpgradeOption statUpgrade in statUpgrades) { options.Add(statUpgrade); }
        //foreach(WeaponUpgradeOption weaponUpgrade in weaponUpgrades) { options.Add(weaponUpgrade); }
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
        shipStats.XpNextLevelUp *= 1.1f; // Exponenci�ln� zvy�ujeme po�adavek na dal�� �rove�
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
        List<IUpgradeOption> evolves = GetAvailableEvolves(upgradeCards.Length);
        foreach (var evo in evolves)
        {
            if (!upgradeChoices.Contains(evo))
                upgradeChoices.Add(evo);
        }

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


    /** Vr�t� mo�n� vylep�en� atribut�. */
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
                    options.Add(statUpgrade); // m��u vylep�it
                }
            }
            else if (!hasMaxStats)
            {
                options.Add(statUpgrade); // m��u p�idat nov� stat
            }
        }
    }


    /** Vr�t� mo�n� vylep�en� zbran�. */
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
                options.Add(weaponUpgrade); // nov� zbra�, pokud je m�sto
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
    }

    /** Zav�e panel vylep�ov�n� a odpauzuje hru. */
    private void CloseUpgradePanel()
    {
        upgradePanel.SetActive(false);

        Time.timeScale = 1f;
    }
}
