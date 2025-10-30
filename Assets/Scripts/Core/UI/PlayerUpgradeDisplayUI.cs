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
    [SerializeField] private Sprite emptyIconImage;

    public List<Image> statIcons = new();
    public List<Image> weaponIcons = new();

    public void SetProgression(PlayerProgression Progression) => progression = Progression;
    public PlayerProgression GetProgression() => progression;

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
        foreach (var icon in statIcons)
        {
            icon.sprite = emptyIconImage;
            icon.color = new Color(1f, 1f, 1f, 0.5f);
        }

        foreach (var icon in weaponIcons)
        {
            icon.sprite = emptyIconImage;
            icon.color = new Color(1f, 1f, 1f, 0.5f);
        }

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
