using UnityEngine;

public class ShipBase : MonoBehaviour, IController
{
    [SerializeField] protected ShipStats shipStats;
    private Vector3 velocity;

    public virtual void Controll()
    {
        // Ovládání pro pohyb dopøedu/zpìt
        float moveInput = Input.GetAxis("Vertical");
        float rotationInput = Input.GetAxis("Horizontal");

        // Ovládání akcelerace a zpomalení
        if (moveInput > 0)
        {
            // Pøidáváme akceleraci pøi pohybu vpøed
            velocity += transform.up * shipStats.acceleration * Time.deltaTime;
        }
        else
        {
            // Zpomalení pøi absenci vstupu
            velocity = Vector3.Lerp(velocity, Vector3.zero, shipStats.deceleration * Time.deltaTime);
        }

        // Omezíme rychlost na maximální hodnotu
        velocity = Vector3.ClampMagnitude(velocity, shipStats.speed);

        // Aplikujeme pohyb
        transform.position += velocity * Time.deltaTime;

        // Rotaèní pohyb
        float rotationStep = rotationInput * shipStats.rotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.forward, -rotationStep);

        // Støelba pøi stisknutí mezerníku
        if (Input.GetKey(KeyCode.Space))
        {
            FireWeapons();
        }
    }

    private void FireWeapons()
    {
        // Získání všech pøipojených zbraní implementujících IWeapon
        IWeapon[] weapons = GetComponents<IWeapon>();
        foreach (IWeapon weapon in weapons)
        {
            weapon?.Fire();
        }
    }

    public void TakeDamage(int damage)
    {
        shipStats.health -= damage;
        if (shipStats.health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
