using UnityEngine;
using UnityEngine.Pool;

public class XpPooling : MonoBehaviour
{
    [Header("Object pooling")]
    public static XpPooling Instance { get; private set; }

    [SerializeField] private GameObject xpOrbPrefab;

    private ObjectPool<GameObject> XpPool;

    public int maxXpOrbs = 100;
    public int activeXpCount = 0;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        CreateXpPool();
    }

    private void CreateXpPool()
    {
        XpPool = new ObjectPool<GameObject>(
            () =>
            {
                GameObject xpOrb = Instantiate(xpOrbPrefab, transform.position, transform.rotation);
                xpOrb.SetActive(false);

                return xpOrb;
            },
            xpOrb =>
            {
                xpOrb.SetActive(true);
                activeXpCount++;
            },
            xpOrb =>
            {
                xpOrb.SetActive(false);
                activeXpCount--;
            },
            xpOrb =>
            {
                Destroy(xpOrb);
            },
            false,
            50,
            maxXpOrbs
        );
    }

    public GameObject SpawnXp(Vector3 pos, Quaternion rot, float amount)
    {
        if (activeXpCount >= maxXpOrbs)
        {
            GameObject orb = FindFarthestXp();
            if (orb != null)
            {
                XPOrb x = orb.GetComponent<XPOrb>();
                if (x != null) 
                    x.xpAmount += amount;

                orb.transform.SetPositionAndRotation(pos, rot);
                return orb;
            }
        }

        GameObject xp = XpPool.Get();
        xp.transform.SetPositionAndRotation(pos, rot);

        XPOrb orbComp = xp.GetComponent<XPOrb>();
        if (orbComp != null)
            orbComp.xpAmount = amount;

        return xp;
    }

    private GameObject FindFarthestXp()
    {
        XPOrb[] allXpOrbs = FindObjectsOfType<XPOrb>();
        if (allXpOrbs.Length == 0) return null;

        GameObject playingPlayer = ShipController.GetControllingObject();
        Transform playerTransform;
        if (playingPlayer != null)
        {
            playerTransform = playingPlayer.transform;
        }
        else
        {
            playerTransform = transform;
        }

        float maxDist = 0;
        GameObject farthest = null;

        foreach (var xp in allXpOrbs)
        {
            float d = Vector3.Distance(playerTransform.position, xp.transform.position);
            if (d > maxDist)
            {
                maxDist = d;
                farthest = xp.gameObject;
            }
        }

        return farthest;
    }

    public static void ReleaseXpFromPool(GameObject xp)
    {
        Instance.XpPool.Release(xp);
    }
}
