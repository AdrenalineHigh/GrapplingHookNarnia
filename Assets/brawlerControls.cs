using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class brawlerControls : MonoBehaviour {

	CharacterController controller;
	Vector3 oldPosition;
	public Vector3 velocity;
	
	//public Top top;
	public GameObject PlayerHead;

	public BrawlerBrain brawlerInput;
	
	public Vector3 MouseDirection;
	public float rotationSpeed = 30.0F;
	public float runSpeed = 30F;
	
	public RaycastHit hitInfo;
	
	//public Vector2 step;
	//public Vector2 foothold;
	public Vector3[] foothold;
	
	public float delta;

	new bool notUsingUserInput = true; //DEBUG

	Dictionary<string,float> controls = new Dictionary<string,float>();

	// Use this for initialization
	void Start () {
		//controller = transform.Find ("Body").GetComponent<CharacterController>();
		controller = GetComponent<CharacterController>();
		velocity = Vector3.zero;
			velocity.y = 900.81F;
		
		oldPosition = transform.position;
		
		//PlayerHead = GameObject.Find("Player/Head");
		//foothold = Vector3.zero;
		foothold = new Vector3[2];

		MouseDirection.x = 280;

		controls.Add ("MouseDirectionx", 0F);
		controls.Add ("MouseDirectiony", 0F);
		controls.Add ("Vertical", 0F);
		controls.Add ("Horizontal", 0F);
		controls.Add ("Jump", 0F);

		delta = Time.deltaTime;
		
	}
	
	// Update is called once per frame
	void Update () {
		velocity = (transform.position - oldPosition)/delta;

		delta = Time.deltaTime;//top.deltaTime;
		
		//velocity = (transform.position - oldPosition)/top.deltaTime;

		
		velocity.y += -9.81F*delta;

		// GET USER/COMPUTER INPUT

		if (notUsingUserInput == false) {
						controls ["MouseDirectiony"] = -Input.GetAxis ("Mouse Y") * rotationSpeed;
						controls ["MouseDirectionx"] = Input.GetAxis ("Mouse X") * rotationSpeed;
						//controls["MouseDirectiony"] = new Vector3(-Input.GetAxis("Mouse Y") * rotationSpeed, 0, 0);
						//controls["MouseDirectionx"] = new Vector3(0, Input.GetAxis("Mouse X") * rotationSpeed, 0);

						controls ["Vertical"] = Input.GetAxis ("Vertical");
						controls ["Horizontal"] = Input.GetAxis ("Horizontal");
						controls ["Jump"] = Input.GetAxis ("Jump");
				} else {
			foreach(KeyValuePair<string, float> entry in brawlerInput.controls){
				controls[entry.Key] = entry.Value;
			}
		}
		MouseDirection += new Vector3 (controls ["MouseDirectiony"]*rotationSpeed, controls ["MouseDirectionx"]*rotationSpeed, 0);
		//0-90: Looking straight to down
		//360-275: Looking straight to up
		//For easier boundary logic:
		MouseDirection.x -= 90;
		//360-180: Looking straight down to straight up
			
		if (MouseDirection.x > 360)
			MouseDirection.x = 360;
		if (MouseDirection.x < 180)
			MouseDirection.x = 180;
			
		MouseDirection.x += 90;
		
		
		PlayerHead.transform.rotation = Quaternion.Euler(MouseDirection);

		//Running mechanic

		if (controls["Vertical"] == 1) {
			velocity += (PlayerHead.transform.TransformDirection(Vector3.forward)) * runSpeed*delta;
		} else if (controls["Vertical"] == -1) {
			velocity += (PlayerHead.transform.TransformDirection(-Vector3.forward)) * runSpeed*delta;
			}
		if (controls["Horizontal"] == 1) {
			velocity += (PlayerHead.transform.TransformDirection(Vector3.right)) * runSpeed*delta;
		} else if (controls["Horizontal"] == -1) {
			velocity += (PlayerHead.transform.TransformDirection(-Vector3.right)) * runSpeed*delta;
			}
			
		if (controls["Jump"] == 1 && controller.isGrounded) {
			velocity += Vector3.up*runSpeed*50*delta;//(PlayerHead.transform.TransformDirection(Vector3.up*2)) * 2;
			}

			/*
			//Detect firing at dragons/enemies. Not working yet
			int playerMask = 1 << 8;
			playerMask = ~playerMask;
			if (Physics.Raycast (transform.position, PlayerHead.transform.TransformDirection (Vector3.forward), out hitInfo, Mathf.Infinity, playerMask)) {
				Debug.Log ("Fire");			
				//if (hitInfo.Equals (GameObject.Find ("GameObject Dragon(Clone)"))) {
				if (hitInfo.collider.gameObject.name == "GameObject Dragon(Clone)") {
					Debug.Log ("HIT DRAGON");
				} else {
					Debug.Log ("pinged " + hitInfo.collider.gameObject.name);
				}
			}
			*/
		
		
		
		//Update
		Debug.DrawLine(transform.position, oldPosition, Color.green, 300, true);
		Vector3 deltaVelocity = velocity * delta;
		//Debug.Log ("velocity = " + velocity + ". delta = " + delta + ". velocity*delta = " + deltaVelocity);
		/*
		if (float.IsNaN (deltaVelocity.x) || float.IsNaN (deltaVelocity.y) || float.IsNaN (deltaVelocity.z))
		{
			deltaVelocity = velocity * Time.deltaTime;
			Debug.Log ("BAD TIMEDELTA!");
		}
		*/

		oldPosition = transform.position;
		controller.Move(deltaVelocity);	// Using char controller collisions

		// Trying to control editor camera view
		//SceneView.lastActiveSceneView.LookAt(SceneView.lastActiveSceneView.pivot, Quaternion.LookRotation(Vector3.forward));
		//SceneView.lastActiveSceneView.pivot = new Vector3( 0, 0, 0 ); SceneView.lastActiveSceneView.rotation = Quaternion.Euler( new Vector3( 0, 0, 0 ) ); SceneView.lastActiveSceneView.size = 20; SceneView.lastActiveSceneView.Repaint();
		//Camera.current.transform.rotation = new Quaternion (0, 0, 0,0);
	}

	void OnGUI(){
		GUI.Label(new Rect(10,10,Screen.width,Screen.height),"velocity = " + velocity + ". delta = " + delta);

		//Debug.Log ("velocity = " + velocity + ". delta = " + delta + ". velocity*delta = " + deltaVelocity);
	}
}
