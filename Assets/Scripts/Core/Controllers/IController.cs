interface IController
{
    /** Ovl�d�n� entity v kontrole. */
    public void Controll();

    public void TakeDamage(float damage, float? criticalStrike = null);
}