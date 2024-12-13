
public interface IDamagable
{
    /// <summary>
    /// Reduces Character Health Life, if health is lower than zero handles death
    /// </summary>
    /// <param name="healthDamage"></param>
    public void TakeDamage(float healthDamage, int damageAnimation);
}
