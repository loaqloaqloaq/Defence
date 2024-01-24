using UnityEngine;

public enum AttackType
{ 
    Common,
    Fire,
    Explosion
}

public struct DamageMessage 
{
    public AttackType attackType;
    public GameObject damager; //攻撃側
    public float amount;

    public Vector3 hitPoint;
    public Vector3 hitNormal;
}
