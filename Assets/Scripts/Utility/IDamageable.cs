public interface IDamageable
{
    bool ApplyDamage(DamageMessage damageMessage);

    //�_���[�W����
    public void Damage(int damage);
}