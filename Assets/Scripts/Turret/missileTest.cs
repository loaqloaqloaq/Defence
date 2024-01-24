using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class missileTest : MonoBehaviour
{
    [SerializeField] GameObject m;
    [SerializeField] Transform target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var m_ = Instantiate(m, transform.position, Quaternion.identity);
            m_.GetComponent<HomingMissile>().Target = target;
        }
    }
}
