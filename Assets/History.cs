using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class History : MonoBehaviour {

	//Dictionary<string, Dictionary<string, int>> gameobjects = new Dictionary<string, Dictionary<string, int>>();
	// This collection is a horrific yet awesome thing. I wonder how it will perform 0_o
	Dictionary<string, Dictionary<string, Dictionary<int, object>>> gameobjects = new Dictionary<string, Dictionary<string, Dictionary<int, object>>>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
