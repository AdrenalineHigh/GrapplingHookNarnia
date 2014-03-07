using UnityEngine;
using System.Collections;

public class Top : MonoBehaviour {

	public float deltaTime;
	public PlayerRunner player;
	public float newdelta;

	// Use this for initialization
	void Start () {
		deltaTime = Time.deltaTime;
		player = GameObject.Find ("Player").GetComponent<PlayerRunner>();
	}
	
	// Update is called once per frame
	void Update () {
	
		newdelta = Time.deltaTime / (((player.velocity.magnitude + 1)* .5F));
		/*
		if (!float.IsNaN (newdelta))
		{
			deltaTime = newdelta;
			Debug.Log ("proper delta");
		} else {
			deltaTime = Time.deltaTime;
			Debug.Log ("BAD delta");
		}
		*/
		deltaTime = newdelta + .001F;//.111111111111111111111111111111F;
		//deltaTime = 1;

		//Debug.Log ("player velocity ref = " + Time.deltaTime / (player.velocity.magnitude *.8F + 1F));
	}
}
