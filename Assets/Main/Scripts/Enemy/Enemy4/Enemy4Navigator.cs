using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy4Navigator : MonoBehaviour
{
    public Transform target, destination;
    private Animator animator;
    private Enemy4Controller ec;
    private float lastRotation;
    private Transform g1, g2, g3;

    private Transform routes, area, route;
    private Vector3 checkPoint;
    private int checkPointIndex;
    private float offsetRange;

    // Start is called before the first frame update

    void Start()
    {
        animator = GetComponent<Animator>();
        ec = GetComponent<Enemy4Controller>();
        lastRotation = transform.localEulerAngles.y;
        g1 = GameObject.Find("Gate1").transform;
        g2 = GameObject.Find("Gate2").transform;
        g3 = GameObject.Find("Gate3").transform;

        routes = GameObject.Find("Routes")?.transform??null;

        destination = g1;

        offsetRange = 5f;
    }

    // Update is called once per frame
    void Update()
    {        
        if (ec?.agent?.enabled == true && !ec.attacking)
        {
            target = ec.target ?? g1 ?? g2 ?? g3;
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
                if (routes == null) target = ec.target;
                else
                {
                    if ((target == g1 && area != routes.GetChild(0)) || (target == g2 && area != routes.GetChild(1)) || (target == g3 && area != routes.GetChild(2))) RandomRoute();
                    CheckRoute();
                }

                Vector3 targetPos = Vector3.zero;
                if (target.CompareTag("Player")) targetPos = target.position;
                else targetPos = checkPoint;
                ec.agent.destination = targetPos;

            }
            else
            {
                animator.SetBool("walking", false);
                if (ec.agent.isStopped == false) ec.agent.isStopped = true;
            }
        }
        else
        {
            animator.SetBool("walking", false);
            if (ec?.agent?.enabled == true && ec.agent.isStopped == false)
                ec.agent.isStopped = true;
        }


    }

    void NextCheckPoint()
    {
        checkPointIndex++;
        if (checkPointIndex >= route.childCount)
        {
            destination = ec.gate;
            checkPoint = destination.position;
            checkPoint.x = (destination.position - (destination.position - destination.position).normalized * 2.2f).x;
            return;
        }
        destination = route.GetChild(checkPointIndex);
        checkPoint = destination.position;
        checkPoint.x += Random.Range(-offsetRange, offsetRange);
        checkPoint.z += Random.Range(-offsetRange, offsetRange);        
    }
    void RandomRoute()
    {
        if (ec.gate == g1) area = routes.GetChild(0);
        else if (ec.gate == g2) area = routes.GetChild(1);
        else if (ec.gate == g3) area = routes.GetChild(2);
        route = area.GetChild(Random.Range(0, area.childCount));
        checkPointIndex = -1;
        NextCheckPoint();
    }

    void CheckRoute()
    {
        if (destination.CompareTag("Gate")) return;
        Vector3 pos = transform.position;
        Vector3 checkPos = checkPoint;
        pos.y = 0;
        checkPos.y = 0;
        if (Vector3.Distance(checkPos, pos) < 0.2f) NextCheckPoint();


    }


}
