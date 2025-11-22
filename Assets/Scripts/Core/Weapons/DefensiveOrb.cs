using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class DefensiveOrb : MonoBehaviour, IWeapon
{
    [Header("Orb Settings")]
    /** Prefab orb projektilu. */
    public GameObject orbPrefab;

    /** Výchozí vzdálenost obíhání kolem hráèe. */
    public float baseOrbitRadius = 2f;

    /** Výchozí rychlost obíhání orbù. */
    public float baseOrbitSpeed = 180f;

    [Header("Runtime")]
    /** Aktuální seznam aktivních orbù. */
    private List<GameObject> orbs = new List<GameObject>();

    /** Odkaz na ShipStats hráèe. */
    private ShipStats shipStats;

    /** Odkaz na PlayerProgression pro pøidávání XP. */
    private PlayerProgression playerProgression;

    /** Poslední známý poèet projektilù (orbù). */
    private int lastProjectileCount = -1;

    /** Pøíznak, zda je orb ve stavu evoluce. */
    private bool isEvolved = false;

    [Header("Object pooling")]
    private ObjectPool<GameObject> ProjectilePool;

    private void Start()
    {
        CreateProjectilePool();
        shipStats = GetComponentInParent<ShipStats>();
        playerProgression = GetComponentInParent<PlayerProgression>();

        if (shipStats == null)
        {
            Debug.LogError("DefensiveOrb: ShipStats not found on parent!");
            return;
        }

        UpdateOrbCount();
    }

    private void Update()
    {
        if (shipStats != null && shipStats.ProjectilesCount != lastProjectileCount)
        {
            UpdateOrbCount();
        }
    }

    private void UpdateOrbCount()
    {
        lastProjectileCount = shipStats.ProjectilesCount;
        SpawnOrbs(lastProjectileCount);
    }

    private void SpawnOrbs(int count)
    {
        foreach (var orb in orbs)
        {
            ReleaseProjectileFromPool(orb);
        }
        orbs.Clear();

        for (int i = 0; i < count; i++)
        {
            GameObject newOrb = Instantiate(orbPrefab);
            newOrb.transform.parent = null;

            Orb orbScript = newOrb.GetComponent<Orb>();
            if (orbScript == null)
            {
                Debug.LogWarning("Orb prefab missing Orb script!");
                ReleaseProjectileFromPool(newOrb);
                continue;
            }

            float angle = (360f / count) * i;
            orbScript.Initialize(this, transform, baseOrbitSpeed, baseOrbitRadius, angle, playerProgression, isEvolved);

            orbs.Add(newOrb);
        }
    }

    private void CreateProjectilePool()
    {
        ProjectilePool = new ObjectPool<GameObject>(
            () => { return Instantiate(orbPrefab); },
        projectile => { projectile.gameObject.SetActive(true); },
        projectile => { projectile.gameObject.SetActive(false); },
        projectile => { Destroy(projectile); },
        false, 10, 20
        );
    }

    public void ReleaseProjectileFromPool(GameObject projectile)
    {
        ProjectilePool.Release(projectile);
    }

    public void Fire() {}

    public void Upgrade()
    {
        baseOrbitSpeed += 10f;
        UpdateOrbCount();
    }

    public void Downgrade()
    {
        baseOrbitSpeed -= 10f;
        UpdateOrbCount();
    }

    public void Evolve()
    {
        baseOrbitRadius += 1.5f;
        baseOrbitSpeed += 10f;
        isEvolved = true;
        UpdateOrbCount();
    }
}
