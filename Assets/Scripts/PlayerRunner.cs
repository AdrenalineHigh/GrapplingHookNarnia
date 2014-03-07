using UnityEngine;
using System.Collections;

public class PlayerRunner : MonoBehaviour {

	CharacterController controller;
	Vector3 oldPosition;
	public Vector3 velocity;

	public Top top;
	public GameObject PlayerHead;


	public Vector3 mouseDirection;
	public float rotationSpeed = 30.0F;


	public RaycastHit hitInfo;

	public Vector2 step;
	//public Vector2 foothold;
	public Vector3[] foothold;

	public float delta;

	// Use this for initialization
	void Start () {
		//controller = transform.Find ("Body").GetComponent<CharacterController>();
		controller = GetComponent<CharacterController>();
		velocity = Vector3.zero;
		oldPosition = transform.position;

		PlayerHead = GameObject.Find("Player/Head");
		step = Vector2.zero;
		//foothold = Vector3.zero;
		foothold = new Vector3[2];

		mouseDirection.x = 280;

	}
	
	// Update is called once per frame
	void Update () {
		delta = top.deltaTime;

		//velocity = (transform.position - oldPosition)/top.deltaTime;
		velocity = (transform.position - oldPosition)/delta;

		velocity.y += -9.81F;

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
		
		PlayerHead.transform.rotation = Quaternion.Euler(mouseDirection);
		
		
		//Running mechanic
		if (Input.GetAxis("Vertical") == 1)
	    {
			for (int foot = 0; foot < 2; foot++)
			{
				//STEP:
				// 0-.75: Push against ground
				// .76-.99: move foot
				// 1-0: find new foothold

				//Find foothold
				if (step[foot] == 0F)
				{
					//Cast ray down and backward
					if (Physics.Raycast(transform.position + Vector3.up*.5F, transform.TransformDirection(Vector3.down + Vector3.back), out hitInfo))
					{
						//Debug.DrawLine(transform.position, hitInfo.point, Color.red, 300, true);
						if (hitInfo.distance < 2)
						{
							//tether.distance = (transform.position - hitInfo.point).magnitude;

							foothold[foot] = hitInfo.point;
							//Debug.Log ("foothold fine at" + (foothold[foot] - transform.position).sqrMagnitude);
							Debug.DrawLine(transform.position, foothold[foot], Color.blue, 300, true);
							//velocity += (Vector3.forward + Vector3.up*2) * 2;
						}
					}
				}

				//Push against foothold
				if (step[foot] <= 0.75F && foothold[foot] != Vector3.zero)
				{
					if ((foothold[foot] - transform.position).sqrMagnitude < 2)
					{
						//Debug.DrawLine(transform.position, foothold[foot], Color.blue, 300, true);
						velocity += (PlayerHead.transform.TransformDirection(Vector3.forward + Vector3.up*5)) * 2;
					} else {
						//Debug.Log ("foothold too far at" + (foothold[foot] - transform.position).sqrMagnitude);
						foothold[foot] = Vector3.zero;
						//Debug.DrawLine(transform.position, foothold[foot], Color.red, 3F, true);
					}
				}

				//Increment step cycle
				//TODO: Change to velocit relative to surface
				float stepPercent = (.2F + velocity.magnitude);
				if (stepPercent > .75) stepPercent = .75F;
				step[foot] += stepPercent;

				if(foothold[foot] == Vector3.zero) step[foot] = 0;

				//Offset footsteps
				if (step[0] > step[1] -.25F && step[0] < step[1] + .25)
				{
					foothold[0] = Vector3.zero;
					step[0] = step[1] + .5F;
					if (step[0] > 1) step[0] -= 1;
				}

				//Pushing off against ground; start of running or end of step?
				if (step[foot] >= 1F) step[foot] = 0F;
				
			}

		}
		
		
		//Update
		Debug.DrawLine(transform.position, oldPosition, Color.green, 300, true);
		oldPosition = transform.position;
		Vector3 deltaVelocity = velocity * delta;
		Debug.Log ("velocity = " + velocity + ". delta = " + delta + ". velocity*delta = " + deltaVelocity);
		if (float.IsNaN (deltaVelocity.x) || float.IsNaN (deltaVelocity.y) || float.IsNaN (deltaVelocity.z))
		{
						deltaVelocity = velocity * Time.deltaTime;
			Debug.Log ("BAD TIMEDELTA!");
				}
		controller.Move(deltaVelocity);	// Using char controller collisions
	}
}
