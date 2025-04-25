using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncher : MonoBehaviour, IWeapon
{
    /** Prefab rakety. */
    public GameObject rocketPrefab;

    /** Výchozí bod odpálení rakety. */
    public Transform shootingPoint;

    /** Rychlost střelby. */
    public float fireRate = 1f;

    /** Cooldown pro další střelbu. */
    private float nextFireTime = 0f;    
    
    /** Zpozdění střelby raket. */
    private float fireRocketDelay = 0.1f;

    /** Vzádlenost ve které se vyhledávají nepřátelé. */
    private float baseRadius = 10f;

    /** Základní poškození zbraně. */
    private float baseDamage = 10f;

    /** Základní počet střel. */
    private int baseProjectilesCount = 1;

    /** Seznam tagů, na které může být projektil vystřelen. */
    [SerializeField] private List<string> collisionTags;

    [SerializeField] private List<GameObject> hitTargets = new List<GameObject>();

    private SpaceEntity owner;

    private void Start()
    {
        shootingPoint = transform;

        if (owner == null)
        {
            owner = GetComponentInParent<SpaceEntity>();

            if (owner == null)
            {
                owner = transform.root.GetComponentInChildren<SpaceEntity>();
            }

            if (owner == null)
            {
                Debug.LogError($"{gameObject.name}: SpaceEntity nebyl nalezen ani v předcích ani v rootu.");
            }
        }
    }

    public void Fire()
    {
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;

            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f;

            int rocketsShot = baseProjectilesCount + owner.getShipStats().ProjectilesCount;
            StartCoroutine(FireRocketsCoroutine(shootingPoint, mousePosition, rocketsShot));
        }
    }

    private IEnumerator FireRocketsCoroutine(Transform launchPoint, Vector3 targetPosition, int rocketCount)
    {
        for (int i = 0; i < rocketCount; i++)
        {
            GameObject rocketInstance = Instantiate(rocketPrefab, launchPoint.position, launchPoint.rotation);
            Rocket rocketScript = rocketInstance.GetComponent<Rocket>();

            GameObject closestEnemy = FindClosestEnemyFromPosition(targetPosition);
            if (rocketScript != null)
            {
                rocketScript.Initialize(owner, baseDamage);
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
                float enemyDistance = Vector2.Distance(position, collider.transform.position);
                if (enemyDistance < minDistance)
                {
                    minDistance = enemyDistance;
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
