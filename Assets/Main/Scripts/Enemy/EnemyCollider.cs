using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollider : MonoBehaviour,IDamageable
{
    Transform rootObject;
    IDamageable root;
    public bool ApplyDamage(DamageMessage damageMessage)
    {
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
        while (!rootObject.CompareTag("Enemy")) {
            rootObject = rootObject.parent;
        }
        root = rootObject.GetComponent<IDamageable>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
