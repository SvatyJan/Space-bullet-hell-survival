using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    /** Prefaby nepřátel. */
    [SerializeField] private GameObject[] enemyPrefabs;

    /** Reference na hráče. */
    [SerializeField] private Transform player;

    /** Vzdálenost spawnu od hráče. */
    [SerializeField] private float spawnRadius = 15f;

    /** Bezpečná zóna kolem hráče, kde se nespawnují nepřátelé. */
    [SerializeField] private float safeRadius = 10f;

    /** Interval mezi spawny nepřátel. */
    [SerializeField] private float spawnInterval = 3f;

    private void Start()
    {
        // Cyklický spawn
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    /** Vytvoří náhodného nepřítele v náhodné oblasti kolem hráče. */
    private void SpawnEnemy()
    {
        Vector3 spawnPosition;
        do
        {
            spawnPosition = player.position + Random.insideUnitSphere * spawnRadius;
            spawnPosition.z = 0f;
        }
        while (Vector3.Distance(spawnPosition, player.position) < safeRadius);

        int randomIndex = Random.Range(0, enemyPrefabs.Length);

        Instantiate(enemyPrefabs[randomIndex], spawnPosition, Quaternion.identity);
    }
}