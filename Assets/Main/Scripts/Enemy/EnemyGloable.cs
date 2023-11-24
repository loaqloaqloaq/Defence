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

    public Transform player;
    public Transform gate;
    public GateController gc;
    Transform gate1, gate2, gate3;
    GateController g1,g2,g3;

    void Awake() {       
        EnemyJson = JsonUtility.FromJson<EnemyJsonReader>(EnemyJsonFile.ToString());
        dropPrefab.Add("ammo", ammoPack);
        dropPrefab.Add("health", healthPack);

        gate1 = GameObject.Find("Gate1").transform;
        g1 = gate1.GetComponent<GateController>();
        gate2 = GameObject.Find("Gate2").transform;
        g2 = gate2.GetComponent<GateController>();
        gate3 = GameObject.Find("Gate3").transform;
        g3 = gate3.GetComponent<GateController>();

        gate = gate1 != null ? gate1 : gate2 != null ? gate2 : gate3;

        player = GameObject.Find("Player").transform;
    }

    void Update()
    {
        if (gate1 != null && !g1.broke) { 
            gate=gate1;
        }
        else if (gate2 != null && !g2.broke)
        {
            gate = gate2;
        }
        else if (gate3 != null && !g3.broke)
        {
            gate = gate3;
        }
    }
}
