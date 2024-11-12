using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;  // Prefaby rùzných typù nepøátel
    public float spawnInterval = 3f;
    public float spawnRadius = 10f;

    private float nextSpawnTime;

    private void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            nextSpawnTime = Time.time + spawnInterval;
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        Vector2 spawnPosition = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;
        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}
