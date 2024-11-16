using UnityEngine;

public class PlayerShip : SpaceEntity, IController
{
    private void Update()
    {
        Controll(); // Řídí hráče
    }

    public override void Controll()
    {
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
    }
}