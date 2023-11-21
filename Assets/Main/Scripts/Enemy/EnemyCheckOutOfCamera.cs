using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCheckOutOfCamera : MonoBehaviour
{
    [SerializeField] EnemyController ec;
    private void Start()
    {
        
    }
    void OnBecameInvisible()
    {
        Debug.Log("out of cam");        
        ec.setRenders(false);
    }

    // Enable this script when the GameObject moves into the camera's view
    void OnBecameVisible()
    {
        Debug.Log("view by cam");
        ec.setRenders(true);
    }   
}
