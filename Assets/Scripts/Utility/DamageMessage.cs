using UnityEngine;

public struct DamageMessage 
{
    public GameObject damager; //攻撃側
    public float amount;

    public Vector3 hitPoint;
    public Vector3 hitNormal;
}