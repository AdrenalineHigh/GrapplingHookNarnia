using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[SerializeField]
public class ConnectionEdit : MonoBehaviour {

	public BeamHandlerScript BeamHandler;

	// Use this for initialization
	void OnEnable () {
		//BeamHandlerScript.ConnectionInstances.Add (this);
	}
	
	// Update is called once per frame
	void Update () {
		if (!BeamHandler.connectionInstances.Contains(this)) BeamHandler.connectionInstances.Add (this);
	}
}
