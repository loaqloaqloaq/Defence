public interface IEnemyDamageable
{   
    bool ApplyDamage(DamageMessage damageMessage);

    //ƒ_ƒ[ƒWˆ—

    public bool IsDead();   
    //public float GetColliderSize()
}

public interface IEnemyDamageable : IEnemyDamageable
{    
    bool ApplyDamage(DamageMessage damageMessage,Part part);    

}

