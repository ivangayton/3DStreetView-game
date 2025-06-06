using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Transform playerCamera = null;
    
    [SerializeField] float mouseSensitivity = 3.0f;
    [SerializeField] float keyRotSensitivity = 1.0f;
    [SerializeField] float walkSpeed = 16.0f;
    [SerializeField] float vertSpeed = 16.0f;
    
    [SerializeField] bool lockCursor = true;
    [SerializeField] bool invertedY = false;
    
    float cameraPitch = 25.0f;
    
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
    }
    
    void UpdateRotation()
    {
    	float yaw = 0.0f;
	if(Input.GetKey("d")) yaw = keyRotSensitivity;
	if(Input.GetKey("a")) yaw = -keyRotSensitivity;
	yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
	transform.Rotate(Vector3.up * yaw);
	
	if(Input.GetKey("q")) cameraPitch -= keyRotSensitivity;
	if(Input.GetKey("e")) cameraPitch += keyRotSensitivity;
	cameraPitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
	cameraPitch = Mathf.Clamp(cameraPitch, -85.0f, 85.0f);
	playerCamera.localEulerAngles = Vector3.right * cameraPitch;	
    }
    
    void UpdateMovement()
    {
        float vTOL = 0.0f;
        if(Input.GetKey("w")) vTOL = 1;
	if(Input.GetKey("s")) vTOL = -1;
	float strafe = 0.0f;
	if(Input.GetKey(";")) strafe = 1;
	if(Input.GetKey("k")) strafe = -1;
	float forward = 0.0f;
	if(Input.GetKey("o")) forward = 1;
	if(Input.GetKey("l")) forward = -1;
	Vector2 keySlew = new Vector2(strafe, forward);
	keySlew.Normalize();
	
	Vector3 velocity = (transform.forward * keySlew.y + transform.right * keySlew.x) * walkSpeed + transform.up * vTOL * vertSpeed;
	
	controller.Move(velocity * Time.deltaTime);
    }

}
