using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy2Navigator : MonoBehaviour
{
    public Transform target;    
    private Animator animator;
    private Enemy2Controller ec;
    private Transform player;


    // Start is called before the first frame update

    void Start()
    {
        animator = GetComponent<Animator>();
        ec = GetComponent<Enemy2Controller>();
        player = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (ec?.agent?.enabled == true && !ec.attacking)
        {
            target = ec.target ?? player;
            if (Vector3.Distance(transform.position, target.position) > 1.5f && !ec.attacking)
            {
                animator.SetBool("walking", true);                
                var targetPos = target.position;               
                ec.agent.destination = targetPos;
            }
            else
            {
                animator.SetBool("walking", false);                
            }
        }
        else {
            animator.SetBool("walking", false);            
        }        
    }

   
}
