using UnityEngine;

public class RocketLauncher : MonoBehaviour, IWeapon
{
    public GameObject rocketPrefab;           // Prefab rakety
    public Transform leftLaunchPoint;        // Levý bod odpálení
    public Transform rightLaunchPoint;       // Pravý bod odpálení
    public float fireRate = 1f;              // Rychlost střelby
    private float nextFireTime = 0f;
    private float baseDamage = 10f;          // Základní poškození zbraně

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
