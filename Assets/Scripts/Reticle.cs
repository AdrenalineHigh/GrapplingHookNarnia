using UnityEngine;
using System.Collections;

public class Reticle : MonoBehaviour {
	
	public Texture2D crosshairImage;
	
	// Use this for initialization
	void Start () {
		Screen.lockCursor = true;
	}
	
	// Update is called once per frame
	void Update() {
		if (Input.GetAxis("Fire2") == 1 || Input.GetAxis("Fire1") == 1)
			Screen.lockCursor = true;

		if (Input.GetKeyDown("escape"))
			Screen.lockCursor = false;
		
		if (!Screen.lockCursor && wasLocked) {
			wasLocked = false;
			DidUnlockCursor();
		} else
		if (Screen.lockCursor && !wasLocked) {
			wasLocked = true;
			DidLockCursor();
		}
	}
	
	void DidLockCursor() {
		Debug.Log("Locking cursor");
		//guiTexture.enabled = false;	//For a pause menu
	}
	void DidUnlockCursor() {
		Debug.Log("Unlocking cursor");
		//guiTexture.enabled = true;
	}
	void OnMouseDown() {	//Default; Doesn't work
		Screen.lockCursor = true;
	}
	private bool wasLocked = false;

	
	void OnGUI()
	{
	    float xMin = (Screen.width / 2) - (crosshairImage.width / 2);
	    float yMin = (Screen.height / 2) - (crosshairImage.height / 2);
	    GUI.DrawTexture(new Rect(xMin, yMin, crosshairImage.width, crosshairImage.height), crosshairImage);
	}
}
	
