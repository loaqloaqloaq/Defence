using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using static UnityEngine.EventSystems.EventTrigger;

public class Enemy2Controller : MonoBehaviour, IDamageable, EnemyInterface
{
    [HideInInspector]
    public float HP, MAXHP, ATK;
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

    float checkFeq, lastCheck;   
    

    //�U����H�������
    //int damage_Cnt = 0;

    bool loaded = false;

    int frameDelay = 5;
    int frameCnt = 0;
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

            MAXHP = EnemyJson.hp;

            ATK = EnemyJson.atk;

            drop.Add("ammo", EnemyJson.drop.ammo);
            drop.Add("health", EnemyJson.drop.health);

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
        Debug.Log("HIT");
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
        //�O�ɂ��Ȃ��ƃ_���[�W�󂯂Ȃ�
        var heading = target.position - transform.position;
        float dot = Vector3.Dot(heading, transform.forward);        
        if (dot < 0.2f || dot > 1.7f) return;
        //�U�������ƃA�j���V�����I���܂ł�����x�U�����Ȃ�
        if (attacked) return;
        //�����s���ƃ_���[�W�󂯂Ȃ�
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

    /*
    //�_���[�W����
    public void Damage(int damage)
    {
        if (dead) return;

        //HP������
        HP -= damage;
        //�_���[�W��H�������
        ++damage_Cnt;
        //HP��0�ȉ������̍U���Ŏ��񂾂Ƃ�
        if (HP <= 0 && damage_Cnt == 1)
        {
            //�����ɔj�󂳂���
            destoryTimer = 3.0f;
            animator.SetTrigger("die");
            dead = true;
        }
        //HP��0�ȉ������ȏ�̍U���Ŏ��񂾂Ƃ�
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
}