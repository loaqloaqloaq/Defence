using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TurretData
{
    public TurretStatus t1, t2, t3;
}

[Serializable]
public class TurretStatus
{
    public float atk;
    public int cost;
   
}

public class TurretJsonLoader : MonoBehaviour
{
    // Start is called before the first frame update

    //Singleton       
    private static TurretJsonLoader instance;
    public static TurretStatus T1
    {
        get
        {            
            return Instance.getData().t1;
        }
    }
    public static TurretStatus T2 {
        get
        {
            return Instance.getData().t2;
        }
    }
    public static TurretStatus T3{
        get
        {
            return Instance.getData().t3;
        }
    }


    [SerializeField]
    public TextAsset TurretJsonFile;

    public TurretData turretData=null;

    public static TurretJsonLoader Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<TurretJsonLoader>();            
            return instance;
        }
    }
    

    void Awake()
    {
        if (TurretJsonFile)
        {
            turretData = JsonUtility.FromJson<TurretData>(TurretJsonFile.ToString());            
        }
    }

    private TurretData getData() { 
        if(turretData==null) turretData = JsonUtility.FromJson<TurretData>(TurretJsonFile.ToString());
        return turretData;
    }
}
