using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    public float moveSpeed = 1.5f;
    public float fadeOutTime = 2f;

    private TextMeshPro textMesh;
    private Color textColor;
    private float timer;

    public void Setup(float damage)
    {
        textMesh = GetComponent<TextMeshPro>();

        float displayedDamage = damage > 0 ? Mathf.Ceil(damage) : 0;

        textMesh.text = displayedDamage.ToString("F0");
        textColor = textMesh.color;

        Destroy(gameObject, fadeOutTime);
    }

    private void Update()
    {
        transform.position += new Vector3(0, moveSpeed * Time.deltaTime, 0);
        timer += Time.deltaTime;

        if (timer > fadeOutTime * 0.5f)
        {
            textColor.a -= Time.deltaTime / fadeOutTime;
            textMesh.color = textColor;
        }
    }
}
