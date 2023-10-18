using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    //爆発VFX
    [SerializeField] private ParticleSystem explosionVFX;
    
    //コンポネント
    private BoxCollider boxCollider;
    private Rigidbody rb;

    [SerializeField] private GameObject mesh;

    [SerializeField] private float explosionRange = 10.0f;
    [SerializeField] private float damageAmount = 200.0f;

    [SerializeField] private LayerMask whatIsTarget;

    //コンポネント取得
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
    }

    public void StartExplosion(float delay)
    {
        SetActive(true);
        StartCoroutine(Explosion(delay));
    }

#if UNITY_EDITOR 
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        Gizmos.DrawSphere(transform.position, explosionRange);
    }
#endif
    //物理処理オフ＆シーンから見えないようにする
    private void SetActive(bool isActive)
    {
        rb.useGravity = isActive;
        mesh.SetActive(isActive);
        boxCollider.enabled = isActive;
    }

    IEnumerator Explosion(float delay)
    {
        //待機
        yield return new WaitForSeconds(delay);

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        //VFX・SFX再生
        explosionVFX.Play();
        SoundManager.Instance.Play("Sounds/Sfx/explosion1");

        SetActive(false);

        var colliders = Physics.OverlapSphere(transform.position, 10.0f, whatIsTarget);
        //範囲内のTargetLayer Colliderにダメージ
        foreach (var collider in colliders)
        {
            var target = collider.GetComponent<IDamageable>();

            if (target != null)
            {
                Vector3 normal = collider.transform.position - transform.position;

                DamageMessage damageMessage;
                damageMessage.damager = null;
                damageMessage.amount = damageAmount;
                damageMessage.hitPoint = collider.transform.position;
                damageMessage.hitNormal = normal;

                target.ApplyDamage(damageMessage);
            }
        }
    }
}
