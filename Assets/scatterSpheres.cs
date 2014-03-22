using UnityEngine;
using System.Collections;

public class scatterSpheres : MonoBehaviour {

	public GameObject sphereInstance;
	public GameObject spawnsHolder;
	
	// Use this for initialization
	void Start() {

		for (int i = 0; i < 2000; i++) {
			GameObject chunk;
			chunk = Instantiate(sphereInstance) as GameObject;
			chunk.transform.parent = spawnsHolder.transform;

			chunk.transform.position = Random.insideUnitSphere * 1500 - Vector3.up*120;
			float size = Mathf.Pow((Random.value*2F), 9) + 40;
			chunk.transform.localScale += new Vector3(size,size,size);

			if (chunk.transform.position.sqrMagnitude < chunk.transform.localScale.sqrMagnitude + 50) Destroy(chunk);

			//chunk.transform.Rotate(Vector3.up, (Random.value*360F));
			
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
