public interface IDamageable
{
    bool ApplyDamage(DamageMessage damageMessage);

    //ダメージ処理
    public void Damage(int damage);
}