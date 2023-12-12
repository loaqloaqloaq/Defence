using UnityEngine;

public class EnemyCollider : MonoBehaviour, EnemyPart
{
    Transform rootObject;
    
    public Part part;

    public IEnemyDamageable root;
    public bool ApplyDamage(DamageMessage damageMessage)
    {
        //Debug.Log(rootObject.ToString());
        root.ApplyDamage(damageMessage,part);
        return true;
    }
    public Part GetPart()
    {
        return part;
    }
    // Start is called before the first frame update
    void Start()
    {
        rootObject = transform.parent;
        while (!rootObject.name.StartsWith("Enemy")) {
            rootObject = rootObject.parent;
        }
        //Debug.Log(rootObject.ToString());
        root = rootObject.GetComponent<IEnemyDamageable>();
    }

    public bool IsDead()
    {
        return root.IsDead();
    }

    
}
