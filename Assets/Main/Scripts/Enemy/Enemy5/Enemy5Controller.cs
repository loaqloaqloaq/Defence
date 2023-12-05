using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

using static UnityEngine.EventSystems.EventTrigger;

public class Enemy5Controller : MonoBehaviour, IDamageable, EnemyInterface
{
    [HideInInspector]
    public float HP, MAXHP, ATK;
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
    GameObject explosion;
    Dictionary<string, GameObject> dropPrefab = new Dictionary<string, GameObject>();

    float checkFeq, lastCheck;

    [SerializeField]
    TextAsset EnemyJsonFile;
    EnemyData EnemyJson;

    Type resist, weakness;

    Collider[] colliders;
    
    //攻撃を食らった回数
    //int damage_Cnt = 0;

    float expRadius;   

    bool loaded = false;

    int frameDelay = 5;
    int frameCnt = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (!loaded)
        {        
            colliders = transform.GetComponents<Collider>();

            animator = GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>();

            eg = GameObject.Find("EnemyLoader").GetComponent<EnemyGloable>();           
            

            EnemyJson = eg.EnemyJson.Enemy5;
            agent.speed = EnemyJson.moveSpeed;

            resist = EnemyJson.resist;
            weakness = EnemyJson.weakness;

            expRadius = EnemyJson.AttackRadius;           

            MAXHP = EnemyJson.hp;
            ATK = EnemyJson.atk;

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
        
        lastCheck = 0;
        checkFeq = 0.5f;
        
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
            lastCheck += Time.deltaTime;
            if (lastCheck >= checkFeq)
            {
               
            }
            frameCnt++;
            if (frameCnt >= frameDelay)
            {
                target = getClosestTurret()?.transform ?? null;
                if (target == null) return;
                frameCnt = 0;
                var disToTarget = Vector3.Distance(transform.position, target.position);
                if (disToTarget < 2.5f)
                {
                    Gizmos.color = new Color(0.5f,0.5f,1,0.7f);
                    Gizmos.DrawSphere(transform.position, 2.5f);
                    agent.enabled = false;
                }
                else {
                    agent.enabled = true;
                }
            }

        }
    }

    private GameObject getClosestTurret() {
        GameObject closest = eg.ts.Length > 0 ? eg.ts[0] : null ;
        foreach (var turret in eg.ts) {
           if(Vector3.Distance(transform.position, turret.transform.position)< Vector3.Distance(transform.position, closest.transform.position)) closest = turret;
        }
        return closest;
    }
    public bool ApplyDamage(DamageMessage damageMessage)
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
        //GetComponent<ParticleSystem>().Stop();
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