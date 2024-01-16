using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using static UnityEngine.EventSystems.EventTrigger;

public class Enemy2Controller : MonoBehaviour, IEnemyDamageable, EnemyInterface
{
    [HideInInspector]
    public float HP, MAXHP, ATK;
    int reward;
    Dictionary<string, float> drop = new Dictionary<string, float>();
    private Animator animator;
    EnemyGloable eg;
    [HideInInspector]
    public NavMeshAgent agent;

    [HideInInspector]
    public bool attacking,attacked;

    private float destoryTimer, destoryTime;
    private bool dead;

    public Transform target;
    public Transform player;
    GameObject explosion;

    Dictionary<string, GameObject> dropPrefab = new Dictionary<string, GameObject>();
    EnemyData EnemyJson;
    Type resist, weakness;

    Dictionary<Part, bool> takingDamage = new Dictionary<Part, bool>();

    float checkFeq, lastCheck;   
    

    //攻撃を食らった回数
    //int damage_Cnt = 0;

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

    // Start is called before the first frame update
    void Start()
    {
        if (!loaded)
        {
            

            animator = GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>();

            eg = GameObject.Find("EnemyLoader").GetComponent<EnemyGloable>();

            player = eg.player;

            EnemyJson = eg.EnemyJson.Enemy2;
            agent.speed = EnemyJson.moveSpeed;

            resist = EnemyJson.resist;
            weakness = EnemyJson.weakness;

            MAXHP = EnemyJson.hp;

            ATK = EnemyJson.atk;
            reward=EnemyJson.reward;

            drop.Add("ammo", EnemyJson.drop.ammo);
            drop.Add("health", EnemyJson.drop.health);

            for (int i = 0; i < Enum.GetValues(typeof(Part)).Length; i++)
            {
                if (!takingDamage.ContainsKey((Part)i)) takingDamage.Add((Part)i, false);
            }

            dropPrefab = eg.dropPrefab;
            explosion = eg.explosion;

            loaded = true;
        }

        HP = MAXHP;
        agent.enabled = true;
        target = player;

        lastCheck = 0;
        checkFeq = 0.5f;
        destoryTime = 3.0f;
        destoryTimer = 0;
        dead = false;

        attacking = false;
        setCollider(true);

        frameCnt = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (HP <= 0)
        {
            setCollider(false);
           
            agent.enabled = false;
            destoryTimer += Time.deltaTime;
            if (destoryTimer >= destoryTime)
            {                
                if (explosion != null) {
                    var pos = transform.position;
                    pos.y += 0.5f;
                    var exp=Instantiate(explosion, pos, transform.rotation);
                    exp.transform.localScale=new Vector3(0.7f, 0.7f, 0.7f);
                }
                transform.parent.GetComponent<EnemyController>().dead();
            }
        }
        else {  
            lastCheck += Time.deltaTime;
            if (lastCheck >= checkFeq)
            {
                lastCheck = 0;
            }
            frameCnt++;
            if (frameCnt >= frameDelay)
            {
                frameCnt = 0;
                var disToTarget = Vector3.Distance(transform.position, target.position);
                if (disToTarget < 1.5f && animator.GetCurrentAnimatorStateInfo(0).IsName("idle"))
                {
                    attacking = true;
                    //face to target
                    var lookPos = target.position - transform.position;
                    lookPos.y = 0;
                    transform.rotation = Quaternion.LookRotation(lookPos);
                    animator.SetTrigger("attack");
                }
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("attack"))
                {
                    agent.isStopped = true;
                    if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f)
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
        Debug.Log(damageMessage.damager);
        return ApplyDamage(damageMessage, Part.BODY);
    }
    private KeyValuePair<Part, bool>? GetTakingDamagePart()
    {
        foreach (var p in takingDamage)
        {
            if (p.Value) return p;
        }
        return null;
    }
    public bool ApplyDamage(DamageMessage damageMessage, Part part)
    {
        //Debug.Log("HIT");
        KeyValuePair<Part, bool>? takingDamagePart = GetTakingDamagePart();
        if (takingDamagePart != null && takingDamagePart.Value.Key != part) return true;
        float damageMuiltplier = 1f;
        takingDamage[part] = true;
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
        takingDamage[part] = false;
        return true;
    }
    private void Dead() {
        Drop();
        animator.SetTrigger("die");
        GameManager.Instance.AddScrap(reward);
        //GetComponent<Rigidbody>().isKinematic = true;
    }
    private void Drop() {
        float dice = UnityEngine.Random.Range(0f, 100f);
        float prevPresent = 0;
        foreach (KeyValuePair<string, float> d in drop) {            
            if (dice > prevPresent && dice <= prevPresent+d.Value && dropPrefab[d.Key] != null) {
                var pos = transform.position;
                pos.x += UnityEngine.Random.Range(-0.5f, 0.5f);
                pos.z += UnityEngine.Random.Range(-0.5f, 0.5f);
                pos.y = 200f;
                RaycastHit hitInfo;
                if (Physics.Raycast(pos, Vector3.down, out hitInfo))
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
    private void Attack() {
        //前にいないとダメージ受けない
        var heading = target.position - transform.position;
        float dot = Vector3.Dot(heading, transform.forward);        
        if (dot < 0.2f || dot > 1.7f) return;
        //攻撃したとアニメション終わるまでもう一度攻撃しない
        if (attacked) return;
        //遠い行くとダメージ受けない
        if (Vector3.Distance(target.position,transform.position) >= 1.7f) return;

        DamageMessage dm= new DamageMessage();
        dm.damager = gameObject;
        dm.amount = ATK;
        dm.hitNormal = transform.position - target.transform.position; 
        if ((target.GetComponent<GateController>() ?? null) != null) target.GetComponent<GateController>().ApplyDamage(dm);
        else if ((target.GetComponent<PlayerHealth>() ?? null) != null) target.GetComponent<PlayerHealth>().ApplyDamage(dm);   
        attacked = true;
    }

    private void ResetAfterAttack() { 
        attacking = false;
        attacked = false;
        agent.isStopped = false;
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
    private void setCollider(GameObject gb,bool enable) {
        foreach (Transform child in transform) {
            child.GetComponent<Collider>().enabled = enable;
            setCollider(child.gameObject, enable);
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
    private void setCollider(bool en) {
        transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(2).GetChild(0).GetComponent<BoxCollider>().enabled = en;
        transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(3).GetChild(0).GetComponent<BoxCollider>().enabled = en;
        transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<BoxCollider>().enabled = en;
        transform.GetChild(1).GetChild(0).GetChild(2).GetComponent<BoxCollider>().enabled = en;
        transform.GetComponent<BoxCollider>().enabled = en;
    }
    public Part GetPart()
    {
        return Part.BODY;
    }
}