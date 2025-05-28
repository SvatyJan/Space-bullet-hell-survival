using System.Collections.Generic;
using UnityEngine;

public class DefensiveOrb : MonoBehaviour, IWeapon
{
    [Header("Orb Settings")]
    public GameObject orbPrefab;
    public float baseOrbitRadius = 2f;
    public float baseOrbitSpeed = 180f;

    private List<GameObject> orbs = new List<GameObject>();
    private ShipStats shipStats;
    private PlayerProgression playerProgression;

    private int lastProjectileCount = -1;
    private bool isEvolved = false;

    private void Start()
    {
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
            Destroy(orb);
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
                Destroy(newOrb);
                continue;
            }

            float angle = (360f / count) * i;
            orbScript.Initialize(transform, baseOrbitSpeed, baseOrbitRadius, angle, playerProgression, isEvolved);

            orbs.Add(newOrb);
        }
    }

    public void Fire() {}

    public void Upgrade()
    {
        baseOrbitSpeed += 10f;
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
