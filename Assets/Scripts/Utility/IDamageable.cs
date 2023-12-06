public interface IDamageable
{
    bool ApplyDamage(DamageMessage damageMessage);

    //�_���[�W����

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