using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SunController : MonoBehaviour
{
    private Material skybox;     
    private float Tfactor;
    enum DayTime { 
        Sun,Sunset,Night
    }
    
    [SerializeField] float rotateSpeed;
    [SerializeField] DayTime dayTime;     
    [SerializeField] float DayNigntChangeSpeed;
    
    Light envlight;
    Color dayColor, sunsetColor;
    //6B7AA0


    // Start is called before the first frame update
    void Start()
    {
        if (rotateSpeed <= 0) rotateSpeed = 1f;
        skybox = RenderSettings.skybox;
        dayTime = DayTime.Sun;        
        if (DayNigntChangeSpeed <= 0) DayNigntChangeSpeed = 0.5f;
        envlight = GameObject.Find("Directional Light").GetComponent<Light>();

        Tfactor = -1;
        dayColor = Colors.FromHex("94CFFF");
        sunsetColor = Colors.FromHex("FFD794");

        DayTimeUpdate();
        SkyBoxUpdate(false); 

    }

    // Update is called once per frame
    void Update()
    {               
        transform.Rotate(-rotateSpeed*Time.deltaTime, 0, 0, Space.Self);

        DayTimeUpdate();
        SkyBoxUpdate();


    }
    void DayTimeUpdate() {        
        if      (transform.localRotation.eulerAngles.x > 335f)  dayTime = DayTime.Sunset;
        else if (transform.localRotation.eulerAngles.x > 200f)  dayTime = DayTime.Night;
        else if (transform.localRotation.eulerAngles.x > 155f)  dayTime = DayTime.Sunset;
        else if (transform.localRotation.eulerAngles.x > 20f)   dayTime = DayTime.Sun;        
        else                                                    dayTime = DayTime.Sunset;
    }

    void SkyBoxUpdate(bool fade = true) {
        if (dayTime == DayTime.Night && Tfactor != 2)
        {
            if (!fade) Tfactor = 2;
            else
            {
                var diff = (2 - Tfactor) / math.abs(2 - Tfactor);
                Tfactor += diff * Time.deltaTime * DayNigntChangeSpeed;
                if (math.abs(2 - Tfactor) < 0.1) Tfactor = 2;                
            }
            skybox.SetFloat("_value", Tfactor);
            envlight.color = Color.Lerp(dayColor, sunsetColor, math.abs(2 - Tfactor));
        }
        else if (dayTime == DayTime.Sunset && Tfactor != 1)
        {
            if (!fade) Tfactor = 1;
            else
            {
                var diff = (1 - Tfactor) / math.abs(1 - Tfactor);
                Tfactor += diff * Time.deltaTime * DayNigntChangeSpeed;
                if (math.abs(1 - Tfactor) < 0.1) Tfactor = 1;               
            }
            skybox.SetFloat("_value", Tfactor);
            envlight.color = Color.Lerp(sunsetColor, dayColor, math.abs(1 - Tfactor));
        }
        else if (dayTime == DayTime.Sun && Tfactor != 0)
        {
            if (!fade) Tfactor = 0;
            else
            {
                var diff = (0 - Tfactor) / math.abs(0 - Tfactor);
                Tfactor += diff * Time.deltaTime * DayNigntChangeSpeed;
                if (math.abs(0 - Tfactor) < 0.1) Tfactor = 0;                
            }
            skybox.SetFloat("_value", Tfactor);
            envlight.color = Color.Lerp(dayColor, sunsetColor, math.abs(0 - Tfactor));
        }        
    }
}


