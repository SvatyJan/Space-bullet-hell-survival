using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncher : MonoBehaviour, IWeapon
{
    [Header("Prefabs")]
    /** Prefab rakety. */
    public GameObject rocketPrefab;

    [Header("Firing Points")]
    /** Pole bodů, ze kterých může být odpálena raketa. */
    [SerializeField] private Transform[] shootingPoints;

    /** Index určující aktuální výstřelový bod. */
    private int currentPointIndex = 0;

    [Header("Attributes")]
    /** Interval mezi výstřely. */
    [SerializeField] private float fireRate = 0.5f;

    /** Čas pro příští výstřel. */
    private float nextFireTime = 0f;

    /** Zpozdění mezi odpaly jednotlivých raket. */
    private float fireRocketDelay = 0.1f;

    /** Poloměr hledání cíle. */
    private float baseRadius = 10f;

    /** Základní poškození zbraně. */
    private float baseDamage = 10f;

    /** Základní počet projektilů. */
    private int baseProjectilesCount = 1;

    [Header("References")]
    /** Odkaz na vlastníka zbraně. */
    private SpaceEntity owner;

    /** Atributy vlastníka zbraně. */
    private ShipStats shipStats;

    [Header("Targeting")]
    /** Seznam tagů, které lze zasáhnout. */
    [SerializeField] private List<string> collisionTags;

    /** Seznam již zasažených cílů v rámci řetězení. */
    [SerializeField] private List<GameObject> hitTargets = new List<GameObject>();


    private void Start()
    {
        owner = GetComponentInParent<SpaceEntity>();
        shipStats = owner?.GetComponent<ShipStats>();

        if (shipStats == null)
        {
            Debug.LogError($"{gameObject.name}: Chybí komponenta ShipStats!");
        }

        // Pokud nejsou nastaveny střelné body, použij vlastní pozici
        if (shootingPoints == null || shootingPoints.Length == 0)
        {
            shootingPoints = new Transform[] { transform };
            Debug.LogWarning("RocketLauncher: shootingPoints nejsou nastaveny, použit vlastní transform.");
        }
    }

    public void Fire()
    {
        if (Time.time >= nextFireTime)
        {
            float totalFireRate = Mathf.Max(0.05f, fireRate * shipStats.FireRate);
            nextFireTime = Time.time + totalFireRate;

            int rocketCount = baseProjectilesCount + shipStats.ProjectilesCount;

            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f;

            Transform launchPoint = shootingPoints[currentPointIndex];
            StartCoroutine(FireRocketsCoroutine(launchPoint, mousePosition, rocketCount));

            currentPointIndex = (currentPointIndex + 1) % shootingPoints.Length;
        }
    }

    private IEnumerator FireRocketsCoroutine(Transform launchPoint, Vector3 targetPosition, int rocketCount)
    {
        for (int i = 0; i < rocketCount; i++)
        {
            GameObject closestEnemy = FindClosestEnemyFromPosition(targetPosition);
            if (closestEnemy == null)
            {
                yield break;
            }

            GameObject rocketInstance = Instantiate(rocketPrefab, launchPoint.position, launchPoint.rotation);
            Rocket rocketScript = rocketInstance.GetComponent<Rocket>();

            if (rocketScript != null)
            {
                float finalDamage = baseDamage + shipStats.BaseDamage;
                float critChance = shipStats.CriticalChance;

                rocketScript.Initialize(owner, finalDamage);
                rocketScript.SetTarget(closestEnemy);
            }

            yield return new WaitForSeconds(fireRocketDelay);
        }
    }

    private GameObject FindClosestEnemyFromPosition(Vector2 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, baseRadius);
        GameObject closestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (var collider in colliders)
        {
            if (collisionTags.Contains(collider.tag) && !hitTargets.Contains(collider.gameObject))
            {
                float distance = Vector2.Distance(position, collider.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestEnemy = collider.gameObject;
                }
            }
        }

        return closestEnemy;
    }

    public void Upgrade()
    {
        baseRadius += 1f;
    }

    public void Evolve()
    {
        baseRadius += 2f;
        baseProjectilesCount += 3;
    }
}
