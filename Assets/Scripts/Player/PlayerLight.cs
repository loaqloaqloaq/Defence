using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class PlayerLight : MonoBehaviour
{
    [SerializeField] GameObject playerLight;
    [SerializeField] Transform CameraLookAt;
    bool lightOn;
    // Start is called before the first frame update
    void Start()
    {
        if(playerLight == null) playerLight = transform.Find("light").gameObject;
        lightOn = false;
        playerLight.SetActive(lightOn);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Player Light"))
        {
            lightOn = !lightOn;
            playerLight.SetActive(lightOn);
        }
        if (lightOn && CameraLookAt) {
            RaycastHit hitInfo;
            Ray ray=new Ray();
            ray.origin = Camera.main.transform.position;
            ray.direction = Camera.main.transform.forward;
            if (Physics.Raycast(ray, out hitInfo))
            {
                playerLight.transform.LookAt(hitInfo.point);
            }
            else
            {
                playerLight.transform.localEulerAngles = CameraLookAt.localEulerAngles;
            }
            
        }
    }
}
