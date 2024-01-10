using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    private Transform spawnPoint;
    // Start is called before the first frame update
    void Awake()
    {
        spawnPoint = transform.GetChild(2);
    }

    public Vector3 GetSpawnPoint() { 
        return spawnPoint.position;
    }
    
}
