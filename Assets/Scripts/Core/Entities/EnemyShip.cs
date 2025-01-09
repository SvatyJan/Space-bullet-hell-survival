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
        if(target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    private void Update()
    {
        Controll();
    }

    /** Ovládání. Implementované chování. */
    public override void Controll()
    {
        behavior?.Execute();
    }

    /**
     * Odečte životy.
     * Pokud má entita méně životů než 0, tak je zničena.
     */
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