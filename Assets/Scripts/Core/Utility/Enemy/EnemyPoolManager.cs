using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyPoolManager : MonoBehaviour
{
    public static EnemyPoolManager Instance { get; private set; }

    /** Maximální pooling size typu nepøátel. */
    [SerializeField] private int maxEnemyTypePoolingSize = 100;

    [System.Serializable]
    public struct PoolEntry
    {
        public GameObject prefab;
    }

    [SerializeField] private PoolEntry[] poolConfig;

    private Dictionary<GameObject, ObjectPool<GameObject>> pools = new();
    private Dictionary<GameObject, GameObject> instanceToPrefab = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        foreach (var entry in poolConfig)
        {
            var prefab = entry.prefab;

            var pool = new ObjectPool<GameObject>(
                () =>
                {
                    var obj = Instantiate(prefab);
                    obj.SetActive(false);
                    return obj;
                },
                obj =>
                {
                    obj.SetActive(true);
                },
                obj =>
                {
                    obj.SetActive(false);
                },
                obj =>
                {
                    Destroy(obj);
                },
                false,
                10,
                maxEnemyTypePoolingSize
            );

            pools[prefab] = pool;
        }
    }

    public GameObject Get(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        if (!pools.ContainsKey(prefab))
            return null;

        var obj = pools[prefab].Get();
        obj.transform.SetPositionAndRotation(pos, rot);

        instanceToPrefab[obj] = prefab;

        var ship = obj.GetComponent<EnemyShip>();
        if (ship != null)
            ship.originPrefab = prefab;

        return obj;
    }

    public void Release(GameObject prefab, GameObject instance)
    {
        if (instanceToPrefab.ContainsKey(instance))
            instanceToPrefab.Remove(instance);

        if (pools.ContainsKey(prefab))
        {
            pools[prefab].Release(instance);
        }
        else
        {
            Destroy(instance);
        }
    }
}
