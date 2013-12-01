using UnityEngine;
using System.Collections;


public class manyChunks : MonoBehaviour {
 
 	public GameObject GroundChunk1;
	
	// Use this for initialization
	void Start() {
		for (int i = 0; i < 2000; i++) {
			GameObject chunk;
			chunk = Instantiate(GroundChunk1) as GameObject;

			chunk.transform.position = Random.insideUnitSphere * 20000 + Vector3.up*12000;
			float size = Mathf.Pow((Random.value*2.4F), 7) + 8;
			chunk.transform.localScale += new Vector3(size,size,size);
			
			chunk.transform.Rotate(Vector3.up, (Random.value*360F));
			
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
