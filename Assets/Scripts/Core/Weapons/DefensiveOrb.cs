using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefensiveOrb : MonoBehaviour, IWeapon
{
    public GameObject orbPrefab;
    public int baseOrbCount = 1;
    public float baseOrbitRadius = 2f;
    public float baseOrbitSpeed = 180f;

    private List<GameObject> orbs = new List<GameObject>();

    private void Start()
    {
        SpawnOrbs(baseOrbCount);
    }

    private void SpawnOrbs(int count)
    {
        // Nejprve znièíme staré orby
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
            float angle = (360f / count) * i;

            orbScript.Initialize(transform, baseOrbitSpeed, baseOrbitRadius, angle);

            orbs.Add(newOrb);
        }
    }

    public void Fire()
    {
        return;
    }

    public void Upgrade()
    {
        baseOrbCount++;
        baseOrbitSpeed += 30f;
        SpawnOrbs(baseOrbCount);
    }

    public void Evolve()
    {
        throw new System.NotImplementedException();
    }
}
