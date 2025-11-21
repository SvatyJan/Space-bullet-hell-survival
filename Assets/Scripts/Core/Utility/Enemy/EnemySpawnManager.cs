using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    public static EnemySpawnManager Instance { get; private set; }

    public int maxEnemiesOnMap = 150;
    private int currentEnemies = 0;

    private struct Req
    {
        public GameObject prefab;
        public Vector3 pos;
        public Quaternion rot;
        public int priority;
    }

    private List<Req> queue = new();

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void EnqueueSpawn(GameObject prefab, Vector3 pos, Quaternion rot, int priority)
    {
        var r = new Req { prefab = prefab, pos = pos, rot = rot, priority = priority };
        queue.Add(r);
        queue.Sort((a, b) => b.priority.CompareTo(a.priority));
        TrySpawnFromQueue();
    }

    public static void NotifyEnemySpawned()
    {
        Instance.currentEnemies++;
    }

    public static void NotifyEnemyDestroyed()
    {
        Instance.currentEnemies = Mathf.Max(0, Instance.currentEnemies - 1);
        Instance.TrySpawnFromQueue();
    }

    private void TrySpawnFromQueue()
    {
        if (queue.Count == 0) return;

        while (queue.Count > 0 && currentEnemies < maxEnemiesOnMap)
        {
            var r = queue[0];
            queue.RemoveAt(0);

            var obj = EnemyPoolManager.Instance.Get(r.prefab, r.pos, r.rot);
            if (obj != null) NotifyEnemySpawned();
        }
    }
}
