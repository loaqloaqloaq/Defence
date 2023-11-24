using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy1Navigator : MonoBehaviour
{
    public Transform target;    
    private Animator animator;
    private Enemy1Controller ec;
    private Transform g1, g2, g3;

    // Start is called before the first frame update

    void Start()
    {
        animator = GetComponent<Animator>();
        ec = GetComponent<Enemy1Controller>();        
        g1 = GameObject.Find("Gate1").transform;
        g2 = GameObject.Find("Gate2").transform;
        g3 = GameObject.Find("Gate3").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (ec?.agent?.enabled == true && !ec.attacking)
        {
            target = ec.target ?? g1 ?? g2 ?? g3;
            if (Vector3.Distance(transform.position, target.position) > 1.5f && !ec.attacking)
            {
                animator.SetBool("walking", true);
                ec.agent.isStopped = false;
                var targetPos = target.position;
                if (target.name.StartsWith("Gate"))
                    targetPos.x = (target.position - (target.position - transform.position).normalized * 1.5f).x;
                ec.agent.destination = targetPos;

            }
            else
            {
                animator.SetBool("walking", false);
                if (ec.agent.isStopped == false) ec.agent.isStopped = true;
            }
        }
        else {
            animator.SetBool("walking", false);
            if(ec?.agent?.enabled == true  && ec.agent.isStopped == false) ec.agent.isStopped = true;
        }

        
    }

   
}
