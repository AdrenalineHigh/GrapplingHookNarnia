using UnityEngine;
using System.Collections;

public class Root : MonoBehaviour 
{
	void Start()
	{
		DX11ParticleController.Init();
	}
	
	void FixedUpdate()
	{
		DX11ParticleController.ManualUpdate();
	}
	
	void OnRenderObject()
	{
		DX11ParticleController.RenderAllParticles();
	}
	
	void OnDestroy()
	{
		DX11ParticleController.OnManualDestroy();	
	}
	
}
