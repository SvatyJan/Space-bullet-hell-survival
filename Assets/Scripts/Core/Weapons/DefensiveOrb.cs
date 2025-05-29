using System.Collections.Generic;
using UnityEngine;

public class DefensiveOrb : MonoBehaviour, IWeapon
{
    [Header("Orb Settings")]
    /** Prefab orb projektilu. */
    public GameObject orbPrefab;

    /** V�choz� vzd�lenost ob�h�n� kolem hr��e. */
    public float baseOrbitRadius = 2f;

    /** V�choz� rychlost ob�h�n� orb�. */
    public float baseOrbitSpeed = 180f;

    [Header("Runtime")]
    /** Aktu�ln� seznam aktivn�ch orb�. */
    private List<GameObject> orbs = new List<GameObject>();

    /** Odkaz na ShipStats hr��e. */
    private ShipStats shipStats;

    /** Odkaz na PlayerProgression pro p�id�v�n� XP. */
    private PlayerProgression playerProgression;

    /** Posledn� zn�m� po�et projektil� (orb�). */
    private int lastProjectileCount = -1;

    /** P��znak, zda je orb ve stavu evoluce. */
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
