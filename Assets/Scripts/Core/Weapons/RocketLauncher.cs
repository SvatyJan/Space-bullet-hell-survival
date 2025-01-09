using UnityEngine;

public class RocketLauncher : MonoBehaviour, IWeapon
{
    /** Prefab rakety. */
    public GameObject rocketPrefab;

    /**  Levý bod odpálení. */
    public Transform leftLaunchPoint;

    /** Pravý bod odpálení. */
    public Transform rightLaunchPoint;

    /** Rychlost střelby. */
    public float fireRate = 1f;

    /** Cooldown pro další střelbu. */
    private float nextFireTime = 0f;

    /** Základní poškození zbraně. */
    private float baseDamage = 10f;

    private void Awake()
    {
        GameObject leftPoint = GameObject.FindGameObjectWithTag("LeftShootingPoint");
        GameObject rightPoint = GameObject.FindGameObjectWithTag("RightShootingPoint");

        if (leftPoint != null)
        {
            leftLaunchPoint = leftPoint.transform;
        }
        else
        {
            Debug.LogError("RocketLauncher: LeftShootingPoint tag not found!");
            leftLaunchPoint = transform;
        }

        if (rightPoint != null)
        {
            rightLaunchPoint = rightPoint.transform;
        }
        else
        {
            Debug.LogError("RocketLauncher: RightShootingPoint tag not found!");
            rightLaunchPoint = transform;
        }
    }

    public void Fire()
    {
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;

            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f;

            FireRocket(leftLaunchPoint, mousePosition);
            FireRocket(rightLaunchPoint, mousePosition);
        }
    }

    public void Upgrade()
    {
        throw new System.NotImplementedException();
    }

    public void Evolve()
    {
        throw new System.NotImplementedException();
    }

    private void FireRocket(Transform launchPoint, Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - launchPoint.position).normalized;

        GameObject rocketInstance = Instantiate(rocketPrefab, launchPoint.position, Quaternion.identity);

        Rocket rocketScript = rocketInstance.GetComponent<Rocket>();
        if (rocketScript != null)
        {
            rocketScript.Initialize(GetComponentInParent<SpaceEntity>(), baseDamage);
            rocketScript.SetDirection(direction);
        }
    }
}
