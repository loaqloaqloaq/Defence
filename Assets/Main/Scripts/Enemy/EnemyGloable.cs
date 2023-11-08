using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyGloable :MonoBehaviour
{
    [SerializeField]
    TextAsset EnemyJsonFile;

    public EnemyJsonReader EnemyJson;

    public GameObject explosion;

    public Dictionary<string, GameObject> dropPrefab = new Dictionary<string, GameObject>();

    [SerializeField]
    GameObject ammoPack, healthPack;    

    void Awake() {       
        EnemyJson = JsonUtility.FromJson<EnemyJsonReader>(EnemyJsonFile.ToString());
        dropPrefab.Add("ammo", ammoPack);
        dropPrefab.Add("health", healthPack);        
    }


}
