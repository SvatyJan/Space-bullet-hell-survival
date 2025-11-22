using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EffectPoolManager : MonoBehaviour
{
    public static EffectPoolManager Instance { get; private set; }

    [SerializeField] private int maxPoolingSize = 200;

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
                    //obj.transform.SetParent(null);
                    obj.SetActive(false);
                },
                obj =>
                {
                    Destroy(obj);
                },
                false,
                10,
                maxPoolingSize
            );

            pools[prefab] = pool;
        }
    }

    public GameObject Get(GameObject prefab, Vector3 pos)
    {
        if (!pools.ContainsKey(prefab))
            return null;

        var obj = pools[prefab].Get();
        instanceToPrefab[obj] = prefab;

        obj.transform.position = pos;

        return obj;
    }

    public void Release(GameObject instance)
    {
        if (!instanceToPrefab.TryGetValue(instance, out var prefab))
        {
            Destroy(instance);
            return;
        }

        instanceToPrefab.Remove(instance);

        pools[prefab].Release(instance);
    }
}
