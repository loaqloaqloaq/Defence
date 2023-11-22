using UnityEngine;

public class PlayerHealth : LivingEntity
{
    private Animator animator;

    [SerializeField] private AudioData hitSE;
    [SerializeField] private AudioData deadSE;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        SoundManager.Instance.AddAudioInfo(hitSE);
        SoundManager.Instance.AddAudioInfo(deadSE);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        UpdateUI(false);
    }

    //EÜÖ
    public override void RestoreHealth(float newHealth)
    {
        if (health + newHealth > startingHealth)
        {
            health = startingHealth;
        }
        else
        {
            base.RestoreHealth(newHealth);
        }
        UpdateUI(false);
    }

    private void UpdateUI(bool anim)
    {
        UIManager.Instance.UpdateHealth(startingHealth, health, anim);
        //UIManager.Instance.UpdateHealthText(health);
    }

    public override bool ApplyDamage(DamageMessage damageMessage)
    {
        if (!base.ApplyDamage(damageMessage)) return false;
        EffectManager.Instance.PlayHitEffect(damageMessage.hitPoint, damageMessage.hitNormal,
            transform, EffectManager.EffectType.Flesh);

        SoundManager.Instance.PlaySE(hitSE.name);

        UpdateUI(true);
        return true;
    }

    public override void Die()
    {
        base.Die();
        SoundManager.Instance.PlaySE(deadSE.name);
        animator.SetTrigger("Die");

        UpdateUI(false);
    }
}
