using UnityEngine;
using System.Collections;

public class DragonBrain : MonoBehaviour {

	private GameObject player;
	public float speed;
	private Vector3 oldPosition;

	// Use this for initialization
	void Start () {
		player = GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update () {


		//CharacterController controller = GetComponent<CharacterController>();
		
		//if(controller.isGrounded) velocity *= .5F;
		//velocity = transform.position - oldPosition;

		transform.LookAt(player.transform);
		transform.Translate(Vector3.forward*(Vector3.Distance(transform.position, player.transform.position)/3)*speed * Time.deltaTime);

		if (oldPosition != new Vector3(0,0,0))
			Debug.DrawLine(transform.position, oldPosition, Color.blue, 10, true);
		oldPosition = transform.position;

	}
}
