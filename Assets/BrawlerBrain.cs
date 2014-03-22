using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BrawlerBrain : MonoBehaviour {


	public bool notUsingUserInput = false; //DEBUG
	public Dictionary<string,float> controls = new Dictionary<string,float>();


	// Use this for initialization
	void Start () {
	
		controls.Add ("MouseDirectionx", 0F);
		controls.Add ("MouseDirectiony", 0F);
		controls.Add ("Vertical", 0F);
		controls.Add ("Horizontal", 0F);
		controls.Add ("Jump", 0F);
	}
	
	// Update is called once per frame
	void Update () {

		Sphere[] sceneSpheres  = FindObjectsOfType (typeof(Sphere)) as Sphere;
		for (int i=0;i<sceneSpheres.Length;i++) {
			//myGates[i].OpenGate ();
			Debug.Log (sceneSpheres[i].transform.name);
		}
				

		if (notUsingUserInput == false) {
						controls ["MouseDirectiony"] = -Input.GetAxis ("Mouse Y");
						controls ["MouseDirectionx"] = Input.GetAxis ("Mouse X");
						//controls["MouseDirectiony"] = new Vector3(-Input.GetAxis("Mouse Y") * rotationSpeed, 0, 0);
						//controls["MouseDirectionx"] = new Vector3(0, Input.GetAxis("Mouse X") * rotationSpeed, 0);
			
						controls ["Vertical"] = Input.GetAxis ("Vertical");
						controls ["Horizontal"] = Input.GetAxis ("Horizontal");
						controls ["Jump"] = Input.GetAxis ("Jump");
				} else {

		}


	}
}
