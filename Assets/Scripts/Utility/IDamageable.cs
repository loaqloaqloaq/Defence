public interface IDamageable
{
    bool ApplyDamage(DamageMessage damageMessage);

    //ダメージ処理

    public bool IsDead();

    //public float GetColliderSize()
}