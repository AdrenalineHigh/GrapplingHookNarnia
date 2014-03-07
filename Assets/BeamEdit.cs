using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[SerializeField]
public class BeamEdit : MonoBehaviour {
	
	public BeamHandlerScript BeamHandler;
	

	void onEnable () {
		//BeamHandler.beamInstances.Add (this);
	}

	void Start () {
		//BeamHandler.beamInstances.Add (this);

	}
	
	// Update is called once per frame
	void Update () {
		if (!BeamHandler.beamInstances.Contains(this)) BeamHandler.beamInstances.Add (this);
	}
}
