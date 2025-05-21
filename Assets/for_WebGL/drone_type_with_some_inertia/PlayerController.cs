using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Transform playerCamera = null;
    
    bool lockCursor = true; // I don't remember why
    
    float mouseSensitivity = 1.0f;
    float keyRotSensitivity = 6.0f;
    float rotDamper = 2.0f;
    float keySlewSensitivity = 6.0f;
    float slewDamper = 3.0f;
    float minSpeedSqrt = 0.1f;

    float cameraPitch = 0.0f;
    float vTOL = 0.0f;
    float strafe = 0.0f;
    float forward = 0.0f;
    float yaw = 0.0f;
    float pitch = 0.0f;
    float yInvert = 1;
    
    CharacterController controller = null;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
	
        if(lockCursor)
	{
	    Cursor.lockState = CursorLockMode.Locked;
	    Cursor.visible = false;
	}
    }

    void Update()
    {
        UpdateSettings();
        UpdateRotation();
	UpdateMovement();
    }
    
    void UpdateSettings()
    {
        // Quit on escape key; not needed for WebGL build
        //if(Input.GetKey("escape"))
	//{
	//    Application.Quit();
	//}
	// yinvert not used in drone-type setup
	if(Input.GetKeyUp("y"))
	{
	    yInvert *= -1;
	}
    }
    
    void UpdateRotation()
    {
        float d = (1.0f - (rotDamper * Time.deltaTime));
	if(Input.GetKey("d")) yaw += keyRotSensitivity;
	if(Input.GetKey("a")) yaw -= keyRotSensitivity;
	yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
	yaw *= d;
	transform.Rotate(Vector3.up * yaw * Time.deltaTime);
	
	if(Input.GetKey("q")) pitch -= keyRotSensitivity;
	if(Input.GetKey("e")) pitch += keyRotSensitivity;
	pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
	pitch *= d;
	cameraPitch += pitch * Time.deltaTime;
	cameraPitch = Mathf.Clamp(cameraPitch, -15.0f, 90.0f);
	playerCamera.localEulerAngles = Vector3.right * cameraPitch;	
    }
    
    void UpdateMovement()
    {
	float m = Time.deltaTime * keySlewSensitivity;
	bool vertbraking = true;
	bool strafebraking = true;
	bool forwardbraking = true;
        if(Input.GetKey("w"))
	{
	    vTOL += m;
	    vertbraking = false;
	}
	if(Input.GetKey("s"))
	{
	    vTOL -= m;
	    vertbraking = false;
	}
	if(Input.GetKey(";"))
	{
	    strafe += m;
	    strafebraking = false;
	}
	if(Input.GetKey("k"))
	{
	    strafe -= m;
	    strafebraking = false;
	}
	if(Input.GetKey("o")) {
	    forward += m;
	    forwardbraking = false;
	}
	if(Input.GetKey("l"))
	{
	    forward -= m;
	    forwardbraking = false;
	}
        if(Input.GetKey("space"))
	{
	    vertbraking = false;
	    strafebraking = false;
	    forwardbraking = false;
	}

	float brake = (1.0f - (slewDamper * Time.deltaTime));
	if(vertbraking)
	{
	    vTOL *= brake;
	    if(vTOL * vTOL < minSpeedSqrt) vTOL = 0;
	}
	if (strafebraking)
	{
	    strafe *= brake;
	    if(strafe * strafe < minSpeedSqrt) strafe = 0;
	}
	if(forwardbraking)
	{
	    forward *= brake;
	    if(forward * forward < minSpeedSqrt) forward = 0;
	}	
	
	Vector2 keySlew = new Vector2(strafe, forward);
	
	Vector3 velocity = (transform.forward * keySlew.y + transform.right * keySlew.x) + transform.up * vTOL;

	controller.Move(velocity * Time.deltaTime);
    }

}
