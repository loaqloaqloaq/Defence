using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Timer : MonoBehaviour
{
    TextMeshProUGUI text;  
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();       
    }

    // Update is called once per frame
    void Update()
    {        
       
    }

    public void setTimerString(float time) {
        float ms = time * 1000;
        string timeStr = String.Format("{0:00}:{1:00}:{2:000}", (int)ms / 60000, (int)(ms / 1000) % 60, ms % 1000);
        text.text = "Timer : " + timeStr;
    }

}
