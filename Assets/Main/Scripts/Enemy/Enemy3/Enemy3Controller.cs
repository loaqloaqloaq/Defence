using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

using static UnityEngine.EventSystems.EventTrigger;

public class Enemy3Controller : MonoBehaviour, IDamageable, EnemyInterface
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
    EnemyData EnemyJson;
    RaycastWeapon rcw;
    //攻撃を食らった回数
    int damage_Cnt = 0;

    float fireFreq, fireCnt, fireStop, fireStopCnt;
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
        agent.enabled = true;

        EnemyGloable eg = GameObject.Find("EnemyLoader").GetComponent<EnemyGloable>();

        EnemyJson = eg.EnemyJson.Enemy3;
        agent.speed = EnemyJson.moveSpeed;

        MAXHP = EnemyJson.hp;
        HP = MAXHP;
        ATK = EnemyJson.atk;
        fireFreq = EnemyJson.AttackDuration;
        fireStop = EnemyJson.AttackStop;

        fireCnt = 0;
        fireStopCnt = 0;

        drop.Add("ammo", EnemyJson.drop.ammo);
        drop.Add("health", EnemyJson.drop.health);

        dropPrefab = eg.dropPrefab;
        explosion = eg.explosion;
        

        rcw = transform.Find("Hips/Spine/Spine1/Spine2/RightShoulder/RightArm/RightForeArm/RightHand/Weapon_Rifle 0").GetComponent<RaycastWeapon>();
        rcw.damage = ATK;

        transform.GetComponent<Collider>().enabled = true;


    }

    // Update is called once per frame
    void Update()
    {
        if (HP <= 0)
        {   
            transform.GetComponent<Collider>().enabled = false;
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
                //Debug.Log(Time.time.ToString() + " : " + Physics.Linecast(transform.position, target.transform.position));
            }
            var disToTarget = Vector3.Distance(transform.position, target.position);           
            if (!Physics.Linecast(transform.position, target.transform.position) && disToTarget < 20f)
            {
                attacking = true;
                //face to target
                var lookPos = target.position - transform.position;
                lookPos.y = 0;
                Quaternion rot= Quaternion.LookRotation(lookPos) * Quaternion.Euler(0, 50, 0);               
                transform.rotation = rot;

                animator.SetBool("attacking", true);                
                Attack();
            }
            else {
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
        animator.speed = 1;
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
        bool fire = true;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Shooting"))
        {
            fireCnt += Time.deltaTime;
            if (fireCnt>=fireFreq)
            {
                animator.Play("Shooting");
                animator.speed = 0f;
                fire = false;
                fireStopCnt += Time.deltaTime;
                if (fireStopCnt >= fireStop) {
                    fireStopCnt = 0;
                    fireCnt = 0;
                    animator.speed = 1f;
                }
            }
            
        }

        rcw.UpdateNPCWeapon(Time.deltaTime, fire);
    }

    private void ResetAfterAttack() { 
        attacking = false;
        attacked = false;
        animator.SetBool("attacking", false);
        animator.speed = 1;
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