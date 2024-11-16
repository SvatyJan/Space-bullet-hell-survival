using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs; // Prefaby nepřátel
    [SerializeField] private Transform player;          // Reference na hráče
    [SerializeField] private float spawnRadius = 15f;   // Vzdálenost spawnu od hráče
    [SerializeField] private float safeRadius = 10f;    // Bezpečná zóna kolem hráče, kde se nespawnují nepřátelé
    [SerializeField] private float spawnInterval = 3f;  // Interval mezi spawny
    private void Start()
    {
        // Spustíme cyklický spawn
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

    private void SpawnEnemy()
    {
        // Náhodná pozice ve vzdálenosti kolem hráče
        Vector3 spawnPosition;
        do
        {
            spawnPosition = player.position + Random.insideUnitSphere * spawnRadius;
            spawnPosition.z = 0f; // Ujistíme se, že nepřítel je ve 2D rovině
        }
        while (Vector3.Distance(spawnPosition, player.position) < safeRadius);

        // Náhodně vybereme typ nepřítele
        int randomIndex = Random.Range(0, enemyPrefabs.Length);

        // Vytvoříme nepřítele
        Instantiate(enemyPrefabs[randomIndex], spawnPosition, Quaternion.identity);
    }
}