    using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

using static UnityEngine.EventSystems.EventTrigger;

public class Enemy1Controller : MonoBehaviour, IDamageable, EnemyInterface
{
    [HideInInspector]
    public float HP, MAXHP, ATK;
    Dictionary<string, float> drop = new Dictionary<string, float>();
    private Animator animator;
    [HideInInspector]
    public NavMeshAgent agent;

    [HideInInspector]
    public bool attacking,attacked;

    private float destoryTimer, destoryTime;
    private bool dead;

    public Transform target;
    public Transform gate, player;
    GateController gc;
    GameObject explosion;
    Dictionary<string, GameObject> dropPrefab = new Dictionary<string, GameObject>();    

    float checkFeq, lastCheck;

    [SerializeField]
    TextAsset EnemyJsonFile;
    EnemyData EnemyJson;

    //攻撃を食らった回数
    int damage_Cnt = 0;

    bool loaded = false;

    EnemyController ec;
    EnemyGloable eg;

    int frameDelay = 5;
    int frameCnt = 0;

    List<Collider> colliders= new List<Collider>();
    // Start is called before the first frame update
    void Start()
    {
        if (!loaded) { 
            animator = GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>();
            ec = transform.parent.GetComponent<EnemyController>();

            eg = GameObject.Find("EnemyLoader").GetComponent<EnemyGloable>();

            gate = eg.gate;
            player = eg.player;

            EnemyJson = eg.EnemyJson.Enemy1;
            agent.speed = EnemyJson.moveSpeed;

            MAXHP = EnemyJson.hp;
            ATK = EnemyJson.atk;

            if (!drop.ContainsKey("ammo")) drop.Add("ammo", EnemyJson.drop.ammo);
            if (!drop.ContainsKey("health")) drop.Add("health", EnemyJson.drop.health);

            dropPrefab = eg.dropPrefab;
            explosion = eg.explosion;

            colliders.Add(GetComponent<Collider>());
            colliders.Add(transform.Find("root/root.x/spine_01.x").GetComponent<Collider>());
            colliders.Add(transform.Find("root/root.x/spine_01.x/spine_02.x/shoulder.l/arm_stretch.l").GetComponent<Collider>());
            colliders.Add(transform.Find("root/root.x/spine_01.x/spine_02.x/shoulder.r/arm_stretch.r").GetComponent<Collider>());
            
            colliders.Concat(transform.Find("root/root.x/thigh_stretch.l").GetComponents<Collider>());
            colliders.Concat(transform.Find("root/root.x/thigh_stretch.r").GetComponents<Collider>());

            loaded = true;            
        }


        target = gate;
        destoryTime = 3.0f;
        destoryTimer = 0;
        dead = false;

        lastCheck = 0;
        checkFeq = 0.5f;

        attacking = false;
        agent.enabled = true;
        HP = MAXHP;

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
                ec.dead();
            }
        }
        else {
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
                else{
                    target = gate;
                }
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
        //Debug.Log("HIT");
        HP -= damageMessage.amount;        
        if (HP <= 0 && !dead)
        {            
            dead = true;
            Dead();
        }
        return true;
    }
    private void Dead() {
        Drop();
        animator.SetTrigger("die");
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
                pos.y = 0;
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
        if (target.name.StartsWith("Player") && Vector3.Distance(target.position,transform.position) >= 1.7f) return;

        DamageMessage dm= new DamageMessage();
        dm.damager = gameObject;
        dm.amount = ATK;
        if((target.GetComponent<GateController>() ?? null) != null) target.GetComponent<GateController>().ApplyDamage(dm);
        else if ((target.GetComponent<PlayerHealth>() ?? null) != null) target.GetComponent<PlayerHealth>().ApplyDamage(dm);   
        attacked = true;
    }

    private void ResetAfterAttack() { 
        attacking = false;
        attacked = false;
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
    private void setCollider(bool en) {
        foreach (Collider c in colliders) { 
            c.enabled = en;
        }
    }
}