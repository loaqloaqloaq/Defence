using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SunController : MonoBehaviour
{
    private Material skybox;     
    private float Tfactor;

    [SerializeField] Material nightSkyBox;
    [SerializeField] float rotateSpeed;
    [SerializeField] bool isNight;
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

        Tfactor = DayValue;
        skybox.SetColor("_Tint", new Color(Tfactor, Tfactor, Tfactor, 1));
    }

    // Update is called once per frame
    void Update()
    {               
        transform.Rotate(-rotateSpeed*Time.deltaTime, 0, 0, Space.Self);
        Debug.Log(transform.localRotation.eulerAngles);

        isNight = transform.localRotation.eulerAngles.x > 200f;

        if (isNight && Tfactor >= NightValue)
        {
            Tfactor -= Time.deltaTime * DayNigntChangeSpeed;
            skybox.SetColor("_Tint", new Color(Tfactor, Tfactor, Tfactor, 1));
            if (Tfactor <= NightValue) { 
            
            }
        }
        else if(!isNight && Tfactor <= DayValue) {
            Tfactor += Time.deltaTime * DayNigntChangeSpeed;
            skybox.SetColor("_Tint", new Color(Tfactor, Tfactor, Tfactor, 1));
        }


    }
}
