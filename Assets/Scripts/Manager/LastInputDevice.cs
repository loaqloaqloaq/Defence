using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum InputDevice
{
    KEYBOARD, GAMEPAD
};

public class LastInputDevice : MonoBehaviour
{
    // Start is called before the first frame update
    PlayerInput i;
    void Start()
    {
       i=GameObject.FindWithTag("Player").GetComponent <PlayerInput>();        
    }

    // Update is called once per frame
    void Update()
    {        
        //player movement           
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            if(Input.GetAxis("LStick X") != 0 || Input.GetAxis("LStick Y") != 0) GameManager.LastInputDevice = InputDevice.GAMEPAD;
            else GameManager.LastInputDevice = InputDevice.KEYBOARD;           
        }
        //camera movement
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0) { 
            if(Input.GetAxis("RStick X") != 0 || Input.GetAxis("RStick Y") != 0) GameManager.LastInputDevice = InputDevice.GAMEPAD;
            else GameManager.LastInputDevice = InputDevice.KEYBOARD;
        }

        //catch all key input
        foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
            {
                if (kcode.ToString().StartsWith("Joystick"))
                {
                    // if key name has joystick means it input from joystick
                    GameManager.LastInputDevice = InputDevice.GAMEPAD;
                }
                else
                {
                    // if not than is keyboard and mouse
                    GameManager.LastInputDevice = InputDevice.KEYBOARD;
                }
            }
        }

    }
}
