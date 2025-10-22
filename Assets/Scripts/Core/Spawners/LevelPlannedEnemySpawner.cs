using System.Collections;
using UnityEngine;

public class LevelPlannedEnemySpawner : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Transform player;

    [Header("Timing")]
    [SerializeField] private float levelDurationSeconds = 20f;

    [Header("Spawn Area")]
    [SerializeField] private float spawnRadius = 15f;
    [SerializeField] private float safeRadius = 10f;
    [SerializeField] private LayerMask invalidSpawnLayers;

    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject chaserPrefab;       // XP 1
    [SerializeField] private GameObject spitterPrefab;      // XP 2
    [SerializeField] private GameObject eyePrefab;          // XP 3
    [SerializeField] private GameObject mediumEyePrefab;    // XP 10
    [SerializeField] private GameObject bigEyePrefab;       // XP 50
    [SerializeField] private GameObject bigSpitterPrefab;   // XP 50
    [SerializeField] private GameObject guardianPrefab;     // XP 100

    [System.Serializable]
    public struct LevelRow
    {
        public int Chaser, Spitter, Eye, MediumEye, BigEye, BigSpitter, Guardian;
    }

    // 1 øádek = 1 level = 20 s okno (lze zmìnit levelDurationSeconds)
    [SerializeField]
    private LevelRow[] plan = new LevelRow[]
    {
        new LevelRow{Chaser=4,   Spitter=0,  Eye=0,  MediumEye=0, BigEye=0, BigSpitter=0, Guardian=0},   // 1
        new LevelRow{Chaser=10,  Spitter=0,  Eye=0,  MediumEye=0, BigEye=0, BigSpitter=0, Guardian=0},   // 2
        new LevelRow{Chaser=10,  Spitter=4,  Eye=0,  MediumEye=0, BigEye=0, BigSpitter=0, Guardian=0},   // 3
        new LevelRow{Chaser=12,  Spitter=4,  Eye=2,  MediumEye=0, BigEye=0, BigSpitter=0, Guardian=0},   // 4
        new LevelRow{Chaser=10,  Spitter=7,  Eye=4,  MediumEye=0, BigEye=0, BigSpitter=0, Guardian=0},   // 5
        new LevelRow{Chaser=12,  Spitter=6,  Eye=4,  MediumEye=1, BigEye=0, BigSpitter=0, Guardian=0},   // 6
        new LevelRow{Chaser=12,  Spitter=7,  Eye=4,  MediumEye=2, BigEye=0, BigSpitter=0, Guardian=0},   // 7
        new LevelRow{Chaser=10,  Spitter=7,  Eye=5,  MediumEye=3, BigEye=0, BigSpitter=0, Guardian=0},   // 8
        new LevelRow{Chaser=10,  Spitter=7,  Eye=6,  MediumEye=4, BigEye=0, BigSpitter=0, Guardian=0},   // 9
        new LevelRow{Chaser=10,  Spitter=7,  Eye=4,  MediumEye=1, BigEye=1, BigSpitter=0, Guardian=0},   //10
        new LevelRow{Chaser=6,   Spitter=7,  Eye=4,  MediumEye=3, BigEye=1, BigSpitter=0, Guardian=0},   //11
        new LevelRow{Chaser=27,  Spitter=7,  Eye=5,  MediumEye=2, BigEye=0, BigSpitter=1, Guardian=0},   //12
        new LevelRow{Chaser=21,  Spitter=10, Eye=7,  MediumEye=3, BigEye=0, BigSpitter=1, Guardian=0},   //13
        new LevelRow{Chaser=11,  Spitter=7,  Eye=5,  MediumEye=2, BigEye=0, BigSpitter=0, Guardian=1},   //14
        new LevelRow{Chaser=10,  Spitter=18, Eye=0,  MediumEye=3, BigEye=0, BigSpitter=2, Guardian=0},   //15
        new LevelRow{Chaser=49,  Spitter=5,  Eye=5,  MediumEye=2, BigEye=0, BigSpitter=0, Guardian=1},   //16
        new LevelRow{Chaser=51,  Spitter=20, Eye=7,  MediumEye=0, BigEye=1, BigSpitter=1, Guardian=0},   //17
        new LevelRow{Chaser=10,  Spitter=7,  Eye=6,  MediumEye=4, BigEye=0, BigSpitter=1, Guardian=1},   //18
        new LevelRow{Chaser=100, Spitter=50, Eye=0,  MediumEye=0, BigEye=0, BigSpitter=1, Guardian=0},   //19
        new LevelRow{Chaser=10,  Spitter=15, Eye=10, MediumEye=10,BigEye=0, BigSpitter=0, Guardian=1},   //20
        new LevelRow{Chaser=0,   Spitter=25, Eye=0,  MediumEye=0, BigEye=0, BigSpitter=5, Guardian=0},   //21
        new LevelRow{Chaser=0,   Spitter=0,  Eye=21, MediumEye=10,BigEye=1, BigSpitter=0, Guardian=1},   //22
        new LevelRow{Chaser=32,  Spitter=0,  Eye=0,  MediumEye=0, BigEye=0, BigSpitter=0, Guardian=3},   //23
        new LevelRow{Chaser=104, Spitter=10, Eye=10, MediumEye=10,BigEye=2, BigSpitter=0, Guardian=0},   //24
        new LevelRow{Chaser=0,   Spitter=60, Eye=2,  MediumEye=10,BigEye=1, BigSpitter=2, Guardian=0},   //25
        new LevelRow{Chaser=0,   Spitter=0,  Eye=100,MediumEye=0, BigEye=2, BigSpitter=0, Guardian=0},   //26
        new LevelRow{Chaser=52,  Spitter=50, Eye=50, MediumEye=2, BigEye=0, BigSpitter=0, Guardian=1},   //27
        new LevelRow{Chaser=446, Spitter=0,  Eye=0,  MediumEye=0, BigEye=0, BigSpitter=0, Guardian=0},   //28
        new LevelRow{Chaser=0,   Spitter=0,  Eye=0,  MediumEye=2, BigEye=1, BigSpitter=0, Guardian=4},   //29
        new LevelRow{Chaser=9,   Spitter=60, Eye=5,  MediumEye=10,BigEye=1, BigSpitter=2, Guardian=1},   //30
        new LevelRow{Chaser=0,   Spitter=1,  Eye=56, MediumEye=15,BigEye=1, BigSpitter=1, Guardian=1},   //31
        new LevelRow{Chaser=1,   Spitter=0,  Eye=15, MediumEye=20,BigEye=6, BigSpitter=0, Guardian=0},   //32
        new LevelRow{Chaser=400, Spitter=0,  Eye=0,  MediumEye=2, BigEye=1, BigSpitter=0, Guardian=1},   //33
        new LevelRow{Chaser=14,  Spitter=60, Eye=4,  MediumEye=0, BigEye=0, BigSpitter=5, Guardian=2},   //34
        new LevelRow{Chaser=0,   Spitter=1,  Eye=124,MediumEye=0, BigEye=5, BigSpitter=0, Guardian=0},   //35
        new LevelRow{Chaser=50,  Spitter=0,  Eye=0,  MediumEye=0, BigEye=0, BigSpitter=0, Guardian=6},   //36
        new LevelRow{Chaser=198, Spitter=50, Eye=50, MediumEye=23,BigEye=0, BigSpitter=0, Guardian=0},   //37
        new LevelRow{Chaser=500, Spitter=2,  Eye=0,  MediumEye=0, BigEye=0, BigSpitter=0, Guardian=2},   //38
        new LevelRow{Chaser=0,   Spitter=81, Eye=0,  MediumEye=7, BigEye=0, BigSpitter=8, Guardian=1},   //39
        new LevelRow{Chaser=0,   Spitter=0,  Eye=100,MediumEye=31,BigEye=3, BigSpitter=0, Guardian=0},   //40
        new LevelRow{Chaser=140, Spitter=0,  Eye=0,  MediumEye=0, BigEye=1, BigSpitter=0, Guardian=6},   //41
        new LevelRow{Chaser=600, Spitter=1,  Eye=2,  MediumEye=1, BigEye=1, BigSpitter=1, Guardian=1},   //42
        new LevelRow{Chaser=300, Spitter=204,Eye=10, MediumEye=11,BigEye=0, BigSpitter=0, Guardian=0},   //43
        new LevelRow{Chaser=150, Spitter=34, Eye=20, MediumEye=10,BigEye=5, BigSpitter=5, Guardian=0},   //44
        new LevelRow{Chaser=0,   Spitter=4,  Eye=0,  MediumEye=30,BigEye=0, BigSpitter=0, Guardian=6},   //45
        new LevelRow{Chaser=300, Spitter=19, Eye=100,MediumEye=0, BigEye=1, BigSpitter=5, Guardian=0},   //46
        new LevelRow{Chaser=200, Spitter=49, Eye=40, MediumEye=50,BigEye=1, BigSpitter=0, Guardian=0},   //47
        new LevelRow{Chaser=0,   Spitter=0,  Eye=0,  MediumEye=0, BigEye=0, BigSpitter=0, Guardian=10},  //48
        new LevelRow{Chaser=1000,Spitter=0,  Eye=10, MediumEye=0, BigEye=0, BigSpitter=0, Guardian=0},   //49
        new LevelRow{Chaser=300, Spitter=50, Eye=30, MediumEye=21,BigEye=2, BigSpitter=2, Guardian=1},   //50
    };

    private void Start()
    {
        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }
        StartCoroutine(RunPlan());
    }

    private IEnumerator RunPlan()
    {
        for (int i = 0; i < plan.Length; i++)
        {
            float start = Time.time;
            float end = start + levelDurationSeconds;

            SpawnOverWindow(chaserPrefab, plan[i].Chaser, start, end);
            SpawnOverWindow(spitterPrefab, plan[i].Spitter, start, end);
            SpawnOverWindow(eyePrefab, plan[i].Eye, start, end);
            SpawnOverWindow(mediumEyePrefab, plan[i].MediumEye, start, end);
            SpawnOverWindow(bigEyePrefab, plan[i].BigEye, start, end);
            SpawnOverWindow(bigSpitterPrefab, plan[i].BigSpitter, start, end);
            SpawnOverWindow(guardianPrefab, plan[i].Guardian, start, end);

            float remain = end - Time.time;
            if (remain > 0) yield return new WaitForSeconds(remain);
            else yield return null;
        }
    }

    private void SpawnOverWindow(GameObject prefab, int count, float windowStart, float windowEnd)
    {
        if (prefab == null || count <= 0) return;
        StartCoroutine(SpawnType(prefab, count, windowStart, windowEnd));
    }

    private IEnumerator SpawnType(GameObject prefab, int count, float windowStart, float windowEnd)
    {
        float window = Mathf.Max(0.01f, windowEnd - windowStart);
        float step = window / count;

        for (int i = 0; i < count; i++)
        {
            float t = windowStart + step * i + Random.Range(-step * 0.25f, step * 0.25f);
            float wait = Mathf.Max(0f, t - Time.time);
            if (wait > 0f) yield return new WaitForSeconds(wait);
            SpawnOne(prefab);
        }
    }

    private void SpawnOne(GameObject prefab)
    {
        if (player == null) return;

        for (int i = 0; i < 10; i++)
        {
            Vector3 pos = player.position + (Vector3)(Random.insideUnitCircle.normalized * spawnRadius);
            if (Vector3.Distance(pos, player.position) >= safeRadius &&
                !Physics2D.OverlapCircle(pos, 0.5f, invalidSpawnLayers))
            {
                Instantiate(prefab, pos, Quaternion.identity);
                return;
            }
        }
    }
}
