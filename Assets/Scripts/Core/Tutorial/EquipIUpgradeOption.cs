using UnityEngine;

public class EquipIUpgradeOption : MonoBehaviour, IInteractable
{
    [Header("Upgrade Reference")]
    [SerializeField] private ScriptableObject upgradeAsset;

    [Header("UI")]
    [SerializeField] private string descriptionText = "Equip Upgrade";

    public string DescriptionText => descriptionText;

    public void Interact(GameObject interactor)
    {
        if (upgradeAsset == null)
        {
            Debug.LogWarning($"{name}: Missing upgrade reference!");
            return;
        }

        if (upgradeAsset is not IUpgradeOption upgradeOption)
        {
            Debug.LogWarning($"{name}: Assigned asset is not IUpgradeOption (must be StatUpgradeOption or WeaponUpgradeOption).");
            return;
        }

        var progression = interactor.GetComponent<PlayerProgression>();
        if (progression == null)
        {
            Debug.LogWarning($"{name}: Interactor has no PlayerProgression component!");
            return;
        }

        progression.AddOrUpgradeUpgrade(upgradeOption);
        Debug.Log($"{name}: Equipped upgrade {upgradeOption.name}");
    }
}
