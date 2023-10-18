using System;
using UnityEngine;

//体力を持ってるすべての親クラス
//ダメージを受ける
public class LivingEntity : MonoBehaviour, IDamageable   
{
    public float startingHealth = 100f;
    public float health { get; protected set; }
    public bool dead { get; protected set; }

    public event Action OnDeath;

    private const float minTimeBetDamaged = 0.1f; //攻撃を受けて次の攻撃を受けるまでの時間
    private float lastDamagedTime; //最後に攻撃を受けた時間

    //攻撃を受けてから十分な時間がたっているかチェック
    protected bool IsInvulnerabe
    {
        get
        {
            if (Time.time >= lastDamagedTime + minTimeBetDamaged) return false;

            return true;
        }
    }

    protected virtual void OnEnable()
    {
        dead = false;
        health = startingHealth;
    }
    //ダメージを受ける
    public virtual bool ApplyDamage(DamageMessage damageMessage) //IDamageable
    {
        if (IsInvulnerabe || damageMessage.damager == gameObject || dead) return false; //무적상태/공격자가 자신/죽음

        lastDamagedTime = Time.time;
        health -= damageMessage.amount;
        if (health <= 0)
        {
            health = 0;
            Die();
        } 

        return true;
    }
    //体力回復
    public virtual void RestoreHealth(float newHealth)
    {
        if (dead) return;

        health += newHealth;
    }
    //Die
    public virtual void Die()
    {
        if (OnDeath != null) OnDeath();

        dead = true;
    }
}