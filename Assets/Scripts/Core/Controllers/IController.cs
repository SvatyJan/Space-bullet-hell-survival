interface IController
{
    /** Ovládání entity v kontrole. */
    public void Controll();

    public void TakeDamage(float damage, float? criticalStrike = null);
}