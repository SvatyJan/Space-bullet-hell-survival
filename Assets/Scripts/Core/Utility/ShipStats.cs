using UnityEngine;

[System.Serializable]
public class ShipStats : MonoBehaviour
{
    [SerializeField] private float speed = 10f;           // Maxim�ln� rychlost
    [SerializeField] private float rotationSpeed = 100f;  // Rychlost rotace
    [SerializeField] private float acceleration = 5f;     // Zrychlen�
    [SerializeField] private float deceleration = 2f;     // Zpomalen�
    [SerializeField] private float health = 100f;         // Zdrav� lodi
    [SerializeField] private float baseDamage = 10f;      // Z�kladn� po�kozen� lodi
    [SerializeField] private Vector3 velocity;            // Rychlost pohybu

    public float Speed
    {
        get { return speed; }
        set { speed = Mathf.Max(0, value); }  // Chceme, aby speed nemohl b�t negativn�
    }

    public float RotationSpeed
    {
        get { return rotationSpeed; }
        set { rotationSpeed = Mathf.Max(0, value); }  // Chceme, aby rotationSpeed nemohl b�t negativn�
    }

    public float Acceleration
    {
        get { return acceleration; }
        set { acceleration = Mathf.Max(0, value); }  // Chceme, aby acceleration nemohl b�t negativn�
    }

    public float Deceleration
    {
        get { return deceleration; }
        set { deceleration = Mathf.Max(0, value); }  // Chceme, aby deceleration nemohl b�t negativn�
    }

    public float BaseDamage
    {
        get { return baseDamage; }
        set { baseDamage = value; }
    }
   
    public float Health
    {
        get { return health; }
        set { health = Mathf.Clamp(value, 0, 100); }  // Chceme, aby health byl mezi 0 a 100
    }

    public Vector3 Velocity
    {
        get { return velocity; }
        set { velocity = value; }
    }
}