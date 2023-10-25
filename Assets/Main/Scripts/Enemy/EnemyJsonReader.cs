using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyJsonReader
{
    public EnemyData Enemy1;
}

public class EnemyData 
{
    public float moveSpeed, atk;
    public Drop drop;
}

public class Drop 
{
    public float ammo;
}
