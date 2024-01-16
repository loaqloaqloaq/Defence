using UnityEngine;


public class Checkpoint : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;
   
    public Vector3 GetPos(Vector3 offset) {
        var pos = GetFloor(offset);
        return pos == Vector3.zero ? transform.position : pos;
    }

    public Vector3 GetFloor(Vector3 offset) {
        var pos = Vector3.zero;
        
        RaycastHit hitInfo;   
        var origin= transform.position+offset;
        if (Physics.Raycast(origin, Vector3.up, out hitInfo, Mathf.Infinity,LayerMask.GetMask("Floor")))
        {
            pos = hitInfo.point ;
            Debug.LogWarning(hitInfo.transform.name);
        }
        else if (Physics.Raycast(origin, -Vector3.up, out hitInfo, Mathf.Infinity,LayerMask.GetMask("Floor")))
        {   
            pos = hitInfo.point;            
        }
        
        Debug.DrawRay(origin, Vector3.up *999f , UnityEngine.Color.blue,10f);        
        return pos;
    }
   
}
