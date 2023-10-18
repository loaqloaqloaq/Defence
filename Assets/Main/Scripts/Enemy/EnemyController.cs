using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour, IDamageable
{
    [HideInInspector]
    public float HP, MAXHP;
    private Animator animator;
    private float destoryTimer, destoryTime;
    private bool dead;
    // Start is called before the first frame update
    void Start()
    {
        MAXHP = 100f;
        HP = MAXHP;

        animator=GetComponent<Animator>();

        destoryTime = 3.0f;
        destoryTimer = 0;
        dead = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (HP <= 0)
        {
            GetComponent<NavMeshAgent>().enabled = false;
            destoryTimer += Time.deltaTime;
            if (destoryTimer >= destoryTime) Destroy(gameObject);
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
}
