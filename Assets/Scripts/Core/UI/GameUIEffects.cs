using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameUIEffects : MonoBehaviour
{
    public static GameUIEffects Instance { get; private set; }

    [Header("Effects")]
    [SerializeField] private Image damageBlurImage;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public static void ShowDamageBlur(float duration = 0.75f)
    {
        Instance.StartCoroutine(Instance.DamageEffectRoutine(duration));
    }

    private IEnumerator DamageEffectRoutine(float duration)
    {
        damageBlurImage.enabled = true;

        float halfDuration = duration / 2f;
        float timer = 0f;

        Color color = damageBlurImage.color;

        while (timer < halfDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, timer / halfDuration);
            damageBlurImage.color = new Color(color.r, color.g, color.b, alpha);
            timer += Time.deltaTime;
            yield return null;
        }

        damageBlurImage.color = new Color(color.r, color.g, color.b, 1f);

        timer = 0f;
        while (timer < halfDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, timer / halfDuration);
            damageBlurImage.color = new Color(color.r, color.g, color.b, alpha);
            timer += Time.deltaTime;
            yield return null;
        }

        damageBlurImage.enabled = false;
        damageBlurImage.color = new Color(color.r, color.g, color.b, 0f);
    }

}
