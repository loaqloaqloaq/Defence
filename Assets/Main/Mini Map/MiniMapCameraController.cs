using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCameraController : MonoBehaviour
{
    // Start is called before the first frame update
    Transform player;
    void Start()
    {
        player = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos= player.position;
        transform.position = new Vector3(pos.x, transform.position.y, pos.z);
        transform.eulerAngles = new Vector3(
            transform.eulerAngles.x,
            player.eulerAngles.y,
            transform.eulerAngles.z
        );
    }
}
