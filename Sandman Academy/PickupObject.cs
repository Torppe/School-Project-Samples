using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickupObject : MonoBehaviour {

	private KeyCode rotate {get; set;}

    public float currentDistance = 2f;
	public float minDistance = 1.75f;
    public float maxDistance = 6f;
	public int throwForce = 750;
    //TM Smoothing of the floating object
    public float carryingSmoothing = 10f;
    public bool carrying = false;
	private bool pickupAble;
	public Image handIconPos;
	public Image reticle;
	public Sprite pickupIcon;
	public Sprite pickedupIcon;

	GameObject playerOnObj;

    Vector3 relativeRotation;
	Vector3 referenceValue1;
	Vector3 referenceValue2;

    GameObject playerCamera;
    GameObject carriedObject;
    Rigidbody carriedObjectRb;

    RaycastHit hitInfo;
    Ray ray;

	float mouseXInput;
	float mouseYInput;
	float mouseSens = 5f;

	public GameObject holdingPoint;

	void Start () {
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera");
		rotate = (KeyCode)System.Enum.Parse (typeof(KeyCode), PlayerPrefs.GetString ("Rotate", "R"));

    }

	void Update () {
		//TM check to see if the player is already carrying an object, so the same keycode can be used to pickup/drop the carried object
		if (carrying) {
			if (Vector3.Distance (transform.position, carriedObject.transform.position) < 1.2f) {
				DropObj ();
			}
			if (Input.GetAxis("Mouse ScrollWheel") != 0) {
				UseScroll ();
			}
			if (Input.GetMouseButtonDown(0)) {
				DropObj ();
				currentDistance = 1.75f;
			}
			
		} else if (Input.GetMouseButtonDown(0) && pickupAble) {
			if (hitInfo.collider.gameObject != playerOnObj && hitInfo.collider.tag != "Interactable") {
				Pickup ();
			}
		}
		if (Input.GetKey (rotate) && carrying) {
			TurnObject ();
		}
		//MP Throwing on object carrying
		if (carrying && Input.GetMouseButtonDown (1)) {
			carriedObjectRb.AddForce(playerCamera.transform.forward * throwForce);
			DropObj ();
		}
    }

	void FixedUpdate () {
        //TM Casts a ray from the player's camera by the forward axis
        ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out hitInfo, maxDistance))
        {
            if(hitInfo.collider.tag == "PickUp")
            {
                Interact(hitInfo.collider);
                pickupAble = true;
            }
        }
        else if (pickupAble)
        {
            UnFocus();
        }

        if (carrying)
        {
            Carry(carriedObject);
        }
    }

	private void UnFocus () {
		if (!carrying) {
			pickupAble = false;
			handIconPos.sprite = pickupIcon;
			handIconPos.gameObject.SetActive (false);
			reticle.gameObject.SetActive (true);
		}
	}

	private void Interact (Collider otherObj) {
		//TM Whenever the raycast detects that it's in the reach of a pickupable object, A key can be used to pick it up
		handIconPos.gameObject.SetActive (true);
		reticle.gameObject.SetActive (false);
		if (!carrying) {
			handIconPos.sprite = pickupIcon;
		}
	}
	void Pickup() {
		//TM add a bunch of stuff from the detected object into variables
		carriedObject = hitInfo.collider.gameObject;
		carriedObjectRb = hitInfo.rigidbody;
		carriedObjectRb.useGravity = false;

		holdingPoint.transform.rotation = carriedObject.transform.rotation;
		holdingPoint.transform.position = new Vector3 (carriedObject.transform.position.x, carriedObject.transform.position.y, carriedObject.transform.position.z);
		handIconPos.sprite = pickedupIcon;
		handIconPos.gameObject.SetActive (true);
		reticle.gameObject.SetActive (false);
		if (!carrying) {
			carrying = true;
		}
    }
		
    void Carry(GameObject o) {
		o.transform.position = Vector3.Lerp(carriedObject.transform.position, holdingPoint.transform.position, Time.deltaTime * carryingSmoothing);
		o.transform.rotation = Quaternion.Lerp(carriedObject.transform.rotation, holdingPoint.transform.rotation, Time.deltaTime * carryingSmoothing);
		//MP Prevent object having velocity when carried
		carriedObjectRb.velocity = new Vector3(0,0,0);
    }

	//MP Turning carried object depending the mouse movement
	void TurnObject () {
        mouseXInput = Input.GetAxisRaw("Mouse X") * mouseSens;
        mouseYInput = Input.GetAxisRaw("Mouse Y") * mouseSens;
        //TM rotate the pickable object relative to player camera's axis
        holdingPoint.transform.RotateAround(holdingPoint.transform.position, playerCamera.transform.up, -mouseXInput);
        holdingPoint.transform.RotateAround(holdingPoint.transform.position, playerCamera.transform.right, mouseYInput);
    }

	public void DropObj () {
		carrying = false;
		carriedObjectRb.useGravity = true;
		pickupAble = true;
		handIconPos.sprite = pickupIcon;
		handIconPos.gameObject.SetActive (true);
		reticle.gameObject.SetActive (false);
	}

	private void UseScroll () {
		//MP Zoom in or out object carrying. Prevent object going too far or too close
		if (Input.GetAxis("Mouse ScrollWheel") > 0) {
			currentDistance += 0.075f;
			if (currentDistance > maxDistance) {
				currentDistance = maxDistance;
			}
			holdingPoint.transform.localPosition = new Vector3 (holdingPoint.transform.localPosition.x, holdingPoint.transform.localPosition.y, currentDistance);
		} else if (Input.GetAxis("Mouse ScrollWheel") < 0 ) {
			currentDistance -= 0.075f;
			if (currentDistance < minDistance) {
				currentDistance = minDistance;
			}
			holdingPoint.transform.localPosition = new Vector3 (holdingPoint.transform.localPosition.x, holdingPoint.transform.localPosition.y, currentDistance);
		}
	}
	public void OnTriggerEnter (Collider collObj) {
		if (collObj.gameObject.tag == "PickUp") {
			playerOnObj = collObj.gameObject;
			if (carrying && carriedObject == playerOnObj) {
				DropObj ();
			}
		}
	}

	public void OnTriggerExit (Collider collObj) {
		if (collObj.gameObject.tag == "PickUp") {
			playerOnObj = null;
		}
	}
}