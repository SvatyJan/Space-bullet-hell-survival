using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Fleet : MonoBehaviour, IWeapon
{
    public GameObject fleetShipPrefab;
    public int baseFleetCount = 2;
    public float orbitRadius = 3f;
    public float respawnTime = 5f;
    public float orbitRotateSpeed = 25f;

    private List<GameObject> fleetShips = new List<GameObject>();
    private List<GameObject> formationPoints = new List<GameObject>();
    private Transform shipTransform;
    private float currentAngleOffset;

    [Header("Object pooling")]
    private ObjectPool<GameObject> ProjectilePool;

    private void Start()
    {
        CreateProjectilePool();
        shipTransform = GetComponentInParent<ShipStats>().transform;
        SpawnFleet(baseFleetCount);
    }

    private void Update()
    {
        currentAngleOffset += orbitRotateSpeed * Time.deltaTime;
        UpdateFormationPositions();
    }

    private void SpawnFleet(int count)
    {
        foreach (var ship in fleetShips) ReleaseProjectileFromPool(ship);
        foreach (var point in formationPoints) Destroy(point);
        fleetShips.Clear();
        formationPoints.Clear();

        for (int i = 0; i < count; i++)
        {
            GameObject point = new GameObject($"FleetPoint_{i}");
            formationPoints.Add(point);

            GameObject newShip = Instantiate(fleetShipPrefab, shipTransform.position, Quaternion.identity);
            FleetShipBehavior shipBehavior = newShip.GetComponent<FleetShipBehavior>();
            shipBehavior.SetFollowTarget(point.transform);
            shipBehavior.SetFleetController(this);
            fleetShips.Add(newShip);
        }
    }

    private void UpdateFormationPositions()
    {
        for (int i = 0; i < formationPoints.Count; i++)
        {
            float angle = i * (2 * Mathf.PI / formationPoints.Count) + currentAngleOffset * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * orbitRadius;
            float y = Mathf.Sin(angle) * orbitRadius;
            formationPoints[i].transform.position = shipTransform.position + new Vector3(x, y, 0);
        }
    }

    public void RespawnShip(GameObject destroyedShip)
    {
        StartCoroutine(RespawnCoroutine(destroyedShip));
    }

    private System.Collections.IEnumerator RespawnCoroutine(GameObject ship)
    {
        yield return new WaitForSeconds(respawnTime);
        int index = fleetShips.IndexOf(ship);
        if (index != -1)
        {
            GameObject newShip = Instantiate(fleetShipPrefab, shipTransform.position, Quaternion.identity);
            FleetShipBehavior shipBehavior = newShip.GetComponent<FleetShipBehavior>();
            shipBehavior.SetFollowTarget(formationPoints[index].transform);
            shipBehavior.SetFleetController(this);
            fleetShips[index] = newShip;
        }
    }

    public void Fire() { }

    private void CreateProjectilePool()
    {
        ProjectilePool = new ObjectPool<GameObject>(
            () => { return Instantiate(fleetShipPrefab); },
        projectile => { projectile.gameObject.SetActive(true); },
        projectile => { projectile.gameObject.SetActive(false); },
        projectile => { Destroy(projectile); },
        false, 1, 20
        );
    }

    public void ReleaseProjectileFromPool(GameObject projectile)
    {
        ProjectilePool.Release(projectile);
    }


    public void Upgrade()
    {
        baseFleetCount++;
        SpawnFleet(baseFleetCount);
    }

    public void Downgrade()
    {
        baseFleetCount--;
        SpawnFleet(baseFleetCount);
    }

    public void Evolve()
    {
        baseFleetCount += 2;
        orbitRadius *= 0.85f;
        respawnTime *= 0.75f;
        SpawnFleet(baseFleetCount);
    }
}
