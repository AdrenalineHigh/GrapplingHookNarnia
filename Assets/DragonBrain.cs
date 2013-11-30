using UnityEngine;
using System.Collections;

public class DragonBrain : MonoBehaviour {

	private GameObject player;

	// Use this for initialization
	void Start () {
		player = GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update () {
		transform.LookAt(player.transform);
		transform.Translate(Vector3.forward*500 * Time.deltaTime);
	}
}
