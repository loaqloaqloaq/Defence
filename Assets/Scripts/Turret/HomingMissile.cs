using System.Collections;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{

    [SerializeField] private Transform target;
    [SerializeField, Min(0)] private float startTime = 1;
    private float time;
    [SerializeField] private float lifeTime = 2;
    [SerializeField] private bool limitAcceleration = false;
    [SerializeField, Min(0)] private float maxAcceleration = 100;
    [SerializeField] private Vector3 minInitVelocity;
    [SerializeField] private Vector3 maxInitVelocity;

    [SerializeField] private LayerMask targetLayer;

    [SerializeField] private float damage;
    [SerializeField] private float colliderRaidus;
    [SerializeField] private float attackRadius;

    [SerializeField] ParticleSystem smokeTrail;

    private Vector3 position;
    private Vector3 velocity;
    private Vector3 acceleration;

    [SerializeField] private GameObject model;

    private Transform m_transform;
    private AudioSource m_audioSource;

    [SerializeField] private AudioData fireSE;

    public bool isSimulating { get; private set; }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
        Gizmos.DrawSphere(transform.position, colliderRaidus); //オブジェクトの攻撃半径を球体に描画

        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        Gizmos.DrawSphere(transform.position, attackRadius); //オブジェクトの攻撃半径を球体に描画
    }

    public Transform Target
    {
        set
        {
            target = value;
        }
        get
        {
            return target;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
        Gizmos.DrawSphere(transform.position, attackRadius);
    }

    public void Initialize(float damage)
    {
        this.damage = damage;
        m_transform = transform;
        m_audioSource = GetComponent<AudioSource>();
        SoundManager.Instance?.AddAudioInfo(fireSE);
    }


    public void Activate(Vector3 pos, Transform target)
    {
        time = startTime;
        model.SetActive(true);
        isSimulating = true;
        position = pos;
        this.target = target;
        velocity = new Vector3(Random.Range(minInitVelocity.x, maxInitVelocity.x), Random.Range(minInitVelocity.y, maxInitVelocity.y), Random.Range(minInitVelocity.z, maxInitVelocity.z));
        StartCoroutine(Timer());
    }

    private void Deactivate()
    {
        ;
        model.SetActive(false);
        isSimulating = false;
        smokeTrail.Stop();
    }

    public void Update()
    {
        if (!isSimulating) return;

        if (target == null) Deactivate(); 

        //float yModifier = target.GetComponent<Collider>().bounds.size.y * 0.5f;

        //Vector3 targetPosition = target.position + new Vector3(0f, yModifier, 0f);
        Vector3 targetPosition = target.position;
        //ミサイルの動き
        acceleration = 2f / (time * time) * (targetPosition - position - time * velocity);

        if (limitAcceleration && acceleration.sqrMagnitude > maxAcceleration * maxAcceleration)
        {
            acceleration = acceleration.normalized * maxAcceleration;
        }

        time -= Time.deltaTime;

        if (startTime < 0f)
        {
            return;
        }

        velocity += acceleration * Time.deltaTime;
        position += velocity * Time.deltaTime;
        m_transform.position = position;
        m_transform.rotation = Quaternion.LookRotation(velocity);

        if (smokeTrail.isStopped) smokeTrail.Play();

        CheckCollider();
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(lifeTime);

        Deactivate();
    }

    private void CheckCollider()
    {
        var colliders = Physics.OverlapSphere(transform.position, colliderRaidus, targetLayer);
        foreach (var collider in colliders)
        {
            if (!collider) continue;
       
            DamageProcess();
            EffectManager.Instance?.PlayHitEffect(transform.position, Vector3.up, null, EffectManager.EffectType.Explosion);
            SoundManager.Instance?.PlaySE(fireSE.name, m_audioSource);
            Deactivate();
            break;
        }
    }

    private void DamageProcess()
    {
        var colliders = Physics.OverlapSphere(transform.position, attackRadius, targetLayer);
        foreach (var collider in colliders)
        {
            var target = collider.GetComponent<IDamageable>();

            if (target != null)
            {
                Vector3 normal = collider.transform.position - transform.position;

                DamageMessage damageMessage;
                damageMessage.damager = null;
                damageMessage.amount = damage;
                damageMessage.hitPoint = collider.transform.position;
                damageMessage.hitNormal = normal;
                damageMessage.attackType = AttackType.Explosion;

                target.ApplyDamage(damageMessage);
            }
        }
    }


}