using System;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.PlayerSettings;
using static UnityEngine.EventSystems.EventTrigger;

public class EnemyController : MonoBehaviour, IDamageable
{
    [HideInInspector]
    public float HP, MAXHP;
    private Animator animator;
    [HideInInspector]
    public NavMeshAgent agent;

    [HideInInspector]
    public bool attacking,attacked;

    private float destoryTimer, destoryTime;
    private bool dead;

    public Transform target;
    public Transform gate, player;
    public GameObject explosion;

    float checkFeq, lastCheck;

    
    // Start is called before the first frame update
    void Start()
    {
        MAXHP = 100f;
        HP = MAXHP;

        animator=GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        destoryTime = 3.0f;
        destoryTimer = 0;
        dead = false;

        gate = GameObject.Find("Gate").transform;
        player = GameObject.Find("Player").transform;        

        target = gate;

        lastCheck = 0;
        checkFeq = 0.5f;

        attacking = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (HP <= 0)
        {
            GetComponent<NavMeshAgent>().enabled = false;
            destoryTimer += Time.deltaTime;
            if (destoryTimer >= destoryTime)
            {
                var pos = transform.position;
                pos.y += 0.5f;
                if (explosion!=null) Instantiate(explosion, pos, transform.rotation);
                Destroy(gameObject);
            }
        }
        else {            
            int rand = UnityEngine.Random.Range(0, 100);
            var disToPlayer = Vector3.Distance(transform.position, player.position);
            var disToGate = Vector3.Distance(transform.position, gate.position);
            lastCheck += Time.deltaTime;
            if (lastCheck >= checkFeq)
            {
                lastCheck = 0;
                if (target == gate)
                {   if (disToGate > 2f)
                    {
                        if (disToPlayer < 3f) target = player;
                        else if (disToPlayer < 5f && rand < 5) target = player;
                        else if (disToPlayer < 7f && rand < 1) target = player;
                    }                    
                }
                else if (target == player)
                {
                    if (disToPlayer >= 7f && rand < 1) target = gate;
                    else if (disToPlayer >= 9f && rand < 5) target = gate;
                    else if (disToPlayer >= 11f) target = gate;
                }
            }
            var disToTarget = Vector3.Distance(transform.position, target.position);
            if (disToTarget < 1.5f && !animator.GetCurrentAnimatorStateInfo(0).IsName("attack"))
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
            animator.SetTrigger("die");
            dead = true;
        }
        return true;
    }
    private void Attack() {
        //前にいないとダメージ受けない
        var heading = target.position - transform.position;
        float dot = Vector3.Dot(heading, transform.forward);        
        if (dot < 0.2f || dot > 1.7f) return;
        //攻撃したとアニメション終わるまでもう一度攻撃しない
        if (attacked) return;
        //遠い行くとダメージ受けない
        if (Vector3.Distance(target.position,transform.position) >= 1.5f) return;

        DamageMessage dm= new DamageMessage();
        dm.damager = gameObject;
        dm.amount = 10f;
        if((target.GetComponent<GateController>() ?? null) != null) target.GetComponent<GateController>().ApplyDamage(dm);
        else if ((target.GetComponent<PlayerHealth>() ?? null) != null) target.GetComponent<PlayerHealth>().ApplyDamage(dm);   
        attacked = true;
    }

    private void ResetAfterAttack() { 
        attacking = false;
        attacked = false;
    }
}
