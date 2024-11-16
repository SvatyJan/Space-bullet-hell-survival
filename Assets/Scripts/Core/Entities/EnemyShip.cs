using UnityEngine;

public class EnemyShip : SpaceEntity, IController
{
    private Transform target; // Cíl (hráč nebo jiný objekt)

    [SerializeField] public EnemyBehaviorBase behavior;

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
}