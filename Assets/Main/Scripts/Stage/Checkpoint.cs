using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Vector3 pos;
    // Start is called before the first frame update
    void Start()
    {
        RaycastHit hitInfo;
        Ray ray = new Ray();
        ray.origin = transform.position;
        ray.direction = transform.up;
        if (Physics.Raycast(ray, out hitInfo))
        {
            pos = hitInfo.point;            
        }
        else
        {
            ray.direction = -transform.up;
            if (Physics.Raycast(ray, out hitInfo))
            {
                pos = hitInfo.point;
            }
        }
    }
    public Vector3 GetPos() { 
        return pos==Vector3.zero ? transform.position : pos;
    }
   
}
