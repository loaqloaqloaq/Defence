using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour
{
    public Vector2 input { get; private set; }
    public bool Sprint { get; private set; }
    
    public bool Jump { get; private set; }

    public bool Toggle { get; private set; }

    public bool IsFiring { get; private set; }

    public bool Reload { get; private set; }

    public bool Fire { get; private set; }

    public bool InterAction { get; private set; }

    public float Zoom { get; private set; }
    public bool zoom { get; private set; }

    public bool Grenade { get; private set; }

    public float D_XAxis { get; private set; }
    public float D_YAxis { get; private set; }

    public bool Alpha1 { get; private set; }
    public bool Alpha2 { get; private set; }
    public bool Alpha3 { get; private set; }
    public bool Alpha4 { get; private set; }

    public bool Minimap { get; private set; }
    public bool controllerConnected { get; private set; }
    IEnumerator CheckForControllers()
    {
        while (true)
        {
            string[] controllers = Input.GetJoystickNames();

            int length = 0;

            foreach (string controller in controllers)
            {
                if (controller.Length > 0) ++length;
            }

            if (!controllerConnected && length > 0)
            {
                controllerConnected = true;
                Debug.Log("Connected");

            }
            else if (controllerConnected && length == 0)
            {
                controllerConnected = false;
                Debug.Log("Disconnected");
            }


            yield return new WaitForSeconds(1f);
        }
    }

    void Awake()
    {
        StartCoroutine(CheckForControllers());
    }

    // Update is called once per frame
    void Update()
    {
        float xInput = Input.GetAxis("Horizontal");
        float yInput = Input.GetAxis("Vertical");

        input = new Vector2(xInput, yInput);

        Sprint = Input.GetKey(KeyCode.LeftShift) || Input.GetAxis("RightTrigger") > 0.1f;

        Jump = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown("joystick button 0");

        Grenade = Input.GetKeyDown(KeyCode.G) || Input.GetKeyDown("joystick button 1");

        Toggle = Input.GetKeyDown(KeyCode.T) || Input.GetKeyDown("joystick button 2");

        InterAction = Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown("joystick button 3");

        IsFiring = Input.GetMouseButton(0);

        Reload = Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown("joystick button 4");

        Fire = Input.GetKey("joystick button 5");

        Zoom = Input.GetAxisRaw("LeftTrigger");

        zoom = Input.GetMouseButton(1);

        if (Mathf.Abs(D_YAxis) < 0.05f)
        {
            D_XAxis = Input.GetAxisRaw("Dpad X");
        }

        if (Mathf.Abs(D_XAxis) < 0.05f)
        {
            D_YAxis = Input.GetAxisRaw("Dpad Y");
        }

        Alpha1 = D_YAxis > 0.05f || Input.GetKeyDown(KeyCode.Alpha1);
        Alpha2 = D_XAxis > 0.05f || Input.GetKeyDown(KeyCode.Alpha2);
        Alpha3 = D_YAxis < -0.05f || Input.GetKeyDown(KeyCode.Alpha3);
        Alpha4 = D_XAxis < -0.05f || Input.GetKeyDown(KeyCode.Alpha4);
    }

    public void InitInputXY()
    {
        input = Vector2.zero;
    }
}
