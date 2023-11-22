using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp : MonoBehaviour
{
    [SerializeField] SoundManager_ sm;

    private float time_keydown;
    [SerializeField] private float timeBetKeydown;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A) & isOn())
        {
            time_keydown = Time.time;

            sm.playSE("shot");
        }
    }

    private bool isOn()
    {
        return timeBetKeydown + time_keydown < Time.time; 
    }
}
