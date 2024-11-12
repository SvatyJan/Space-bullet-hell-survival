public interface IEnemy
{
    void TakeDamage(int damage);
    int Health { get; }
}