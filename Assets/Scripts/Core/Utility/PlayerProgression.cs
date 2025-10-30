using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerProgression : MonoBehaviour
{
    public ShipStats shipStats;

    /** Dostupn� vylep�en� atribut�. */
    public List<StatUpgradeOption> statUpgrades;

    /** Dostupn� vylep�en� zbran�. */
    public List<WeaponUpgradeOption> weaponUpgrades;

    /** Nese informaci o tom, jak� zbran� byly vylep�eny. */
    public Dictionary<WeaponUpgradeOption, int> weaponLevels = new Dictionary<WeaponUpgradeOption, int>();

    /** Nese informaci o tom, jak� atributy byly vylep�eny. */
    public Dictionary<StatType, int> statLevels = new Dictionary<StatType, int>();

    /** Maxim�ln� po�et vylep�en� atribut�. */
    [SerializeField] public int maxStatUpgrade = 5;

    /** Maxim�ln� po�et vylep�en� zbran�. */
    [SerializeField] public int maxWeaponUpgrade = 5;

    /** Po�et zobrazen�ch karti�ek vylep�en�. */
    [SerializeField] public int upgradesCount = 3;

    /** Mo�nosti vylep�en�. */
    private List<IUpgradeOption> options = new List<IUpgradeOption>();

    /** List vylep�en� co se uk�e na karti�k�ch. */
    List<IUpgradeOption> upgradeChoices = new List<IUpgradeOption>();

    /** Reprezentuje instace zbran� kter� ji� existuj�. */
    private List<GameObject> weaponInstances = new List<GameObject>();

    [SerializeField] private WeaponUpgradeOption? startingWeapon;

    /** P��znak, �e hr�� je ve stavu z�sk�v�n� nov�ho levelu. Nem��e z�sk�vat dal�� aktu�ln� XP. */
    public bool isPlayerLevelingUp = false;

    [Header("UI Elements")]
    /** Upgrade menu. */
    public GameObject upgradePanel;

    [Header("Upgrade UI Prefab")]
    /** Prefab karti�ky vylep�en�. */
    [SerializeField] private GameObject upgradeCardPrefab;
    /** Prefab rodi�e karti�ek vylep�en�. */
    [SerializeField] private Transform upgradeCardParent;
    /** Reference na upgradeDisplayUI. */
    [SerializeField] private PlayerUpgradeDisplayUI upgradeDisplayUI;
    /** End screen menu. */
    [SerializeField] private GameObject endScreenMenu;

    public void SetUpgradePanel(GameObject panel) => upgradePanel = panel;
    public GameObject GetUpgradePanel() => upgradePanel;

    public void SetUpgradeCardPrefab(GameObject UpgradeCardPrefab) => upgradeCardPrefab = UpgradeCardPrefab;
    public GameObject GetUpgradeCardPrefab() => upgradeCardPrefab;

    public void SetUpgradeCardParent(Transform UpgradeCardParent) => upgradeCardParent = UpgradeCardParent;
    public Transform GetUpgradeCardParent() => upgradeCardParent;

    public void SetUpgradeDisplayUI(PlayerUpgradeDisplayUI UpgradeDisplayUI) => upgradeDisplayUI = UpgradeDisplayUI;
    public PlayerUpgradeDisplayUI GetUpgradeDisplayUI() => upgradeDisplayUI;

    public void SetEndScreenMenu(GameObject EndScreenMenu) => endScreenMenu = EndScreenMenu;
    public GameObject GetEndScreenMenu() => endScreenMenu;

    [Header("Observer")]
    public System.Action OnUpgradesChanged;

    private void Start()
    {
        if(endScreenMenu == null)
        {
            Debug.LogError("PlayerProgression requires end screen menu in UI!");
        }
        else
        {
            endScreenMenu.SetActive(false);
        }

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
        shipStats.Level++;
        shipStats.XpNextLevelUp = shipStats.GetXPForNextLevel(shipStats.Level);
        StartCoroutine(SmoothPauseGameAndShowUpgradeChoices());
    }

    /** Uk�e mo�nosti vylep�en�. */
    private void ShowUpgradeChoices()
    {
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
            GameSpeedManager.SetGameSpeed(1f);
            GameSpeedManager.SetSavedGameSpeed(1f);
            Debug.Log("Nen� k v�b�ru ��dn� upgrade.");
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
                StartCoroutine(SmoothResumeGame());
                upgradePanel.SetActive(false);
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
        OnUpgradesChanged?.Invoke();
    }

    /** Plynul� pauza hry. */
    private IEnumerator SmoothPauseGameAndShowUpgradeChoices()
    {
        isPlayerLevelingUp = true;
        GameSpeedManager.SetGameSpeed(0.25f);
        GameSpeedManager.SetSavedGameSpeed(0.25f);

        yield return new WaitForSecondsRealtime(1.5f);

        GameSpeedManager.SetGameSpeed(0f);
        GameSpeedManager.SetSavedGameSpeed(0f);
        ShowUpgradeChoices();
    }

    /** Plynul� n�vrat na plnou rychlost. */
    private IEnumerator SmoothResumeGame()
    {
        GameSpeedManager.SetGameSpeed(0.5f);
        GameSpeedManager.SetSavedGameSpeed(0.5f);

        yield return new WaitForSecondsRealtime(1f);

        GameSpeedManager.SetGameSpeed(1f);
        GameSpeedManager.SetSavedGameSpeed(1f);
        isPlayerLevelingUp = false;
    }

    public void AddOrUpgradeUpgrade(IUpgradeOption upgrade)
    {
        if (upgrade is StatUpgradeOption stat)
        {
            if (!statLevels.ContainsKey(stat.statType))
                statLevels[stat.statType] = 0;

            int currentLevel = statLevels[stat.statType];
            if (currentLevel >= maxStatUpgrade)
            {
                Debug.Log($"Stat '{stat.statType}' je ji� na max �rovni ({maxStatUpgrade}).");
                return;
            }

            statLevels[stat.statType]++;
            stat.Apply(shipStats);
            Debug.Log($"Stat '{stat.statType}' zv��en na {statLevels[stat.statType]}");
        }
        else if (upgrade is WeaponUpgradeOption weapon)
        {
            if (!weaponLevels.ContainsKey(weapon))
            {
                GameObject weaponGO = weapon.Apply(shipStats);
                if (weaponGO != null)
                {
                    weaponInstances.Add(weaponGO);
                    weaponLevels[weapon] = 1;
                    Debug.Log($"Zbra� '{weapon.name}' vytvo�ena (level 1).");
                }
                OnUpgradesChanged?.Invoke();
                return;
            }

            int currentLevel = weaponLevels[weapon];

            if (currentLevel < maxWeaponUpgrade)
            {
                GameObject weaponGO = weapon.GetActiveInstance();
                if (weaponGO != null)
                {
                    var w = weaponGO.GetComponent<IWeapon>();
                    if (w != null)
                    {
                        w.Upgrade();
                        weaponLevels[weapon]++;
                        Debug.Log($"Zbra� '{weapon.name}' zv��ena na level {weaponLevels[weapon]}");
                    }
                }
            }
            else if (CanEvolve(weapon))
            {
                GameObject weaponGO = weapon.GetActiveInstance();
                var w = weaponGO?.GetComponent<IWeapon>();
                if (w != null)
                {
                    w.Evolve();
                    weaponUpgrades.Remove(weapon);
                    Debug.Log($"Zbra� '{weapon.name}' byla evolvov�na!");
                }
            }
            else
            {
                Debug.Log($"Zbra� '{weapon.name}' je ji� na maxim�ln� �rovni ({maxWeaponUpgrade}) a nelze ji evolvovat.");
            }
        }

        OnUpgradesChanged?.Invoke();
    }

    public void RemoveOrDowngradeUpgrade(IUpgradeOption upgrade)
    {
        if (upgrade is StatUpgradeOption stat)
        {
            if (!statLevels.ContainsKey(stat.statType) || statLevels[stat.statType] == 0)
            {
                Debug.Log($"Stat '{stat.statType}' nen� aktivn�.");
                return;
            }

            int currentLevel = statLevels[stat.statType];
            if (currentLevel > 1)
            {
                statLevels[stat.statType]--;
                Debug.Log($"Stat '{stat.statType}' sn�en na level {statLevels[stat.statType]}");
            }
            else
            {
                statLevels[stat.statType] = 0;
                Debug.Log($"Stat '{stat.statType}' odstran�n.");
            }

            stat.Remove(shipStats);
        }
        else if (upgrade is WeaponUpgradeOption weapon)
        {
            if (!weaponLevels.ContainsKey(weapon) || weaponLevels[weapon] == 0)
            {
                Debug.Log($"Zbra� '{weapon.name}' nen� aktivn�.");
                return;
            }

            int currentLevel = weaponLevels[weapon];
            GameObject weaponGO = weapon.GetActiveInstance();
            IWeapon w = weaponGO != null ? weaponGO.GetComponent<IWeapon>() : null;

            if (currentLevel > 1)
            {
                weaponLevels[weapon]--;
                w?.Downgrade();
                Debug.Log($"Zbra� '{weapon.name}' sn�ena na level {weaponLevels[weapon]}");
            }
            else
            {
                if (!weaponUpgrades.Contains(weapon))
                {
                    Debug.Log($"Zbra� '{weapon.name}' byla de-evolvov�na.");
                    weaponUpgrades.Add(weapon);
                }

                if (weaponGO != null)
                {
                    weaponInstances.Remove(weaponGO);
                    Destroy(weaponGO);
                }
                weaponLevels.Remove(weapon);

                Debug.Log($"Zbra� '{weapon.name}' odstran�na.");
            }
        }

        OnUpgradesChanged?.Invoke();
    }
}
