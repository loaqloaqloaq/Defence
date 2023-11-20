using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

using static UnityEngine.EventSystems.EventTrigger;

public class Enemy4Controller : MonoBehaviour, IDamageable, EnemyInterface
{
    [HideInInspector]
    public float HP, MAXHP, ATK;
    Dictionary<string, float> drop = new Dictionary<string, float>();
    private Animator animator;
    [HideInInspector]
    public NavMeshAgent agent;

    [HideInInspector]
    public bool attacking, attacked;

    private float destoryTimer, destoryTime;
    private bool dead;

    public Transform target;
    public Transform gate, gate1, gate2, gate3, player;
    GameObject explosion;
    Dictionary<string, GameObject> dropPrefab = new Dictionary<string, GameObject>();

    float checkFeq, lastCheck;

    [SerializeField]
    TextAsset EnemyJsonFile;
    EnemyData EnemyJson;

    Collider[] colliders;

    //ParticleSystem ps;
    GameObject expEffect;

    //攻撃を食らった回数
    int damage_Cnt = 0;

    float expRadius;
    float expTimer;
    // Start is called before the first frame update
    void Start()
    {

        destoryTime = 3.0f;
        destoryTimer = 0;
        dead = false;

        gate1 = GameObject.Find("Gate1").transform;
        gate2 = GameObject.Find("Gate2").transform;
        gate3 = GameObject.Find("Gate3").transform;
        gate = gate1 != null ? gate1 : gate2 != null ? gate2 : gate3;
        player = GameObject.Find("Player").transform;

        expEffect = transform.GetChild(1).gameObject;
        expEffect.SetActive(false);

        colliders = transform.GetComponents<Collider>();

        target = gate;

        lastCheck = 0;
        checkFeq = 0.5f;

        attacking = false;

        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = true;        

        EnemyGloable eg = GameObject.Find("EnemyLoader").GetComponent<EnemyGloable>();

        EnemyJson = eg.EnemyJson.Enemy4;
        agent.speed = EnemyJson.moveSpeed;
        expRadius = EnemyJson.AttackRadius;
        expTimer = EnemyJson.AttackDuration;

       MAXHP = EnemyJson.hp;
        HP = MAXHP;
        ATK = EnemyJson.atk;        

        drop.Add("ammo", EnemyJson.drop.ammo);
        drop.Add("health", EnemyJson.drop.health);

        dropPrefab = eg.dropPrefab;
        explosion = eg.explosion;

       


    }

    // Update is called once per frame
    void Update()
    {
        if (HP <= 0)
        {
            foreach (Collider c in colliders) { 
                c.enabled = false;
            }
            agent.enabled = false;
            destoryTimer += Time.deltaTime;
            if (destoryTimer >= destoryTime)
            {
                if (explosion != null)
                {
                    var pos = transform.position;
                    pos.y += 0.5f - 1;
                    var exp = Instantiate(explosion, pos, transform.rotation);
                    exp.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
                }
                transform.parent.GetComponent<EnemyController>().dead();
            }
        }
        else
        {
            if (gate1 != null && !gate1.GetComponent<GateController>().broke) { gate = gate1; }
            else if (gate2 != null && !gate2.GetComponent<GateController>().broke) { gate = gate2; }
            else if (gate3 != null && !gate3.GetComponent<GateController>().broke) { gate = gate3; }

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
                if (disToTarget >= 2.5f && animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.4f) {
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
    public bool ApplyDamage(DamageMessage damageMessage)
    {
        HP -= damageMessage.amount;
        if (HP <= 0 && !dead)
        {
            dead = true;
            ResetAfterAttack();
            Dead();
        }
        return true;
    }
    private void Dead()
    {
        Drop();
        animator.SetTrigger("die");
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
            pos.y = 1f;
            var exp = Instantiate(explosion, pos, transform.rotation);
            float scale = 0.75f * expRadius;
            exp.transform.localScale = new Vector3(scale, scale, scale);
            transform.parent.GetComponent<EnemyController>().dead(); 

            DamageMessage dm = new DamageMessage();
            dm.damager = gameObject;

            pos.y = 0;
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

#if UNITY_EDITOR //Turretの攻撃範囲デバッグ
    private void OnDrawGizmosSelected()
    {
        var pos = transform.position;
        pos.y = 0;
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
    public bool IsDead()
    {
        return dead;
    }
    public void resetEnemy()
    {
        Start();
    }
}