using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLight : MonoBehaviour
{
    [SerializeField] GameObject light;
    bool lightOn;
    // Start is called before the first frame update
    void Start()
    {
        light = transform.Find("light").gameObject;
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
    }
}
