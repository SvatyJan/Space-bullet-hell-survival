using UnityEngine;
using UnityEngine.UI;

public class WeaponSlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Image cooldownOverlay;

    private float cooldownTime;
    private float remainingTime;
    private bool coolingDown;

    public void SetCooldown(float duration)
    {
        cooldownTime = duration;
        remainingTime = duration;
        coolingDown = true;

        cooldownOverlay.fillAmount = 1f;
        cooldownOverlay.enabled = true;
    }

    private void Update()
    {
        if (!coolingDown) return;

        remainingTime -= Time.deltaTime;
        float ratio = Mathf.Clamp01(remainingTime / cooldownTime);
        cooldownOverlay.fillAmount = ratio;

        if (remainingTime <= 0f)
        {
            coolingDown = false;
            cooldownOverlay.enabled = false;
        }
    }
}
