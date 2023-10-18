using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavigator : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;
    private Animator animator;
    // Start is called before the first frame update
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        if (player == null) player = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, player.position) > 1f)
        {
            animator.SetBool("walking", true);
            agent.isStopped = false;
            agent.destination = player.position;
        }
        else {
            animator.SetBool("walking", false);
            agent.isStopped = true;
        }
    }
}
