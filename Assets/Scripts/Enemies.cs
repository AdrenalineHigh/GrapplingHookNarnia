using UnityEngine;
using System.Collections;

public class Enemies : MonoBehaviour {

	public GameObject Dragon;
	//public

	// Use this for initialization
	void Start() {
		for (int i = 0; i < 50; i++) {
			GameObject dragonInstance;
			dragonInstance = Instantiate(Dragon) as GameObject;
			
			dragonInstance.transform.position = Random.insideUnitSphere * 20000 + Vector3.up*12000;
			float size = (Mathf.Pow((Random.value*2.4F), 3) + 3) * 100;
			dragonInstance.transform.localScale += new Vector3(size,size,size);

			dragonInstance.GetComponent<DragonBrain>().speed = size*.001F;

			dragonInstance.transform.Rotate(Vector3.up, (Random.value*360F));
			
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
