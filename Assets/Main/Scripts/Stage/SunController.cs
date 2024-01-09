using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SunController : MonoBehaviour
{
    [SerializeField]float rotateSpeed;
    // Start is called before the first frame update
    void Start()
    {
        if (rotateSpeed <= 0) rotateSpeed = 1f;
    }

    // Update is called once per frame
    void Update()
    {               
       transform.Rotate(-rotateSpeed*Time.deltaTime, 0, 0, Space.Self);       
    }
}
