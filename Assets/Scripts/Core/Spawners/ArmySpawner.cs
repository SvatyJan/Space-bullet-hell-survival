using System.Collections;
using UnityEngine;

public class ArmySpawner : MonoBehaviour
{
    [SerializeField] private GameObject armyPrefab;
    [SerializeField] private float spawnInterval = 20f;
    [SerializeField] private Transform[] spawnPoints;

    private void Start()
    {
        StartCoroutine(SpawnArmyRoutine());
    }

    private IEnumerator SpawnArmyRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnArmies();
        }
    }

    private void SpawnArmies()
    {
        if (armyPrefab == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("ArmySpawner: Chybí armyPrefab nebo nejsou nastaveny spawnPoints!");
            return;
        }

        foreach (Transform spawnPoint in spawnPoints)
        {
            GameObject armyInstance = Instantiate(armyPrefab, spawnPoint.position, spawnPoint.rotation);
            StartCoroutine(DetachAndDestroy(armyInstance));
        }
    }

    private IEnumerator DetachAndDestroy(GameObject armyInstance)
    {
        yield return new WaitForSeconds(0.1f);

        foreach (Transform enemy in armyInstance.transform)
        {
            enemy.SetParent(null);
        }

        Destroy(armyInstance);
    }
}
