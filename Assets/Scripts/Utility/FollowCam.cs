using Cinemachine;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    private CinemachineVirtualCamera followCam;

    private void Awake()
    {
        followCam = GetComponent<CinemachineVirtualCamera>();

        Transform target = FindObjectOfType<PlayerController>().transform.Find("CameraLookAt");

        if (target)
        {
            followCam.LookAt = target;
            followCam.Follow = target;
        }
        else
        {
            Debug.Log("Follow Cam Follow Target Is Null");
        }
    }

}
