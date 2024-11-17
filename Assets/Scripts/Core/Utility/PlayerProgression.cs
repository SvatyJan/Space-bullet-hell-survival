using UnityEngine;
using System.Collections.Generic;

public class PlayerProgression : MonoBehaviour
{
    private ShipStats shipStats;
    public List<UpgradeOption> availableUpgrades; // Seznam možných vylepšení

    private void Start()
    {
        shipStats = GetComponent<ShipStats>();
        if (shipStats == null)
        {
            Debug.LogError("PlayerProgression requires ShipStats component!");
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
            shipStats.XP -= shipStats.XpNextLevelUp;
            shipStats.XpNextLevelUp *= 1.5f; // Zvyšujeme požadavek na další úroveò (exponenciálnì)
            LevelUp();
        }
    }

    private void LevelUp()
    {
        Debug.Log("Level Up! Choose an upgrade!");
        ShowUpgradeChoices();
    }

    private void ShowUpgradeChoices()
    {
        // Vybereme 3 náhodné vylepšení z dostupných
        List<UpgradeOption> upgradeChoices = GetRandomUpgrades(3);

        // Logika k zobrazení UI s vylepšeními (UI systém není souèástí)
        foreach (var upgrade in upgradeChoices)
        {
            Debug.Log($"Upgrade Option: {upgrade.name} - {upgrade.description}");
        }

        // Po výbìru voláme ApplyUpgrade s vybraným upgradem
    }

    private List<UpgradeOption> GetRandomUpgrades(int count)
    {
        List<UpgradeOption> randomUpgrades = new List<UpgradeOption>();
        for (int i = 0; i < count && availableUpgrades.Count > 0; i++)
        {
            int index = Random.Range(0, availableUpgrades.Count);
            randomUpgrades.Add(availableUpgrades[index]);
        }
        return randomUpgrades;
    }

    public void ApplyUpgrade(UpgradeOption upgrade)
    {
        upgrade.Apply(shipStats); // Aplikujeme vybrané vylepšení na loï
        Debug.Log($"Applied upgrade: {upgrade.name}");
    }
}