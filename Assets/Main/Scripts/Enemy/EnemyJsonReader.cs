using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyJsonReader
{
    public EnemyData Enemy1;
}

[Serializable]
public class EnemyData 
{
    public float hp,moveSpeed, atk;
    public Drop drop;
}
[Serializable]
public class Drop 
{
    public float ammo;
    public float health;
}

[Serializable]
public class Pattern {
    public string[] pattern;
}