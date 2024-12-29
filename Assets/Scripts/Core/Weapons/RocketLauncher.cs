using UnityEngine;

public class RocketLauncher : MonoBehaviour, IWeapon
{
    public GameObject rocketPrefab;    // Prefab rakety
    public Transform leftLaunchPoint;  // Levý bod odpálení
    public Transform rightLaunchPoint; // Pravý bod odpálení
    public float fireRate = 1f;        // Rychlost střelby
    private float nextFireTime = 0f;
    private float baseDamage = 10f;    // Základní poškození zbraně

    private void Awake()
    {
        // Najdeme levý a pravý střelecký bod podle tagu
        GameObject leftPoint = GameObject.FindGameObjectWithTag("LeftShootingPoint");
        GameObject rightPoint = GameObject.FindGameObjectWithTag("RightShootingPoint");

        if (leftPoint != null)
        {
            leftLaunchPoint = leftPoint.transform;
        }
        else
        {
            Debug.LogError("RocketLauncher: LeftShootingPoint tag not found!");
        }

        if (rightPoint != null)
        {
            rightLaunchPoint = rightPoint.transform;
        }
        else
        {
            Debug.LogError("RocketLauncher: RightShootingPoint tag not found!");
        }
    }

    public void Fire()
    {
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;

            // Získáme světové souřadnice pozice myši
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f; // Ujistíme se, že Z souřadnice je rovná 0

            // Vystřelíme dvě rakety směrem k myši
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
        // Spočítáme směr od bodu odpalu směrem k pozici myši
        Vector3 direction = (targetPosition - launchPoint.position).normalized;

        // Vytvoříme instanci rakety
        GameObject rocketInstance = Instantiate(rocketPrefab, launchPoint.position, Quaternion.identity);

        // Nastavíme směr a vlastnosti rakety
        Rocket rocketScript = rocketInstance.GetComponent<Rocket>();
        if (rocketScript != null)
        {
            rocketScript.Initialize(GetComponentInParent<SpaceEntity>(), baseDamage);
            rocketScript.SetDirection(direction);
        }
    }
}
