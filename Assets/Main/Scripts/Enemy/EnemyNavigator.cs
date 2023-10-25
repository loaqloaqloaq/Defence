using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavigator : MonoBehaviour
{
    public Transform target;    
    private Animator animator;
    private EnemyController ec;
   
    // Start is called before the first frame update
    
    void Start()
    {       
        animator = GetComponent<Animator>();
        ec = GetComponent<EnemyController>();   
    }

    // Update is called once per frame
    void Update()
    {
        if (ec?.agent?.enabled == true && !ec.attacking)
        {
            target = ec.target ?? GameObject.Find("Gate1").transform ?? GameObject.Find("Gate2").transform ?? GameObject.Find("Gate3").transform;
            if (!ec.attacking)
            {
                animator.SetBool("walking", true);
                ec.agent.isStopped = false;
                ec.agent.destination = target.position;
            }
            else
            {
                animator.SetBool("walking", false);
                ec.agent.isStopped = true;
            }
        }

        
    }

   
}
