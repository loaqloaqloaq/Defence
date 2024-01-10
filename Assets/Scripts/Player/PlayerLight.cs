using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLight : MonoBehaviour
{
    [SerializeField] GameObject light;
    [SerializeField] Transform CameraLookAt;
    bool lightOn;
    // Start is called before the first frame update
    void Start()
    {
        if(light == null)light = transform.Find("light").gameObject;
        lightOn = false;
        light.SetActive(lightOn);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Player Light"))
        {
            lightOn = !lightOn;
            light.SetActive(lightOn);
        }
        if (lightOn && CameraLookAt) { 
            light.transform.localEulerAngles= CameraLookAt.localEulerAngles;
        }
    }
}
