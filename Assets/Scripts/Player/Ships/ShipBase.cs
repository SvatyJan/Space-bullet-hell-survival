using UnityEngine;

public class ShipBase : MonoBehaviour, IController
{
    [SerializeField] protected ShipStats shipStats;
    private Vector3 velocity;

    public virtual void Controll()
    {
        // Ovl�d�n� pro pohyb dop�edu/zp�t
        float moveInput = Input.GetAxis("Vertical");
        float rotationInput = Input.GetAxis("Horizontal");

        // Ovl�d�n� akcelerace a zpomalen�
        if (moveInput > 0)
        {
            // P�id�v�me akceleraci p�i pohybu vp�ed
            velocity += transform.up * shipStats.acceleration * Time.deltaTime;
        }
        else
        {
            // Zpomalen� p�i absenci vstupu
            velocity = Vector3.Lerp(velocity, Vector3.zero, shipStats.deceleration * Time.deltaTime);
        }

        // Omez�me rychlost na maxim�ln� hodnotu
        velocity = Vector3.ClampMagnitude(velocity, shipStats.speed);

        // Aplikujeme pohyb
        transform.position += velocity * Time.deltaTime;

        // Rota�n� pohyb
        float rotationStep = rotationInput * shipStats.rotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.forward, -rotationStep);

        // St�elba p�i stisknut� mezern�ku
        if (Input.GetKey(KeyCode.Space))
        {
            FireWeapons();
        }
    }

    private void FireWeapons()
    {
        // Z�sk�n� v�ech p�ipojen�ch zbran� implementuj�c�ch IWeapon
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
