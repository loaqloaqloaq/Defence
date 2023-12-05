using UnityEngine;

public class EffectManager : MonoBehaviour
{
    //Singleton
    private static EffectManager m_Instance;
    public static EffectManager Instance
    {
        get
        {
            if (m_Instance == null) m_Instance = FindObjectOfType<EffectManager>();
            return m_Instance;
        }
    }

    //«¨«Õ«§«¯«È«¿«¤«×
    public enum EffectType
    {
        Common,
        Fire,
        Flesh
    }
    
    //VFX ParticleSystem
    [SerializeField] private ParticleSystem commonHitEffectPrefab;
    [SerializeField] private ParticleSystem fleshHitEffectPrefab;
    [SerializeField] private ParticleSystem fireHitEffectPrefab;
   
    //«¨«Õ«§«¯«Èû¾ßE
    public void PlayHitEffect(Vector3 pos, Vector3 normal, Transform parent = null,
        EffectType effectType = EffectType.Common)
    {
        var targetPrefab = commonHitEffectPrefab;

        if (effectType == EffectType.Flesh)
            targetPrefab = fleshHitEffectPrefab;

        if (effectType == EffectType.Fire)
            targetPrefab = fireHitEffectPrefab;

        var effect = Instantiate(targetPrefab, pos, Quaternion.LookRotation(normal));

        if (parent != null)
        {
            effect.transform.SetParent(parent);
        }

        effect.Play();
    }

    public void PlayHitEffect(Vector3 pos, Vector3 normal, Transform parent = null,
        AttackType attackType = AttackType.Common)
    {
        var targetPrefab = commonHitEffectPrefab;

        if (attackType == AttackType.Fire)
            targetPrefab = fireHitEffectPrefab;

        var effect = Instantiate(targetPrefab, pos, Quaternion.LookRotation(normal));

        if (parent != null)
        {
            effect.transform.SetParent(parent);
        }

        effect.Play();
    }
}