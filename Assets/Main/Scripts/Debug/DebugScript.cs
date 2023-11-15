using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugScript: MonoBehaviour
{
    [SerializeField]
    GameObject warpPoint;
    [SerializeField]
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) {
            Debug.Log("Pressed");
            player.GetComponent<CharacterController>().enabled = false;
            player.transform.position = warpPoint.transform.position;
            player.GetComponent<CharacterController>().enabled = true;
        }
    }
}
