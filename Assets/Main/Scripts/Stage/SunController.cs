using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunController : MonoBehaviour
{
    [SerializeField]float rotateSpeed;
    // Start is called before the first frame update
    void Start()
    {
        if (rotateSpeed <= 0) rotateSpeed = 20f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotateSpeed * Time.deltaTime, rotateSpeed * Time.deltaTime, 0);
    }
}
