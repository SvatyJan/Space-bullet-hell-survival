using System.Collections.Generic;
using UnityEngine;

public class AllyShip : SpaceEntity, IController
{
    [SerializeField] public Transform target;
    [SerializeField] public FleetShipBehavior behavior;
    [SerializeField] private GameObject damagePopUpPrefab;
    [SerializeField] private GameObject deathEffect;

    private float lastDamagePopupTime = 0f;
    private float damagePopupCooldown = 0.1f;
    private static Queue<GameObject> popupPool = new Queue<GameObject>();
    private static int maxPopups = 25;

    private void Awake()
    {
        behavior = GetComponent<FleetShipBehavior>();
    }

    private void Update()
    {
        Controll();
    }

    public override void Controll()
    {
        behavior?.Execute();
    }

    public override void TakeDamage(float damage, float? criticalStrike = null)
    {
        shipStats.CurrentHealth -= damage;
        if (Time.time - lastDamagePopupTime >= damagePopupCooldown)
        {
            ShowDamagePopup(damage, Color.cyan);
            lastDamagePopupTime = Time.time;
        }
        if (shipStats.CurrentHealth <= 0)
        {
            if (deathEffect != null)
            {
                GameObject deathEffectInstance = Instantiate(deathEffect, transform.position, Quaternion.identity);
                Destroy(deathEffectInstance, 1f);
            }
            Destroy(gameObject);
        }
    }

    private void ShowDamagePopup(float damage, Color critColor)
    {
        GameObject popup;
        if (popupPool.Count < maxPopups)
        {
            popup = Instantiate(damagePopUpPrefab);
        }
        else
        {
            popup = popupPool.Dequeue();
        }
        if (popup == null)
        {
            return;
        }
        else
        {
            popup.transform.position = transform.position;
            popup.GetComponent<DamagePopup>().Setup(damage, critColor);
            popupPool.Enqueue(popup);
        }
    }
}
