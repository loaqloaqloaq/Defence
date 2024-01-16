using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SunController : MonoBehaviour
{
    private Material skybox;     
    private float Tfactor;
    
    [SerializeField] float rotateSpeed;
    [SerializeField] public bool isNight;
    [SerializeField] float NightValue, DayValue;
    [SerializeField] float DayNigntChangeSpeed; 
    

    // Start is called before the first frame update
    void Start()
    {
        if (rotateSpeed <= 0) rotateSpeed = 1f;
        skybox = RenderSettings.skybox;        
        isNight = false;
        if (NightValue <= 0) NightValue = 0.1f;
        if (DayValue <= 0) DayValue = 0.5f;
        if (DayNigntChangeSpeed <= 0) DayNigntChangeSpeed = 0.5f;

        Tfactor = 0;
        skybox.SetFloat("_value", Tfactor);
        
    }

    // Update is called once per frame
    void Update()
    {               
        transform.Rotate(-rotateSpeed*Time.deltaTime, 0, 0, Space.Self);        

        isNight = transform.localRotation.eulerAngles.x > 200f;

        if (isNight && Tfactor < 1)
        {
            Tfactor += Time.deltaTime * DayNigntChangeSpeed;
            skybox.SetFloat("_value", Tfactor);
        }
        else if(!isNight && Tfactor > 0) {   
            Tfactor -= Time.deltaTime * DayNigntChangeSpeed;
            skybox.SetFloat("_value", Tfactor);
        }


    }  
}


