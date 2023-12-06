using System.Collections;
using UnityEngine;

public class DamagerFire : MonoBehaviour
{
    private IDamageable m_Damageable;
    private Transform p_Transform;

    [SerializeField] private float duration = 3.0f;
    private float remain = 0f;

    private float damagedTime = 0f;
    private float damageAmount = 0f;

    private GameObject damager;

    [Range(0, 3.0f)][SerializeField] private float timeBetDamage = 0.3f;

    private ParticleSystem vfx;
    [SerializeField] private float vfxDelay = 0.4f;
    private float vfxPlayedTime;

    private bool stanby = false;

    private void Awake()
    {
        vfx = GetComponentInChildren<ParticleSystem>();
    }

    public void ReNew()
    {
        remain = duration;
    }

    public void Initialize(Transform parent, float amount)
    {
        p_Transform = parent;
        damageAmount = amount;
    }

    public void SetTarget(IDamageable target, GameObject damager = null)
    {
        m_Damageable = target;
        if (m_Damageable == null)
        {
            gameObject.SetActive(false);
            transform.parent = p_Transform;
            return;
        }

        this.damager = damager;

        remain = duration;
    }

    void FixedUpdate()
    {
        if (stanby) return;

        bool isDead = m_Damageable.IsDead();

        if (remain <= 0 || isDead)
        {
            StartCoroutine(Stanby());
            return;
        }

        if (vfxPlayedTime + vfxDelay < Time.time)
        {
            vfx.Emit(1);
            vfxPlayedTime = Time.time;
        }

        if (Time.time > damagedTime + timeBetDamage)
        {
            DamageProcess();
            damagedTime = Time.time;
        }

        remain -= Time.fixedDeltaTime;
    }

    private IEnumerator Stanby()
    {
        stanby = true;

        yield return new WaitForSeconds(1.0f);
        
        gameObject.SetActive(false);
        transform.parent = p_Transform;
        
        stanby = false;
    }

    private void DamageProcess()
    {
        DamageMessage damageMessage;

        damageMessage.damager = damager;
        damageMessage.amount = damageAmount;
        damageMessage.attackType = AttackType.Fire;
        damageMessage.hitPoint = transform.position;
        damageMessage.hitNormal = Vector3.up;

        m_Damageable.ApplyDamage(damageMessage);
    }
}
