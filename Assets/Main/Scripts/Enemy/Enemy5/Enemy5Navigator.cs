using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy5Navigator : MonoBehaviour
{
    public Transform target, destination;
    private Animator animator;
    private Enemy5Controller ec;
    private float lastRotation;    

    private Transform routes, area, route;
    
    private int checkPointIndex;
    private float offsetRange,offset;

    // Start is called before the first frame update

    void Start()
    {
        animator = GetComponent<Animator>();
        ec = GetComponent<Enemy5Controller>();
        lastRotation = transform.localEulerAngles.y;         

        offsetRange = 2.5f;
        offset = Random.Range(-offsetRange, offsetRange);
    }

    // Update is called once per frame
    void Update()
    {
        if (ec?.agent?.enabled == true && !ec.attacking)
        {
            target = ec.target;
            if (target == null) return;
            if (Vector3.Distance(transform.position, target.position) > 1.5f)
            {
                //animator.SetBool("walking", true);
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

                Vector3 targetPos = target.transform.position;
                targetPos.x += offset;
                targetPos.z += offset;
                ec.agent.destination = targetPos;

            }
            else
            {
                animator.SetBool("forward", false);
                animator.SetBool("right", false);
                animator.SetBool("left", false);

                if (ec.agent.isStopped == false) ec.agent.isStopped = true;
            }
        }
        else
        {
            animator.SetBool("forward", false);
            animator.SetBool("right", false);
            animator.SetBool("left", false);
            if (ec?.agent?.enabled == true && ec.agent.isStopped == false)
                ec.agent.isStopped = true;
        }
    }    


}
