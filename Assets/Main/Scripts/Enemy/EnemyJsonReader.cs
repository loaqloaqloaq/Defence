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
    public Drop drop;
    public Resist resist;
}
[Serializable]
public class Drop 
{
    public float ammo;
    public float health;
}
[Serializable]
public class Resist
{
    public bool fire, bullet, explode;
}

[Serializable]
public class Pattern {
    public string[] pattern;
    public float genFreq;
    public float randomRange;
    public int maxEnemy;    
}