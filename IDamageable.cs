public interface IEnemyDamageable
{   
    bool ApplyDamage(DamageMessage damageMessage);

    //ダメージ処理

    public bool IsDead();   
    //public float GetColliderSize()
}

public interface IEnemyDamageable : IEnemyDamageable
{    
    bool ApplyDamage(DamageMessage damageMessage,Part part);    

}

