using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy4Navigator : MonoBehaviour
{
    public Transform target;
    private Animator animator;
    private Enemy4Controller ec;
    private float lastRotation;

    // Start is called before the first frame update

    void Start()
    {
        animator = GetComponent<Animator>();
        ec = GetComponent<Enemy4Controller>();
        lastRotation = transform.localEulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (ec?.agent?.enabled == true && !ec.attacking)
        {
            target = ec.target ?? GameObject.Find("Gate1").transform ?? GameObject.Find("Gate2").transform ?? GameObject.Find("Gate3").transform;
            if (Vector3.Distance(transform.position, target.position) > 2.5f && !ec.attacking)
            {
                animator.SetBool("walking", true);
                if (transform.localEulerAngles.y > (lastRotation + 2f))
                {
                    //right
                    animator.SetBool("forward", false);
                    animator.SetBool("right",true);
                    animator.SetBool("left", false);
                }
                else if (transform.localEulerAngles.y < (lastRotation - 2f))
                {
                    //left
                    animator.SetBool("forward", false);
                    animator.SetBool("right", false);
                    animator.SetBool("left", true);
                }
                else {
                    //front
                    animator.SetBool("forward", true);                    
                    animator.SetBool("right", false);
                    animator.SetBool("left", false);
                }
                lastRotation = transform.localEulerAngles.y;
                ec.agent.isStopped = false;
                var targetPos = target.position;
                if (target.name.StartsWith("Gate"))
                    targetPos.x = (target.position - (target.position - transform.position).normalized * 2.2f).x;
                ec.agent.destination = targetPos;

            }
            else
            {
                animator.SetBool("walking", false);
                ec.agent.isStopped = true;
            }
        }
        else
        {
            animator.SetBool("walking", false);
        }


    }


}
