using UnityEngine;

[System.Serializable]
public class ShipStats : MonoBehaviour
{
    [SerializeField] private float speed = 10f;           // Maxim�ln� rychlost
    [SerializeField] private float rotationSpeed = 100f;  // Rychlost rotace
    [SerializeField] private float acceleration = 5f;     // Zrychlen�
    [SerializeField] private float deceleration = 2f;     // Zpomalen�
    [SerializeField] private float maxHealth = 100f;         // Zdrav� lodi
    [SerializeField] private float currentHealth = 100f;         // Zdrav� lodi
    [SerializeField] private float baseDamage = 10f;      // Z�kladn� po�kozen� lodi
    [SerializeField] private Vector3 velocity;            // Rychlost pohybu
    [SerializeField] private float fireRate = 1f;         // Rychlost ptoku
    [SerializeField] private float attackRadius = 1f;     // Oblast �toku
    [SerializeField] private float attractionRadius = 5f; // Radius pro p�ita�en� xp
    [SerializeField] private float attractionSpeed = 2f;  // Rychlost p�itahov�n� xp
    [SerializeField] private float xp = 0f;               // Aktu�ln� po�et xp
    [SerializeField] private float xpNextlevelUp = 15f;   // Po�et xp pro dal�� level

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

    public float MaxHealth
    {
        get { return maxHealth; }
        set { maxHealth = Mathf.Max(0, value); }
    }

    public float CurrentHealth
    {
        get { return currentHealth; }
        set { currentHealth = Mathf.Max(0, value); }
    }

    public Vector3 Velocity
    {
        get { return velocity; }
        set { velocity = value; }
    }

    public float FireRate
    {
        get { return fireRate; }
        set { fireRate = Mathf.Clamp(value, 0, 100); }
    }

    public float AttackRadius
    {
        get { return attackRadius; }
        set { attackRadius = Mathf.Max(0, value); }
    }

    public float AttractionRadius
    {
        get { return attractionRadius; }
        set { attractionRadius = Mathf.Max(0, value); }
    }

    public float AttractionSpeed
    {
        get { return attractionSpeed; }
        set { attractionSpeed = Mathf.Max(0, value); }
    }

    public float XP
    {
        get { return xp; }
        set { xp = Mathf.Max(0, value); }
    }

    public float XpNextLevelUp
    {
        get { return xpNextlevelUp; }
        set { xpNextlevelUp = Mathf.Max(0, value); }
    }
}