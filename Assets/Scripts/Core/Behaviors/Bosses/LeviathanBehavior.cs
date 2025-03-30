using UnityEngine;

public class LeviathanBehavior : EnemyBehaviorBase
{
    /** Prefaby pro Chasery a SpitterBullet */
    public GameObject chaserPrefab;
    public GameObject spitterBulletPrefab;
    public GameObject laserPrefab;

    /** Èas pro støídání útokù */
    private float nextAttackTime = 0f;
    private float attackCooldown = 5f; // Interval mezi útoky

    /** Maximální úhel pro vystøelení SpitterBulletu */
    public float spitterBulletAngleRange = 90f;

    /** Doba, po kterou se Leviathan bude otáèet */
    public float turnSpeed = 2f;

    private Laser laserScript;

    private void Start()
    {
        nextAttackTime = Time.time + attackCooldown;

        // Pøedpokládáme, že laser je v hierarchii pod Leviathanem, a získáme jeho skript
        laserScript = GetComponentInChildren<Laser>();
        if (laserScript == null)
        {
            Debug.LogError("LeviathanBehavior: Laser není pøítomen nebo není pøiøazen správnì!");
        }
    }

    public override void Execute()
    {
        if (target == null) return;

        // Otáèení Leviathana smìrem k hráèi s pomalejším otáèením
        RotateTowardsTarget();

        // Pokud je èas na útok
        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown; // Nastavíme nový èas útoku
            PerformAttack(); // Spustíme útok
        }
    }

    private void RotateTowardsTarget()
    {
        // Výpoèet smìru k hráèi
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;

        // Pomalu otáèíme Leviathana smìrem k hráèi
        float step = turnSpeed * Time.deltaTime;
        float currentAngle = transform.rotation.eulerAngles.z;
        float targetAngle = angle - 90f; // Upravíme podle rotace sprite (pokud je tøeba)

        // Umožníme pomalé otáèení
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, step);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, newAngle));
    }

    private void PerformAttack()
    {
        int attackType = Random.Range(0, 3); // Randomnì vybere typ útoku: 0 = Chasery, 1 = Spitter, 2 = Laser

        switch (attackType)
        {
            case 0:
                SpawnChasers();
                break;
            case 1:
                ShootSpitterBullet();
                break;
            case 2:
                FireLaser();
                break;
        }
    }

    private void SpawnChasers()
    {
        // Vyvoláme 4 Chasery pøed sebou
        for (int i = 0; i < 4; i++)
        {
            Vector3 spawnPosition = transform.position + transform.up * (i + 1); // Vytvoøíme Chasery pøed Leviathanem
            Instantiate(chaserPrefab, spawnPosition, transform.rotation);
        }
    }

    private void ShootSpitterBullet()
    {
        // Vystøelí SpitterBullety v polokruhové formaci
        float angleStep = spitterBulletAngleRange / 3f; // Rozdìlení úhlu pro polokruh
        for (int i = -1; i <= 1; i++)
        {
            float angle = transform.rotation.eulerAngles.z + i * angleStep;
            Vector3 direction = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0f);
            GameObject bullet = Instantiate(spitterBulletPrefab, transform.position, Quaternion.identity);
            bullet.GetComponent<Projectile>().SetDirection(direction);
        }
    }

    private void FireLaser()
    {
        // Aktivace laseru z oka
        if (laserScript != null)
        {
            laserScript.Fire(); // Zavolání metody Fire() na laseru, která provede raycast a vystøelí laser
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Debugging: Ukáže vzdálenost mezi Leviathanem a hráèem
        if (target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, target.position);
        }
    }
}
