using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour {

	private KeyCode rotate {get; set;}
	private KeyCode run {get; set;}
	private KeyCode crouch {get; set;}
	private KeyCode journal {get; set;}

    Camera playerCamera;

    public Rigidbody playerRigidbody;

    public float mouseXInput;
	public float mouseYInput;

    public float movementSpeed = 5f;
    public float mouseSens = 5f;
    public float cameraMin = -90;
    public float cameraMax = 90;

	private bool crouching = false;
	private bool dragging = false;

	public GameObject journalObj;
	public GameObject menuObj;
    
    Vector3 movementInput;
    Vector3 movementDir;
    Vector3 velocity;
    Vector3 playerCameraAngle;
	Vector3 rotationVector;

    private JumpDetector jump;

    private void Start() {
		rotate = (KeyCode)System.Enum.Parse (typeof(KeyCode), PlayerPrefs.GetString ("Rotate", "R"));
		run = (KeyCode)System.Enum.Parse (typeof(KeyCode), PlayerPrefs.GetString ("Run", "LeftShift"));
		crouch = (KeyCode)System.Enum.Parse (typeof(KeyCode), PlayerPrefs.GetString ("Crouch", "LeftControl"));
		journal = (KeyCode)System.Enum.Parse (typeof(KeyCode), PlayerPrefs.GetString ("Journal", "J"));

        playerRigidbody = gameObject.GetComponent<Rigidbody>();
        playerCamera = GetComponentInChildren<Camera>();
		jump = GetComponentInChildren<JumpDetector> ();

		playerCamera.fieldOfView = PlayerPrefs.GetInt ("FOV", 75);
    }

	private void Update()
	{
		if (!journalObj.activeSelf && !menuObj.activeSelf) {
			if (!Input.GetKey (rotate) && !dragging) {
				//TM Adds mouse's input values to variables
				mouseXInput += Input.GetAxisRaw("Mouse X") * mouseSens;
				mouseYInput += Input.GetAxisRaw("Mouse Y") * mouseSens;
				//TM Clamps the vertical rotation of the camera to prevent player's vision from rotating too much
				mouseYInput = Mathf.Clamp(mouseYInput, cameraMin, cameraMax);
			}

			//TM Movement input
			movementInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
			movementDir = movementInput.normalized;
			rotationVector = new Vector3(transform.localEulerAngles.x, mouseXInput, transform.localEulerAngles.z);
		}
		//TM Crouch and sneak inputs
		if (Input.GetKeyDown(crouch))
		{
            if (crouching)
            {
                StartCoroutine(Crouch(0.2f, 0.5f));
                crouching = false;
            }
            else
            {
                StartCoroutine(Crouch(0.2f, -0.5f));
                crouching = true;
            }
        }


		if (crouching) {
			velocity = movementDir * movementSpeed * Time.fixedUnscaledDeltaTime * 0.25f;
		} else if (dragging) {
			velocity = movementDir * movementSpeed * Time.fixedUnscaledDeltaTime * 0.2f;
		} else {
			velocity = movementDir * movementSpeed * Time.fixedUnscaledDeltaTime;
		}
		if (Input.GetKey(run) && !crouching) {
			velocity = velocity * 1.3f;
		}
		if (!jump.GetJumpable ()) {
			Physics.gravity += new Vector3(0, -Time.fixedUnscaledDeltaTime * 10, 0);
		} else if (Physics.gravity.y != -9.81f){
			Physics.gravity = new Vector3(0, -9.81f, 0);
		}
	}

	private void FixedUpdate() {
		if (Input.GetKey (KeyCode.B)) {
			transform.Translate (Vector3.up);
		}
        PlayerMove();
		MouseLook();
	}
    private void MouseLook() {
		playerRigidbody.MoveRotation(Quaternion.Euler(rotationVector));
		playerCamera.transform.localEulerAngles = new Vector3(-mouseYInput, playerCamera.transform.localEulerAngles.y, playerCamera.transform.localEulerAngles.z);
	}
    private void PlayerMove() {
		playerRigidbody.MovePosition(playerRigidbody.position + playerRigidbody.transform.TransformDirection(velocity));
    }

    private IEnumerator Crouch(float time, float crouchAmount)
    {        
        Vector3 originalScale = transform.localScale;
        Vector3 destinationScale = new Vector3(1, transform.localScale.y + crouchAmount, 1);

        float currentTime = 0.0f;

        while (currentTime <= time)
        {
			currentTime += 1f * Time.fixedUnscaledDeltaTime;

            transform.localScale = Vector3.Lerp(originalScale, destinationScale, currentTime / time);
            yield return null;
        } 
    }

    public Vector3 GetVelocity () {
		return velocity;
	}

	public void SetDragging () {
		dragging = !dragging;
	}

	public bool GetDragging () {
		return dragging;
	}
}