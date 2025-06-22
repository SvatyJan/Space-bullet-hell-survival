using UnityEngine;

public class AWeapon : MonoBehaviour
{
    [Header("Weapon UI")]
    protected WeaponSlotUI slotUI;

    public virtual void SetSlotUI(WeaponSlotUI ui) { }

    protected virtual void TriggerCooldown() { }
}
