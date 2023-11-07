using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using static UnityEngine.EventSystems.EventTrigger;

public class Enemy2Controller : MonoBehaviour, IDamageable
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
    public Transform player;
    public GameObject explosion;

    Dictionary<string, GameObject> dropPrefab = new Dictionary<string, GameObject>();
    public GameObject ammoPack,healthPack;

    float checkFeq, lastCheck;

    [SerializeField]
    TextAsset EnemyJsonFile;
    EnemyJsonReader EnemyJson;

    //攻撃を食らった回数
    int damage_Cnt = 0;
    // Start is called before the first frame update
    void Start()
    {              

        destoryTime = 3.0f;
        destoryTimer = 0;
        dead = false;
        
        player = GameObject.Find("Player").transform;        

        target = player;

        lastCheck = 0;
        checkFeq = 0.5f;

        attacking = false;

        animator = GetComponent<Animator>();       
        agent = GetComponent<NavMeshAgent>();

        string jsonString = EnemyJsonFile.ToString();
        EnemyJson= JsonUtility.FromJson<EnemyJsonReader>(jsonString);
        agent.speed = EnemyJson.Enemy2.moveSpeed;

        MAXHP = EnemyJson.Enemy2.hp;
        HP = MAXHP;
        ATK = EnemyJson.Enemy2.atk;
               
        drop.Add("ammo", EnemyJson.Enemy2.drop.ammo);
        drop.Add("health", EnemyJson.Enemy2.drop.health);        
        
        dropPrefab.Add("ammo", ammoPack);
        dropPrefab.Add("health", healthPack);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (HP <= 0)
        {            
            transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(2).GetChild(0).GetComponent<BoxCollider>().enabled = false;
            transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(3).GetChild(0).GetComponent<BoxCollider>().enabled = false;
            transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<BoxCollider>().enabled = false;
            transform.GetChild(1).GetChild(0).GetChild(2).GetComponent<BoxCollider>().enabled = false;
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
                Destroy(gameObject);
            }
        }
        else {  
            lastCheck += Time.deltaTime;
            if (lastCheck >= checkFeq)
            {
                
            }
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
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("attack")) {
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f) {
                    Attack();
                }
            }
            else if(animator.GetCurrentAnimatorStateInfo(0).IsName("idle")) {
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
            Dead();
        }
        return true;
    }
    private void Dead() {
        Drop();
        animator.SetTrigger("die");
        GetComponent<Rigidbody>().isKinematic = true;
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
        if (Vector3.Distance(target.position,transform.position) >= 1.7f) return;

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
}