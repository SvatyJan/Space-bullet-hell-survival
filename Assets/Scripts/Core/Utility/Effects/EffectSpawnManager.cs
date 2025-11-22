using System.Collections.Generic;
using UnityEngine;

public class EffectSpawnManager : MonoBehaviour
{
    public static EffectSpawnManager Instance { get; private set; }

    public int maxEffectsOnMap = 200;
    private int currentEffects = 0;

    private struct Req
    {
        public GameObject prefab;
        public Vector3 pos;
        public SpaceEntity target;
        public SpaceEntity owner;
        public int priority;
    }

    private List<Req> queue = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void EnqueueSpawn(GameObject prefab, Vector3 pos, SpaceEntity target, SpaceEntity owner, int priority = 0)
    {
        var r = new Req { prefab = prefab, pos = pos, target = target, owner = owner, priority = priority };
        queue.Add(r);
        queue.Sort((a, b) => b.priority.CompareTo(a.priority));
        TrySpawnFromQueue();
    }

    public void NotifyEffectSpawned()
    {
        currentEffects++;
    }

    public void NotifyEffectEnded()
    {
        currentEffects = Mathf.Max(0, currentEffects - 1);
        TrySpawnFromQueue();
    }

    private void TrySpawnFromQueue()
    {
        if (queue.Count == 0) return;

        while (queue.Count > 0 && currentEffects < maxEffectsOnMap)
        {
            var r = queue[0];
            queue.RemoveAt(0);

            var obj = EffectPoolManager.Instance.Get(r.prefab, r.pos);
            if (obj == null) continue;

            var eff = obj.GetComponent<BioWeaponEffect>();
            if (eff != null)
                eff.StartEffect(r.target, r.owner);

            NotifyEffectSpawned();
        }
    }
}
