public interface IDamageable
{
    bool ApplyDamage(DamageMessage damageMessage);

    //�_���[�W����

    public bool IsDead();

    //public float GetColliderSize()
}