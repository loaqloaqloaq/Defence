using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyGuardNavigator : MonoBehaviour
{
    public Transform target;    
    private Animator animator;
    private EnemyGuardController ec;
    private Transform player;

    [SerializeField] bool debug;
    


    // Start is called before the first frame update

    void Start()
    {
        animator = GetComponent<Animator>();
        ec = GetComponent<EnemyGuardController>();
        player = GameObject.Find("Player").transform;        
    }

    // Update is called once per frame
    void Update()
    {
        if (ec?.agent?.enabled == true)
        {
            target = ec.target ?? player;
            switch (ec.state)
            {
                case EnemyGuardController.State.STOP:
                    if (ec?.agent?.enabled == true && ec?.agent?.isStopped == false) ec.agent.isStopped = true;
                    animator.SetBool("walking", false);
                    if (Vector3.Distance(ec.enemyBase.transform.position, target.position) < ec.guardRange && Vector3.Distance(transform.position, target.position) > 1.5f)
                    {
                        ec.state= EnemyGuardController.State.FOLLOWING;
                    }                    
                    break;
                case EnemyGuardController.State.FOLLOWING:
                    ec.agent.isStopped = false;
                    animator.SetBool("walking", true);
                    if (Vector3.Distance(ec.enemyBase.transform.position, target.position) > (ec.guardRange + 10))
                    {
                        ec.agent.destination = ec.originalWorldPos;
                    }
                    else {                      
                        ec.agent.destination = target.position;
                    }                    
                    if (Vector3.Distance(transform.position, ec.agent.destination) <= 0.1f)
                    {                        
                        ec.state = EnemyGuardController.State.STOP;
                    }                     
                    break;
                case EnemyGuardController.State.ATTACKING:
                    animator.SetBool("walking", false);
                    if (ec?.agent?.enabled == true && ec?.agent?.isStopped == false) ec.agent.isStopped = true;
                    break;
            }
        }
        /*
        if (ec?.agent?.enabled == true && !ec.attacking)
        {
            target = ec.target ?? player;
            if (Vector3.Distance(ec.enemyBase.transform.position, target.position) < ec.guardRange && Vector3.Distance(transform.position, target.position) > 1.5f && !ec.attacking)
            {
                ec.agent.isStopped = false;
                animator.SetBool("walking", true);
                var targetPos = target.position;
                ec.agent.destination = targetPos;
            }
            else if (Vector3.Distance(ec.enemyBase.transform.position, target.position) > ec.guardRange && Vector3.Distance(transform.position, ec.enemyBase.position )>0.2f) {
                ec.agent.destination = ec.originalPos;
            }
            else
            {
                if(ec?.agent?.enabled==true && ec?.agent?.isStopped == false) ec.agent.isStopped = true;
                animator.SetBool("walking", false);
            }
        }
        else {            
            animator.SetBool("walking", false);            
        }    
        */
    }

   
}
