using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCompassController : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject player;
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, player.transform.eulerAngles.y);
    }
}
