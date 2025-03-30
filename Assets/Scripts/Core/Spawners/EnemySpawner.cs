using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private Transform player;

    [SerializeField] private float spawnRadius = 15f;
    [SerializeField] private float safeRadius = 10f;
    [SerializeField] private float spawnInterval = 3f;

    [SerializeField] private LayerMask invalidSpawnLayers; // Vrstva, kde se nesmí spawnovat (např. Debris)

    private void Start()
    {
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
        Vector3 spawnPosition;
        int maxAttempts = 10; // Počet pokusů najít vhodné místo

        for (int i = 0; i < maxAttempts; i++)
        {
            spawnPosition = GetRandomSpawnPosition();

            // Ověření kolize s nežádoucí vrstvou
            if (!Physics2D.OverlapCircle(spawnPosition, 0.5f, invalidSpawnLayers))
            {
                int randomIndex = Random.Range(0, enemyPrefabs.Length);
                Instantiate(enemyPrefabs[randomIndex], spawnPosition, Quaternion.identity);
                break;
            }
            else
            {
                Debug.LogWarning("EnemySpawner: Nepodařilo se najít vhodné spawnovací místo. Zbývající počet pokusů: " + (maxAttempts-i));
            }
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 spawnPosition;
        do
        {
            spawnPosition = player.position + (Vector3)(Random.insideUnitCircle.normalized * spawnRadius);
        }
        while (Vector3.Distance(spawnPosition, player.position) < safeRadius);

        return spawnPosition;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
