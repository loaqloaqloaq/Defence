using UnityEngine;

public class CrosshairTarget : MonoBehaviour
{
    Camera mainCamera;

    Ray ray;
    RaycastHit hitInfo;

    private const float distance = 200.0f;

    [SerializeField] LayerMask mask;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    

    // Update is called once per frame
    void Update()
    {

        ray.origin = mainCamera.transform.position;
        ray.direction = mainCamera.transform.forward;
        //Debug.DrawRay(ray.origin, ray.direction * distance, Color.red);
        if (Physics.Raycast(ray, out hitInfo, distance, mask))
        {
            transform.position = hitInfo.point;
            //Debug.Log(hitInfo.transform.name);
        }
        else
        {
            transform.position = ray.origin + ray.direction * distance;
        }
        
    }
}
