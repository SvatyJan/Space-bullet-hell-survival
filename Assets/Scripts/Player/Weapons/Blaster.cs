using UnityEngine;

public class Blaster : MonoBehaviour, IWeapon
{
    public GameObject projectilePrefab;   // Prefab projektilu, kter� bude zbra� vyst�elovat
    public Transform shootingPoint;       // V�choz� bod st�ely
    public float fireRate = 0.5f;         // Interval mezi v�st�ely
    private float nextFireTime = 0f;      // �as pro p��t� v�st�el

    public void Fire()
    {
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;

            // Vytvo�en� st�ely na pozici a sm�ru `shootingPoint`
            GameObject projectile = Instantiate(projectilePrefab, shootingPoint.position, shootingPoint.rotation);

            // P�id�me komponent� projektilu rychlost sm�rem, kter�m m��� `shootingPoint`
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.SetDirection(shootingPoint.up); // Nastav� sm�r podle v�choz�ho bodu
            }
        }
    }
}
