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

    /** Seznam tagů, na které může být projektil vystřelen. */
    [SerializeField] private List<string> collisionTags;

    [SerializeField] private List<GameObject> hitTargets = new List<GameObject>();

    private SpaceEntity owner;

    private void Start()
    {
        shootingPoint = transform;
        owner = GetComponentInParent<SpaceEntity>();
    }

    public void Fire()
    {
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;

            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f;

            int rocketsShot = owner.getShipStats().ProjectilesCount;

            for (int i = 0; i < rocketsShot; i++)
            {
                StartFiringRockets(shootingPoint, mousePosition);
            }
        }
    }

    public void StartFiringRockets(Transform launchPoint, Vector3 targetPosition)
    {
        StartCoroutine(FireRocketsCoroutine(launchPoint, targetPosition));
    }

    private IEnumerator FireRocketsCoroutine(Transform launchPoint, Vector3 targetPosition)
    {
        for (int i = 0; i < owner.getShipStats().ProjectilesCount; i++)
        {
            GameObject rocketInstance = Instantiate(rocketPrefab, launchPoint.position, launchPoint.rotation);
            Rocket rocketScript = rocketInstance.GetComponent<Rocket>();

            GameObject closestEnemy = FindClosestEnemyFromPosition(targetPosition);
            if (rocketScript != null)
            {
                rocketScript.Initialize(GetComponentInParent<SpaceEntity>(), baseDamage);
                rocketScript.SetTarget(closestEnemy);
            }

            yield return new WaitForSeconds(0.1f);
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
        throw new System.NotImplementedException();
    }

    public void Evolve()
    {
        throw new System.NotImplementedException();
    }
}
