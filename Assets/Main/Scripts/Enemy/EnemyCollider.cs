using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class EnemyCollider : MonoBehaviour,IDamageable
{
    Transform rootObject;
    
    public IDamageable root;
    public bool ApplyDamage(DamageMessage damageMessage)
    {
        Debug.Log(rootObject.ToString());
        root.ApplyDamage(damageMessage);
        return true;
    }

    public void Damage(int damage)
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        rootObject = transform.parent;
        while (!rootObject.name.StartsWith("Enemy")) {
            rootObject = rootObject.parent;
        }

        Debug.Log(rootObject.ToString());
        root = rootObject.GetComponent<IDamageable>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsDead()
    {
        return root.IsDead();
    }
}
