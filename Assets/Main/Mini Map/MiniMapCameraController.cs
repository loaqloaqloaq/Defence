using UnityEngine;

public class MiniMapCameraController : MonoBehaviour
{
    Transform player;

    void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    void LateUpdate()
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
