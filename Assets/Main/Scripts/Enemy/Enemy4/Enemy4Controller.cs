using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

using static UnityEngine.EventSystems.EventTrigger;

public class Enemy4Controller : MonoBehaviour, IEnemyDamageable, EnemyInterface
{
    [HideInInspector]
    public float HP, MAXHP, ATK;
    public int reward;
    Dictionary<string, float> drop = new Dictionary<string, float>();
    private Animator animator;
    EnemyGloable eg;
    [HideInInspector]
    public NavMeshAgent agent;

    [HideInInspector]
    public bool attacking, attacked;

    private float destoryTimer, destoryTime;
    private bool dead;

    public Transform target;
    public Transform gate, player;
    [SerializeField]GameObject explosion;
    Dictionary<string, GameObject> dropPrefab = new Dictionary<string, GameObject>();

    float checkFeq, lastCheck;
    
    EnemyData EnemyJson;

    Type resist, weakness;

    Collider[] colliders;

    //ParticleSystem ps;
    GameObject expEffect;

    //攻撃を食らった回数
    //int damage_Cnt = 0;

    float expRadius;
    float expTimer;

    bool loaded = false;

    int frameDelay = 5;
    int frameCnt = 0;

    private AudioSource audioSource;

    [SerializeField] private AudioData hitSE;
    [SerializeField] private AudioData deadSE;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        SoundManager.Instance?.AddAudioInfo(hitSE);
        SoundManager.Instance?.AddAudioInfo(deadSE);
    }

    void Start()
    {
        if (!loaded)
        {   
            expEffect = transform.GetChild(1).gameObject;

            colliders = transform.GetComponentsInChildren<Collider>();

            animator = GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>();

            eg = GameObject.Find("EnemyLoader").GetComponent<EnemyGloable>();

            gate = eg.gate;
            player = eg.player;

            EnemyJson = eg.EnemyJson.Enemy4;
            agent.speed = EnemyJson.moveSpeed;

            resist = EnemyJson.resist;
            weakness = EnemyJson.weakness;

            expRadius = EnemyJson.AttackRadius;
            expTimer = EnemyJson.AttackDuration;

            MAXHP = EnemyJson.hp;
            ATK = EnemyJson.atk;
            reward = EnemyJson.reward;

            drop.Add("ammo", EnemyJson.drop.ammo);
            drop.Add("health", EnemyJson.drop.health);

            dropPrefab = eg.dropPrefab;
            explosion = eg.explosion;

            loaded = true;
        }

        destoryTime = 3.0f;
        destoryTimer = 0;
        dead = false;


        foreach (Collider c in colliders)
        {
            c.enabled = true;
        }

        gate = eg.gate;
        target = gate;

        lastCheck = 0;
        checkFeq = 0.5f;
        expEffect.SetActive(false);
        agent.enabled = true;
        attacking = false;

        HP = MAXHP;

        frameCnt = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (HP <= 0)
        {  
            animator.speed = 1;
            agent.enabled = false;
            destoryTimer += Time.deltaTime;
            if (destoryTimer >= destoryTime)
            {
                Attack();
                transform.parent.GetComponent<EnemyController>().dead();
            }
        }
        else
        {
            gate = eg.gate;
            lastCheck += Time.deltaTime;
            if (lastCheck >= checkFeq)
            {
                int rand = UnityEngine.Random.Range(0, 100);
                var disToPlayer = Vector3.Distance(transform.position, player.position);
                var disToGate = Vector3.Distance(transform.position, gate.position);
                lastCheck = 0;
                if (disToGate <= 2f) target = gate;
                else if (target == gate)
                {
                    if (disToPlayer < 3f) target = player;
                    else if (disToPlayer < 5f && rand < 5) target = player;
                    else if (disToPlayer < 7f && rand < 1) target = player;
                }
                else if (target == player)
                {
                    if (disToPlayer >= 7f && rand < 1) target = gate;
                    else if (disToPlayer >= 9f && rand < 5) target = gate;
                    else if (disToPlayer >= 11f) target = gate;
                }
                else
                {
                    target = gate;
                }
            }
            frameCnt++;
            if (frameCnt >= frameDelay)
            {
                frameCnt = 0;
                var disToTarget = Vector3.Distance(transform.position, target.position);
                if (disToTarget < 2.5f && !attacking)
                {
                    attacking = true;
                    //face to target
                    var lookPos = target.position - transform.position;
                    lookPos.y = 0;
                    transform.rotation = Quaternion.LookRotation(lookPos);
                    animator.SetTrigger("attack");
                    //if (!ps.isPlaying) ps.Play();                
                }
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                {
                    animator.speed = 1f / expTimer;
                    //Debug.Log(animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
                    expEffect.SetActive(true);
                    if (disToTarget >= 2.5f && animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.4f)
                    {
                        ResetAfterAttack();
                        animator.Play("idle");
                    }
                    else if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
                    {
                        Attack();
                    }
                }
                else if (animator.GetCurrentAnimatorStateInfo(0).IsName("idle"))
                {
                    ResetAfterAttack();
                }
            }

        }
    }
    public bool ApplyDamage(DamageMessage damageMessage)
    {
        return ApplyDamage(damageMessage, Part.BODY);
    }      
    public bool ApplyDamage(DamageMessage damageMessage,Part part)
    {       
        float damageMuiltplier = 1f;
        switch (damageMessage.attackType)
        {
            case AttackType.Common:
                if (resist.common) damageMuiltplier = resist.persent;
                if (weakness.common) damageMuiltplier = weakness.persent;
                break;
            case AttackType.Fire:
                if (resist.fire) damageMuiltplier = resist.persent;
                if (weakness.fire) damageMuiltplier = weakness.persent;
                break;
            case AttackType.Explosion:
                if (resist.explode) damageMuiltplier = resist.persent;
                if (weakness.explode) damageMuiltplier = weakness.persent;
                break;
            default:
                break;
        }
        HP -= damageMessage.amount * damageMuiltplier;
        //SE & VFX
        EffectManager.Instance?.PlayHitEffect(damageMessage.hitPoint, damageMessage.hitNormal,
            transform, damageMessage.attackType);
        if (damageMessage.attackType == AttackType.Common)
        {
            SoundManager.Instance?.PlaySE(hitSE.name, audioSource);
        }

        if (HP <= 0 && !dead)
        {
            dead = true;
            SoundManager.Instance?.PlaySE(deadSE.name, audioSource);
            Dead();
        }
        return true;
    }
    private void Dead(bool drop = true)
    {
        foreach (Collider c in colliders)
        {
            c.enabled = false;
        }
        if(drop) Drop();
        animator.SetTrigger("die");
        GameManager.Instance.AddScrap(reward);
        //GetComponent<Rigidbody>().isKinematic = true;
    }
    private void Drop()
    {
        float dice = UnityEngine.Random.Range(0f, 100f);
        float prevPresent = 0;
        foreach (KeyValuePair<string, float> d in drop)
        {
            if (dice > prevPresent && dice <= prevPresent + d.Value && dropPrefab[d.Key] != null)
            {
                var pos = transform.position;
                pos.x += UnityEngine.Random.Range(-0.5f, 0.5f);
                pos.z += UnityEngine.Random.Range(-0.5f, 0.5f);
                pos.y = 0;
                Instantiate(dropPrefab[d.Key], pos, transform.rotation);
            }

            prevPresent += d.Value;
        }
    }
    private void Attack()
    {
        if (explosion != null)
        {            
            var pos = transform.position;
            pos.y += 0.5f;
            var exp = Instantiate(explosion, pos, transform.rotation);
            float scale = 0.75f * expRadius;
            exp.transform.localScale = new Vector3(scale, scale, scale);
            transform.parent.GetComponent<EnemyController>().dead(); 

            DamageMessage dm = new DamageMessage();
            dm.damager = gameObject;
            
            Collider[] hitColliders = Physics.OverlapSphere(pos, expRadius);
            foreach (var hitCollider in hitColliders)
            {
                var hitTarget = hitCollider.gameObject.GetComponent<IDamageable>();
                float targetToExp = Vector3.Distance(hitCollider.transform.position, exp.transform.position);
                dm.amount = ATK * (1 - targetToExp / expRadius);

                if (hitTarget != null && dm.amount > 0) {
                    hitTarget.ApplyDamage(dm);
                }
            }

            attacked = true;
        }       
    }

