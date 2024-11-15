using Assets.Scripts.Enemies.Behaviors;
using UnityEngine;

public class EnemyShip : SpaceEntity, IController
{
    private Transform target; // Cíl (hráč nebo jiný objekt)

    protected new ShipStats shipStats;

    private EnemyBehaviorBase behavior;

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
        behavior?.Execute(this); // Delegace na připojené chování
    }

    /*public override void Controll()
    {
        // Sleduj hráče
        Vector3 direction = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, angle), shipStats.RotationSpeed * Time.deltaTime);

        // Pohyb směrem k hráči
        shipStats.Velocity= direction * shipStats.Speed * Time.deltaTime;
        transform.position += shipStats.Velocity;

        // Střelba, pokud je implementována
        FireWeapons();
    }*/
}