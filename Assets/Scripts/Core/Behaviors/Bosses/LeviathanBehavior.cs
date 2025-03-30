using UnityEngine;

public class LeviathanBehavior : EnemyBehaviorBase
{
    /** Prefaby pro Chasery a SpitterBullet */
    public GameObject chaserPrefab;
    public GameObject spitterBulletPrefab;
    public GameObject laserPrefab;

    /** �as pro st��d�n� �tok� */
    private float nextAttackTime = 0f;
    private float attackCooldown = 5f; // Interval mezi �toky

    /** Maxim�ln� �hel pro vyst�elen� SpitterBulletu */
    public float spitterBulletAngleRange = 90f;

    /** Doba, po kterou se Leviathan bude ot��et */
    public float turnSpeed = 2f;

    private Laser laserScript;

    private void Start()
    {
        nextAttackTime = Time.time + attackCooldown;

        // P�edpokl�d�me, �e laser je v hierarchii pod Leviathanem, a z�sk�me jeho skript
        laserScript = GetComponentInChildren<Laser>();
        if (laserScript == null)
        {
            Debug.LogError("LeviathanBehavior: Laser nen� p��tomen nebo nen� p�i�azen spr�vn�!");
        }
    }

    public override void Execute()
    {
        if (target == null) return;

        // Ot��en� Leviathana sm�rem k hr��i s pomalej��m ot��en�m
        RotateTowardsTarget();

        // Pokud je �as na �tok
        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown; // Nastav�me nov� �as �toku
            PerformAttack(); // Spust�me �tok
        }
    }

    private void RotateTowardsTarget()
    {
        // V�po�et sm�ru k hr��i
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;

        // Pomalu ot���me Leviathana sm�rem k hr��i
        float step = turnSpeed * Time.deltaTime;
        float currentAngle = transform.rotation.eulerAngles.z;
        float targetAngle = angle - 90f; // Uprav�me podle rotace sprite (pokud je t�eba)

        // Umo�n�me pomal� ot��en�
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, step);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, newAngle));
    }

    private void PerformAttack()
    {
        int attackType = Random.Range(0, 3); // Randomn� vybere typ �toku: 0 = Chasery, 1 = Spitter, 2 = Laser

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
        // Vyvol�me 4 Chasery p�ed sebou
        for (int i = 0; i < 4; i++)
        {
            Vector3 spawnPosition = transform.position + transform.up * (i + 1); // Vytvo��me Chasery p�ed Leviathanem
            Instantiate(chaserPrefab, spawnPosition, transform.rotation);
        }
    }

    private void ShootSpitterBullet()
    {
        // Vyst�el� SpitterBullety v polokruhov� formaci
        float angleStep = spitterBulletAngleRange / 3f; // Rozd�len� �hlu pro polokruh
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
            laserScript.Fire(); // Zavol�n� metody Fire() na laseru, kter� provede raycast a vyst�el� laser
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Debugging: Uk�e vzd�lenost mezi Leviathanem a hr��em
        if (target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, target.position);
        }
    }
}
