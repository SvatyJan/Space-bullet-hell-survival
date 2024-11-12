using UnityEngine;

public class EnemyBase : MonoBehaviour, IEnemy
{
    [SerializeField] protected int health = 20;
    [SerializeField] protected float speed = 3f;
    [SerializeField] protected int damage = 10;

    protected Transform player;

    public int Health => health;

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void Update()
    {

    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var ship = collision.gameObject.GetComponent<ShipBase>();
            ship?.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
