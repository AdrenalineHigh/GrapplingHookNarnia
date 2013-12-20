using UnityEngine;
using System.Collections;


//[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Fracture : MonoBehaviour {
	
	private GameObject shard;
	
	// Use this for initialization
	void Start () {
		
		//TODO: Get gameObject LittleCity1 and copy its transforms to the shard
		
		Mesh mesh = GameObject.Find("LittleCity1").GetComponent<MeshFilter>().mesh;
		//Mesh mesh = GetComponent<MeshFilter>().mesh;
		
		
		//Make new game object for mesh fragment
		GameObject newGO = new GameObject( "shard1" );
		
		//Instantiate(newGO, transform.position, transform.rotation);	// Set to it's parent's coordinates.
		//newGO.transform.parent = transform;
		
        MeshFilter meshFilter = newGO.AddComponent<MeshFilter>();
		MeshRenderer meshRenderer = newGO.AddComponent<MeshRenderer>();
		
		//Create the new 'shard' mesh from the old mesh
		//meshFilter.mesh = MeshCreationHelper.CreateMesh(mesh);	//was causing errors; is it important?
		
		
		newGO.renderer.material.mainTexture = Resources.Load("LittleCity1") as Texture;;
		
		shard = newGO;//GameObject.Find ("shard1");

		shard.transform.parent = GameObject.Find("top").transform;//parentObject.transform 
		//shard.transform.Rotate(270, 0, 0); // Default
		shard.transform.Rotate(220, 0, 0);
		shard.transform.position = GameObject.Find ("LittleCity1").transform.position + Vector3.up*10;
		shard.transform.localScale = GameObject.Find ("LittleCity1").transform.localScale;
		
	}
	
	// Update is called once per frame
	void Update () {
		
		//shard.transform.Translate(Vector3.up * Time.deltaTime, Space.World);
		//shard.transform.Rotate(0, 100 * Time.deltaTime, 0);

		/*
		Mesh mesh1 = shard.GetComponent<MeshFilter>().mesh;
		print("Shard1 New mesh vertices: " + mesh1.vertices.Length);
		
		print(shard.transform.position.x);
		print(shard.transform.position.y);
		print(shard.transform.position.z);
		*/
	}
}
