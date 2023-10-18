using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public Vector2 input { get; private set; }
    public bool Sprint { get; private set; }
    
    public bool Jump { get; private set; }

    public bool Toggle { get; private set; }

    public bool isFiring { get; private set; }

    public bool Reload { get; private set; }

    public float Fire { get; private set; }

    public float Zoom { get; private set; }
    public bool zoom { get; private set; }

    // Update is called once per frame
    void Update()
    {
        float xInput = Input.GetAxis("Horizontal");
        float yInput = Input.GetAxis("Vertical");

        input = new Vector2(xInput, yInput);

        Sprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey("joystick button 5");

        Jump = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown("joystick button 0");

        Toggle = Input.GetKeyDown(KeyCode.T) || Input.GetKeyDown("joystick button 2");

        isFiring = Input.GetMouseButton(0);

        Reload = Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown("joystick button 4");

        Fire = Input.GetAxis("RightTrigger");

        Zoom = Input.GetAxisRaw("LeftTrigger");
        zoom = Input.GetMouseButtonDown(1);
    }
}