#if UNITY_EDITOR //攻撃範囲デバッグ
    private void OnDrawGizmosSelected()
    {
        var pos = transform.position;        
        Gizmos.color = new Color(1f, 0f, 0f, 0.15f);
        Gizmos.DrawSphere(pos, expRadius);      
     }
#endif

    private void ResetAfterAttack()
    {
        attacking = false;
        attacked = false;
        expEffect.SetActive(false);
        //GetComponent<ParticleSystem>().Stop();
    }
    /*
    //ダメージ処理
    public void Damage(int damage)
    {
        if (dead) return;

        //HPを減少
        HP -= damage;
        //ダメージを食らった回数
        ++damage_Cnt;
        //HPが0以下かつ一回の攻撃で死んだとき
        if (HP <= 0 && damage_Cnt == 1)
        {
            //すぐに破壊させる
            destoryTimer = 3.0f;
            animator.SetTrigger("die");
            dead = true;
        }
        //HPが0以下かつ二回以上の攻撃で死んだとき
        else if (HP <= 0 && damage_Cnt >= 2)
        {
            animator.SetTrigger("die");
            dead = true;
        }
    }
    */
    public bool IsDead()
    {
        return dead;
    }
    public void resetEnemy()
    {
        Start();
    }
    public Part GetPart()
    {
        return Part.BODY;
    }
}