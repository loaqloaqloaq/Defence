public interface IEnemyDamageable
{   
    bool ApplyDamage(DamageMessage damageMessage);

    //�_���[�W����

    public bool IsDead();   
    //public float GetColliderSize()
}

public interface IEnemyDamageable : IEnemyDamageable
{    
    bool ApplyDamage(DamageMessage damageMessage,Part part);    

}

