using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class PlayerUpgradeDisplayUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerProgression progression;

    [Header("UI Parents")]
    [SerializeField] private Transform statParent;
    [SerializeField] private Transform weaponParent;

    [Header("Slot Prefab")]
    [SerializeField] private GameObject iconSlotPrefab;

    public List<Image> statIcons = new();
    public List<Image> weaponIcons = new();

    private void OnValidate()
    {
        if (progression == null)
            progression = FindObjectOfType<PlayerProgression>();
    }

    private void OnDestroy()
    {
        if (progression != null)
            progression.OnUpgradesChanged -= UpdateUpgradeIcons;
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => progression != null && progression.shipStats != null);

        progression.OnUpgradesChanged += UpdateUpgradeIcons;

        CreateEmptySlots();
        UpdateUpgradeIcons();
    }

    public void CreateEmptySlots()
    {
        statIcons.Clear();
        weaponIcons.Clear();

        foreach (Transform child in statParent) Destroy(child.gameObject);
        foreach (Transform child in weaponParent) Destroy(child.gameObject);

        // Attribute slots
        for (int i = 0; i < progression.shipStats.maxStatUpgrades; i++)
        {
            GameObject slot = Instantiate(iconSlotPrefab, statParent);
            statIcons.Add(slot.GetComponent<Image>());
        }

        // Weapon slots
        for (int i = 0; i < progression.shipStats.maxWeapons; i++)
        {
            GameObject slot = Instantiate(iconSlotPrefab, weaponParent);
            weaponIcons.Add(slot.GetComponent<Image>());
        }
    }

    public void UpdateUpgradeIcons()
    {
        int statIndex = 0;
        foreach (var kvp in progression.statLevels)
        {
            if (kvp.Value > 0 && statIndex < statIcons.Count)
            {
                StatUpgradeOption statOpt = progression.statUpgrades.Find(x => x.statType == kvp.Key);
                if (statOpt != null)
                {
                    statIcons[statIndex].sprite = statOpt.icon;
                    statIcons[statIndex].color = Color.white;
                    statIndex++;
                }
            }
        }

        int weaponIndex = 0;
        foreach (var kvp in progression.weaponLevels)
        {
            if (kvp.Value > 0 && weaponIndex < weaponIcons.Count)
            {
                weaponIcons[weaponIndex].sprite = kvp.Key.icon;
                weaponIcons[weaponIndex].color = Color.white;
                weaponIndex++;
            }
        }
    }
}
