using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyJsonReader
{
    public EnemyData Enemy1, Enemy2, Enemy3, Enemy4, Enemy5;    
}

[Serializable]
public class EnemyData 
{
    public float hp,moveSpeed, atk, AttackDuration, AttackStop, AttackRadius;
    public int reward;
    public Drop drop;
    public Type resist, weakness;
}
[Serializable]
public class Drop 
{
    public float ammo;
    public float health;
}
[Serializable]
public class Type
{
    public bool fire, common, explode;
    public float persent;
}

[Serializable]
public class Pattern {
    public string[] pattern;
    public float genFreq;
    public float randomRange;
    public MaxEnemy[] maxEnemy;    
}

[Serializable]
public class MaxEnemy {
    public float timeLeftPresent;
    public int maxEnemy;
}
