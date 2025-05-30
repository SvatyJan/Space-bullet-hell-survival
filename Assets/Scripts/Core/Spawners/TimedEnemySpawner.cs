using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedEnemySpawner : MonoBehaviour
{
    [Header("Player Reference")]
    [SerializeField] private Transform player;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnRadius = 15f;
    [SerializeField] private float safeRadius = 10f;
    [SerializeField] private LayerMask invalidSpawnLayers;

    [Header("Enemies")]
    [SerializeField] private List<TimedEnemyEntry> timedEnemies;

    private float gameTime;

    private void Start()
    {
        foreach (var entry in timedEnemies)
        {
            StartCoroutine(SpawnLoop(entry));
        }
    }

    private void Update()
    {
        gameTime += Time.deltaTime;
    }

    private IEnumerator SpawnLoop(TimedEnemyEntry entry)
    {
        while (true)
        {
            if (gameTime >= entry.unlockTime && (entry.lockTime == 0f || gameTime <= entry.lockTime))
            {
                SpawnEnemy(entry.enemyPrefab);
                yield return new WaitForSeconds(1f / Mathf.Max(0.01f, entry.spawnsPerSecond));
            }
            else
            {
                yield return null;
            }
        }
    }


    private void SpawnEnemy(GameObject prefab)
    {
        Vector3 spawnPosition;
        int maxAttempts = 10;

        for (int i = 0; i < maxAttempts; i++)
        {
            spawnPosition = GetRandomSpawnPosition();
            if (!Physics2D.OverlapCircle(spawnPosition, 0.5f, invalidSpawnLayers))
            {
                Instantiate(prefab, spawnPosition, Quaternion.identity);
                return;
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
}


[System.Serializable]
public class TimedEnemyEntry
{
    /** Prefab nepøítele. */
    public GameObject enemyPrefab;

    /** Kdy se tento typ zpøístupní. */
    public float unlockTime;

    /** Kdy se tento typ zamkne. */
    public float lockTime = 0f;

    /** Kolik se jich má spawnout za sekundu. */
    public float spawnsPerSecond;
}

