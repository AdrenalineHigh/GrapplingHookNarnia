using UnityEngine;
using System.Collections;

public class CTether {
	
	public bool tethered;
	public Vector3 point;
	public float distance;
	public float retractSpeed;
	
	public CTether()
	{
		tethered = false;
		point = new Vector3();
		distance = 0;
	}
	
}

public class player1 : MonoBehaviour {
	
	public float speed;// = 2.0F;
	public float trueGravity = 9.81F;
	public float gravity;
    public float rotationSpeed = 30.0F;
	
	public Vector3 velocity;
	public Vector3 acceleration;
	
	public float mouseXCalibrate;
	public float mouseYCalibrate;
	
	public RaycastHit hitInfo;
	
	public Vector3 testPosition;
	
	public Vector3 tempVector;
	
	public Vector3 mouseDirection;
	Quaternion headOrientation;
	Quaternion bodyOrientation;
	
	public Vector3 oldPosition;
	
	CTether tether = new CTether();
	
	// Use this for initialization
	void Start () {
		mouseXCalibrate = Input.GetAxis("Mouse X");
		mouseYCalibrate = Input.GetAxis("Mouse Y");
		
		Screen.lockCursor = true;
	}
	
	
    void Update() {
		
		CharacterController controller = GetComponent<CharacterController>();
		
		if(controller.isGrounded) velocity *= .5F;
		velocity = transform.position - oldPosition;
		
		speed = velocity.magnitude*0.4F + 2;
		
		Debug.DrawLine(transform.position, oldPosition, Color.green, 30, true);

		
		if (!tether.tethered && Input.GetAxis("Fire2") == 1) {
			//if (Physics.Raycast(transform.position, -transform.rotation.eulerAngles, out hitInfo)) {
			if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitInfo)) {
				tether.tethered = true;
				tether.point = hitInfo.point;
				//tether.distance = hitInfo.distance;
				tether.distance = (transform.position - hitInfo.point).magnitude;
			}
		}
		
		//Debug.Log("Tether: ");
		//Debug.Log(tether.tethered);
		//Debug.Log(tether.point);
		
		// Player controlled momentum
		/*
		if (Input.GetAxis("Jump") == 1)
		{
			gravity = -trueGravity;
		} else {
			gravity = trueGravity;
		}
		*/
		gravity = trueGravity;
		
		
		acceleration = new Vector3(Input.GetAxis("Horizontal") * speed, 0, Input.GetAxis("Vertical") * speed);
		acceleration *= Time.deltaTime;
		velocity += transform.TransformDirection(acceleration) + Vector3.down*gravity*Time.deltaTime;	// Apply local acceleration and world gravity
		//velocity *= Time.deltaTime;
		//Debug.Log(velocity);
		
		//if (tether.tethered && (Input.GetAxis("Fire1") == 1 || Input.GetAxis("Fire3") == 1) && tether.distance > 1) tether.distance = Mathf.Lerp(tether.distance, tether.distance*0.7F, Time.deltaTime);
		//if (tether.tethered && (Input.GetAxis("Fire1") == 1 || Input.GetAxis("Fire3") == 1) && tether.distance > 1) tether.distance = Mathf.Lerp(tether.distance, tether.distance*0.9F - 15, Time.deltaTime);
		//if (tether.tethered && (Input.GetAxis("Fire1") == 1 || Input.GetAxis("Fire3") == 1) && tether.distance > 1) 
		if (tether.tethered && Input.GetAxis("Jump") == 1 && tether.distance > 1) 
		{
			tether.retractSpeed += Mathf.Lerp(0, tether.distance*0.0000001F + 2, Time.deltaTime);
			tether.distance -= tether.retractSpeed + 5;
		} else {
			tether.retractSpeed = 0;
		}
		
		bodyOrientation = Quaternion.identity;
		if (tether.tethered && Input.GetAxis("Fire2") == 1)
		{
			Debug.DrawLine(tether.point, transform.position, Color.red, 30, true);
			Debug.DrawLine(tether.point, transform.position, Color.magenta, 1, true);
				
			testPosition = transform.position + velocity;
			if ((testPosition - tether.point).magnitude > tether.distance) {
				// Off end of rope, pull player in
				
				//testPosition = testPosition - tether.point;
				
				
				testPosition = ((testPosition - tether.point)).normalized * tether.distance + tether.point;
				//testPosition = Vector3.Normalize(testPosition) * tether.distance;
				
				tempVector = (tether.point - testPosition);//.normalized;
				
				//Debug.Log("Angle from to: " + tempVector);
				//Debug.Log("Angles: " + tempVector.x + tempVector.y + tempVector.z);
				
				//Debug.Log("Angle Z Y: " + Mathf.Atan2(tempVector.z, tempVector.y) * Mathf.Rad2Deg);
				//Debug.Log("Angle X Y: " + Mathf.Atan2(tempVector.x, tempVector.y) * Mathf.Rad2Deg);

				
				bodyOrientation = Quaternion.FromToRotation(Vector3.up, tempVector);
				
				
				//transform.position = testPosition;
				velocity = testPosition - transform.position;
			} else {
			
				tether.distance = (testPosition - tether.point).magnitude;
				
			}
			
			//transform.position = tether.point;	// Warp to point. Fun =D
			
		} else if (tether.tethered) {
			tether.tethered = false;
		}
		
		oldPosition = transform.position;
		
		//transform.Translate(velocity);		// Relative to local rotation
		//transform.position = transform.position + velocity;		// Relative to world

		controller.Move(velocity);	// Using char controller collisions
		
		
		
		mouseDirection = new Vector3(-Input.GetAxis("Mouse Y") * rotationSpeed, 0, 0) + mouseDirection;
        mouseDirection = new Vector3(0, Input.GetAxis("Mouse X") * rotationSpeed, 0) + mouseDirection;


		//0-90: Looking straight to down
		//360-275: Looking straight to up
		//For easier boundary logic:
		mouseDirection.x -= 90;
		//360-180: Looking straight down to straight up

		if (mouseDirection.x > 360)
			mouseDirection.x = 360;
		if (mouseDirection.x < 180)
			mouseDirection.x = 180;

		mouseDirection.x += 90;

        //mouseX *= Time.deltaTime;
		//mouseY *= Time.deltaTime;
		//transform.Rotate(-mouseY, mouseX, 0);
		
		headOrientation = Quaternion.Euler(mouseDirection);
		
		transform.rotation = headOrientation;//bodyOrientation*headOrientation;//*bodyOrientation;
    }
}
