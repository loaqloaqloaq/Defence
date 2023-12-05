using UnityEngine;

public class DamagerFire : MonoBehaviour
{
    private IDamageable m_Damageable;
    private Transform p_Transform;

    [SerializeField] private float duration = 3.0f;
    private float remain = 0f;

    private float damagedTime = 0f;
    private float damageAmount = 0f;

    [Range(0, 3.0f)][SerializeField] private float timeBetDamage = 0.3f;

    public void ReNew()
    {
        remain = duration;
    }

    public void Initialize(Transform parent, float amount)
    {
        p_Transform = parent;
        damageAmount = amount;
    }

    public void SetTarget(IDamageable target)
    {
        m_Damageable = target;
        if (m_Damageable == null)
        {
            gameObject.SetActive(false);
            transform.parent = p_Transform;
            return;
        }

        remain = duration;
    }

    void FixedUpdate()
    {
        bool isDead = m_Damageable.IsDead();

        if (remain <= 0 || isDead)
        {
            gameObject.SetActive(false);
            transform.parent = p_Transform;
            return;
        }

        if (Time.time > damagedTime + timeBetDamage)
        {
            DamageProcess();
            damagedTime = Time.time;
        }

        remain -= Time.fixedDeltaTime;

    }

    private void DamageProcess()
    {
        DamageMessage damageMessage;
        damageMessage.damager = gameObject;
        damageMessage.amount = damageAmount;
        damageMessage.attackType = AttackType.Fire;

        damageMessage.hitPoint = transform.position;
        damageMessage.hitNormal = Vector3.up;
        m_Damageable.ApplyDamage(damageMessage);

    }
}
