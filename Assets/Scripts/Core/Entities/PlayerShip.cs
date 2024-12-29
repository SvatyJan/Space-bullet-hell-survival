using UnityEngine;

public class PlayerShip : SpaceEntity, IController
{
    [SerializeField] private bool controlsEnabled = true;
    [SerializeField] private GameObject Weapons;

    private void Update()
    {
        Controll();
    }

    public override void Controll()
    {
        if(!controlsEnabled)
        {
            return;
        }

        // Střelba při stisknutí mezerníku
        if (Input.GetMouseButton(1))
        {
            FireWeapons();
        }

        // Získání pozice kurzoru ve světových souřadnicích
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; // Nastavíme Z souřadnici na 0 (předcházení nesprávnému posunu)

        // Výpočet směru k myši
        Vector3 directionToMouse = (mousePosition - transform.position).normalized;

        // Výpočet cílového úhlu
        float targetAngle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg - 90;

        // Plynulá interpolace rotace směrem k cílovému úhlu
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, shipStats.RotationSpeed * Time.deltaTime);

        // Kontrola, zda je stisknuto levé tlačítko myši
        if (Input.GetMouseButton(0))
        {
            // Přidáváme rychlost směrem, kterým je loď otočená
            shipStats.Velocity += transform.up * shipStats.Acceleration * Time.deltaTime;
        }
        else
        {
            // Zpomalení při absenci vstupu
            shipStats.Velocity = Vector3.Lerp(shipStats.Velocity, Vector3.zero, shipStats.Deceleration * Time.deltaTime);
        }

        // Omezíme rychlost na maximální hodnotu
        shipStats.Velocity = Vector3.ClampMagnitude(shipStats.Velocity, shipStats.Speed);

        // Aplikujeme pohyb
        transform.position += shipStats.Velocity * Time.deltaTime;

        AttractXpOrbs();
    }

    public void FireWeapons()
    {
        if(Weapons == null)
        {
            Debug.Log("Weapons not found!");
        }

        // Získáme všechny child objekty pod Weapons
        foreach (Transform weaponTransform in Weapons.transform)
        {
            // Pokusíme se získat komponentu IWeapon z každého child objektu
            IWeapon weapon = weaponTransform.GetComponent<IWeapon>();
            if (weapon != null)
            {
                weapon.Fire(); // Zavoláme metodu Fire, pokud zbraň implementuje IWeapon
            }
        }
    }

    public override void TakeDamage(float damage)
    {
        shipStats.CurrentHealth -= damage;
        if (shipStats.CurrentHealth <= 0)
        {
            Debug.Log(this.gameObject.name + " took damage: " + damage);
            controlsEnabled = false;
        }
    }

    private void AttractXpOrbs()
    {
        // Najdeme všechny objekty v dosahu attractionRadius
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, shipStats.AttractionRadius);

        foreach (var collider in colliders)
        {
            // Zkontrolujeme, zda objekt má komponentu XPOrb
            XPOrb xpOrb = collider.GetComponent<XPOrb>();
            if (xpOrb != null)
            {
                // Přitahování orbů k hráči
                Vector3 direction = (transform.position - xpOrb.transform.position).normalized;
                xpOrb.transform.position += direction * shipStats.AttractionSpeed * Time.deltaTime;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, shipStats.AttractionRadius);
    }
}