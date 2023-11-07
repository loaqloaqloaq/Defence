using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy1Navigator : MonoBehaviour
{
    public Transform target;    
    private Animator animator;
    private Enemy1Controller ec;
   
    // Start is called before the first frame update
    
    void Start()
    {       
        animator = GetComponent<Animator>();
        ec = GetComponent<Enemy1Controller>();   
    }

    // Update is called once per frame
    void Update()
    {
        if (ec?.agent?.enabled == true && !ec.attacking)
        {
            target = ec.target ?? GameObject.Find("Gate1").transform ?? GameObject.Find("Gate2").transform ?? GameObject.Find("Gate3").transform;
            if (Vector3.Distance(transform.position, target.position) > 1.5f && !ec.attacking)
            {
                animator.SetBool("walking", true);
                ec.agent.isStopped = false;
                var targetPos = target.position;
                if (target.name.StartsWith("Gate"))
                    targetPos.x = (target.position - (target.position - transform.position).normalized * 3f).x;
                ec.agent.destination = targetPos;

            }
            else
            {
                animator.SetBool("walking", false);
                ec.agent.isStopped = true;
            }
        }
        else {
            animator.SetBool("walking", false);
        }

        
    }

   
}
