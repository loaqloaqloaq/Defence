using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        //入力デバイス
        foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
        {
            if (i.input != Vector2.zero) {
                var pressedWASD = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
                if (pressedWASD) GameManager.LastInputDevice = InputDevice.KEYBOARD;                
                else GameManager.LastInputDevice = InputDevice.GAMEPAD;
                
            }
            if (Input.GetKeyDown(kcode)) {
                if (kcode.ToString().StartsWith("Joystick"))
                {
                    GameManager.LastInputDevice = InputDevice.GAMEPAD;
                }
                else {
                    GameManager.LastInputDevice = InputDevice.KEYBOARD;
                }
            }
            if (Input.GetAxis("Mouse X") != 0 && Input.GetAxis("Mouse Y") != 0)  
                GameManager.LastInputDevice = InputDevice.KEYBOARD;
        }       
    }
}
