using UnityEngine;

public class Blaster : MonoBehaviour, IWeapon
{
    public GameObject projectilePrefab;   // Prefab projektilu
    public Transform shootingPoint;       // V�choz� bod st�ely
    public float fireRate = 0.5f;         // Interval mezi v�st�ely
    private float nextFireTime = 0f;      // �as pro p��t� v�st�el

    private float baseDamage = 10f;       // Z�kladn� po�kozen� zbran�
    public SpaceEntity owner;             // Odkaz na vlastn�ka zbran�
    private void Start()
    {
        // Automaticky p�i�ad� vlastn�ka z parent objektu
        baseDamage = GetComponent<ShipStats>().BaseDamage;
        owner = GetComponentInParent<SpaceEntity>();
    }

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
                projectileScript.Initialize(owner, baseDamage);
                projectileScript.SetDirection(shootingPoint.up); // Nastav� sm�r podle v�choz�ho bodu
            }
        }
    }
}