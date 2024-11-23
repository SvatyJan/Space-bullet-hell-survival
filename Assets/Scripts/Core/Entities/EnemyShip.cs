using UnityEngine;

public class EnemyShip : SpaceEntity, IController
{
    private Transform target;

    [SerializeField] public EnemyBehaviorBase behavior;
    [SerializeField] private GameObject XpOrbPrefab;

    private void Awake()
    {
        behavior = GetComponent<EnemyBehaviorBase>();
    }

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform; // Najdi hráče jako cíl
    }

    private void Update()
    {
        Controll(); // Řídí nepřítele
    }

    public override void Controll()
    {
        behavior?.Execute(); // Implementované chování
    }

    public override void TakeDamage(float damage)
    {
        shipStats.CurrentHealth -= damage;
        if (shipStats.CurrentHealth <= 0)
        {
            GameObject xpOrb = Instantiate(XpOrbPrefab, transform.position, transform.rotation);
            xpOrb.GetComponent<XPOrb>().xpAmount = shipStats.XP;

            Destroy(gameObject);
        }
    }
}