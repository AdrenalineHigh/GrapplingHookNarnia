using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[SerializeField]
public class BeamHandlerScript : MonoBehaviour {
	
	public List<BeamEdit> beamInstances;
	public List<ConnectionEdit> connectionInstances;


	void OnEnable () {
		if (beamInstances == null)
			beamInstances = new List<BeamEdit> ();

		if (connectionInstances == null)
			connectionInstances = new List<ConnectionEdit> ();

		//hideFlags = HideFlags.HideAndDontSave;		//Screws up the inspector field between runs
	}

	
	// Update is called once per frame
	void Update () {

	}

	void OnGUI(){
		GUI.Label (new Rect (0, 0, Screen.width, Screen.height), "Test Text block\n");

		int i = 0;
		foreach (BeamEdit beam in beamInstances){
			i += 1;
			//GUI.Label (new Rect (0, i*12, Screen.width, Screen.height), "List has:\n" + i +"  =  ");
			GUI.Label (new Rect (0, i*12, Screen.width, Screen.height), "List has:\n" + beamInstances.Count);
		}

		i = 0;
		foreach (ConnectionEdit beam in connectionInstances){
			i += 1;
			//GUI.Label (new Rect (0, i*12, Screen.width, Screen.height), "List has:\n" + i +"  =  ");
			GUI.Label (new Rect (0, i*12, Screen.width, Screen.height), "List has:\n" + connectionInstances.Count);
		}
	}
}
