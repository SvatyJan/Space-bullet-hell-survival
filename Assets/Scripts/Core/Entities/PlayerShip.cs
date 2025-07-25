﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShip : SpaceEntity, IController
{
    [SerializeField] private bool controlsEnabled = true;
    [SerializeField] private bool shipDestroyed = false;
    [SerializeField] public GameObject Weapons;
    [SerializeField] private ParticleSystem engineEffect;
    private ParticleSystem.EmissionModule emission;

    private float particlesPerSecond = 50f;

    private Animator animator;
    [SerializeField] private ParticleSystem destroyEffect;

    /** End screen menu. */
    [SerializeField] private GameObject endScreenMenu;

    private void Start()
    {
        if (endScreenMenu == null)
        {
            Debug.LogError("PlayerShip requires end screen menu in UI!");
        }
        else
        {
            endScreenMenu.SetActive(false);
        }

        animator = GetComponent<Animator>();

        if (engineEffect != null)
        {
            emission = engineEffect.emission;
            emission.rateOverTime = 0f;
            engineEffect.Play();
        }

        if (destroyEffect != null)
        {
            destroyEffect.Stop();
        }
    }

    private void Update()
    {
        Controll();
        HealthRegen();
    }

    /** Ovládání. */
    public override void Controll()
    {
        if(!controlsEnabled)
        {
            return;
        }

        if(IsEnemyNearby())
        {
            FireWeapons();
        }

        if (Input.GetMouseButton(0))
        {
            shipStats.Velocity += transform.up * shipStats.Acceleration * Time.deltaTime;
            if(engineEffect != null)
            {
                emission.rateOverTime = particlesPerSecond;
            }
        }
        else
        {
            shipStats.Velocity = Vector3.Lerp(shipStats.Velocity, Vector3.zero, shipStats.Deceleration * Time.deltaTime);
            if (engineEffect != null)
            {
                emission.rateOverTime = 0f;
            }
        }

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;

        Vector3 directionToMouse = (mousePosition - transform.position).normalized;

        float targetAngle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg - 90;

        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, shipStats.RotationSpeed * Time.deltaTime);

        shipStats.Velocity = Vector3.ClampMagnitude(shipStats.Velocity, shipStats.Speed);

        transform.position += shipStats.Velocity * Time.deltaTime;

        AttractXpOrbs();
    }

    /** Vystřeli ze všech zbraní co loď má. */
    public void FireWeapons()
    {
        if(Weapons == null)
        {
            Debug.Log("Weapons not found!");
        }

        foreach (Transform weaponTransform in Weapons.transform)
        {
            IWeapon weapon = weaponTransform.GetComponent<IWeapon>();
            if (weapon != null)
            {
                weapon.Fire();
            }
        }
    }

    /**
     * Odečte životy.
     * Pokud má entita méně životů než 0, tak je zničena.
     */
    public override void TakeDamage(float damage, float? criticalStrike = null)
    {
        if (ReturnBeforeTakeDamage())
        {
            return;
        }

        shipStats.CurrentHealth -= damage;
        //Debug.Log(this.gameObject.name + " took damage: " + damage);

        if (shipStats.CurrentHealth <= 0)
        {
            DestroyShip();
        }
    }
    
    /**
     * Volá akce, které se stanou před tím, než hráč dostane poškožení.
     */
    private bool ReturnBeforeTakeDamage()
    {
        ThermalShield shield = GetComponentInChildren<ThermalShield>();

        if (shield != null && shield.getActiveShield())
        {
            shield.TriggerExplosion();
            shield.setActiveShield(false);
            return true;
        }

        return false;
    }

    private void DestroyShip()
    {
        if(shipDestroyed)
        {
            return;
        }

        StartCoroutine(SmoothGameTimeSlow());
        controlsEnabled = false;
        shipDestroyed = true;
        animator.SetTrigger("destroy");

        if (destroyEffect != null)
        {
            destroyEffect.Play();
        }

        if (engineEffect != null)
        {
            engineEffect.Stop();
        }

        ThermalShield shield = GetComponentInChildren<ThermalShield>();

        if(shield != null)
        {
            shield.gameObject.SetActive(false);
        }
    }

    private IEnumerator SmoothGameTimeSlow()
    {
        Time.timeScale = 0.3f;

        yield return new WaitForSecondsRealtime(1f);

        Time.timeScale = 0.75f;

        yield return new WaitForSecondsRealtime(0.5f);

        Time.timeScale = 1f;
        endScreenMenu.SetActive(true);
    }

    /** Vrátí true jestli je nepřítel v okolí, jinak false. */
    private bool IsEnemyNearby()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, shipStats.DetectionRadius);
        foreach (Collider2D collider in colliders)
        {
            if(collider.CompareTag("Enemy"))
            {
                return true;
            }
        }
        return false;
    }

    /** Přitahuje xp orby. */
    private void AttractXpOrbs()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, shipStats.AttractionRadius);

        foreach (var collider in colliders)
        {
            XPOrb xpOrb = collider.GetComponent<XPOrb>();
            if (xpOrb != null)
            {
                Vector3 direction = (transform.position - xpOrb.transform.position).normalized;
                xpOrb.transform.position += direction * shipStats.AttractionSpeed * Time.deltaTime;
            }
        }
    }

    private void HealthRegen()
    {
        if (shipStats.CurrentHealth < shipStats.MaxHealth)
        {
            float amount = shipStats.HealthRegen * Time.deltaTime;
            shipStats.CurrentHealth = Mathf.Min(shipStats.CurrentHealth + amount, shipStats.MaxHealth);
        }
    }

    public void DisableControlForDuration(float duration)
    {
        if (controlsEnabled)
        {
            controlsEnabled = false;
            StartCoroutine(EnableControlAfterDelay(duration));
        }
    }

    private IEnumerator EnableControlAfterDelay(float time)
    {
        yield return new WaitForSeconds(time);

        if(!shipDestroyed)
        {
            controlsEnabled = true;
        }
    }

    public bool IsControlEnabled()
    {
        return controlsEnabled;
    }
}