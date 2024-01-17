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
    int reward;
    Dictionary<string, float> drop = new Dictionary<string, float>();
    private Animator animator;
    EnemyGloable eg;
    [HideInInspector]
    public NavMeshAgent agent;

    GameObject effect;

    [HideInInspector]
    public bool attacking, attacked;

    private float destoryTimer, destoryTime;
    private bool dead;

    public Transform target;

    Abnormality turretController;    

    GameObject explosion;
    Dictionary<string, GameObject> dropPrefab = new Dictionary<string, GameObject>();

    float checkFeq, lastCheck;
    
    EnemyData EnemyJson;

    Type resist, weakness;

    Collider[] colliders;
    
    //çUåÇÇêHÇÁÇ¡ÇΩâÒêî
    //int damage_Cnt = 0;

    float expRadius;   

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
            reward=EnemyJson.reward;


            drop.Add("ammo", EnemyJson.drop.ammo);
            drop.Add("health", EnemyJson.drop.health);

            dropPrefab = eg.dropPrefab;
            explosion = eg.explosion;

            effect = transform.Find("effect").gameObject;

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

        effect.SetActive(false);

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
                    pos.y = 0.5f;
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
                if (disToTarget < 1.5f)
                {
                    attacking = true;                    
                    if (turretController != null) turretController.AddAbnormality(AbnormalityType.STOP);
                    agent.enabled = false;
                }
                else if (attacking && disToTarget >= 2.5f)
                {
                    attacking = false;
                    if (turretController != null) turretController.RemoveAbnormality(AbnormalityType.STOP);
                    agent.enabled = true;
                }
                animator.SetBool("attack",attacking);
                effect.SetActive(attacking);
            }

        }
    }

    private GameObject getClosestTurret() {
        GameObject closest = eg.ts.Count > 0 ? eg.ts[0] : null ;
        foreach (var turret in eg.ts) {
           if(Vector3.Distance(transform.position, turret.transform.position)< Vector3.Distance(transform.position, closest.transform.position)) closest = turret;
        }
        turretController = closest?.GetComponent<Abnormality>() ?? null;       
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
            ResetAfterAttack();
            Dead();
        }
        return true;
    }
    private void Dead()
    {
        //Debug.Log("dead");
        ResetAfterAttack();
        animator.speed = 1;
        animator.SetTrigger("die");
        Drop();
        GameManager.Instance.AddScrap(reward);
        if (turretController != null) turretController.RemoveAbnormality(AbnormalityType.STOP);
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
                pos.y = 200f;
                RaycastHit hitInfo;
                if (Physics.Raycast(pos, Vector3.down, out hitInfo, LayerMask.GetMask("FLoor")))
                {
                    pos = hitInfo.point;
                }
                else
                {
                    pos.y = transform.position.y;
                }
                Instantiate(dropPrefab[d.Key], pos, transform.rotation);
            }

            prevPresent += d.Value;
        }
    }   

#if UNITY_EDITOR 
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
        animator.SetBool("attack", false);
        effect.SetActive(false);
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
    public Part GetPart()
    {
        return Part.BODY;
    }
}