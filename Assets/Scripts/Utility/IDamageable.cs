public interface IDamageable
{
    bool ApplyDamage(DamageMessage damageMessage);

    //ƒ_ƒ[ƒWˆ—

    public bool IsDead();    

    //public float GetColliderSize()
}
public interface EnemyPart : IDamageable {
    public Part GetPart();
}
public interface IEnemyDamageable : EnemyPart
{
    bool ApplyDamage(DamageMessage damageMessage, Part part);
}