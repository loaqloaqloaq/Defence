public interface IDamageable
{
    bool ApplyDamage(DamageMessage damageMessage);

    //ダメージ処理
    public void Damage(int damage);

    public bool IsDead();

    //public float GetColliderSize()
}