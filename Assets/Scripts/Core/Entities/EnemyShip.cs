using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShip : SpaceEntity, IController
{
    [SerializeField] public Transform target;
    [SerializeField] public EnemyBehaviorBase behavior;
    [SerializeField] private GameObject XpOrbPrefab;
    [SerializeField] private GameObject damagePopUpPrefab;

    private float lastDamagePopupTime = 0f;
    private float damagePopupCooldown = 0.1f;
    private static Queue<GameObject> popupPool = new Queue<GameObject>();
    private static int maxPopups = 25;

    private void Awake()
    {
        behavior = GetComponent<EnemyBehaviorBase>();
    }

    private void Start()
    {
        if(target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    private void Update()
    {
        Controll();
    }

    /** Ovládání. Implementované chování. */
    public override void Controll()
    {
        behavior?.Execute();
    }

    /**
     * Odečte životy.
     * Pokud má entita méně životů než 0, tak je zničena.
     */
    public override void TakeDamage(float damage, float? criticalStrike = null)
    {
        var (actualDamage, critColor) = CombatUtils.CalculateCriticalDamage(damage, criticalStrike);

        shipStats.CurrentHealth -= actualDamage;

        // Zobraz popup jen pokud od posledního uběhlo alespoň 0,1s
        if (Time.time - lastDamagePopupTime >= damagePopupCooldown)
        {
            ShowDamagePopup(actualDamage, critColor);
            lastDamagePopupTime = Time.time;
        }

        if (shipStats.CurrentHealth <= 0)
        {
            BioWeaponEffect bioWeaponEffect = GetComponentInChildren<BioWeaponEffect>();
            if (bioWeaponEffect != null)
            {
                bioWeaponEffect.Explode();
            }

            if(XpOrbPrefab != null)
            {
                GameObject xpOrb = Instantiate(XpOrbPrefab, transform.position, transform.rotation);
                xpOrb.GetComponent<XPOrb>().xpAmount = shipStats.XP;
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

        if(popup == null)
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