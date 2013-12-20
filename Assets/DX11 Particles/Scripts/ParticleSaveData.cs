/// <summary>
/// Particle save data.
/// 
/// Author		: 	Dean Stanfield
/// Date  		:		18/02/2013
/// 
/// Description	:
/// This is the layout of save data for a DX11 Particle System that can be loaded and saved to XML
/// </summary>
using UnityEngine;
using System.Collections;

/// <summary>
/// Particle save data.
/// 
/// This class defines the save structure for a DX11ParticleSystem.
/// Any new save data should be added into this class and mimic'ed inside DX11ParticleSystem class
/// </summary>
public class ParticleSaveData
{
	//System Data
	public int numParticles = 1000;
	public string kernelName = "CSParticle";
	public string CSName = "CSParticle";
	public string materialName = "Draw Particles 1";
	public string spriteName = "smokepuff1";
	
	//Shader data
	public float size = 5.0f;
	public Color color = new Color(1.0f, 0.5f, 0.5f, 0.5f);
	
	public Color startingColour = new Color(1.0f,1.0f,1.0f,0.5f);
	public Color targetColour = new Color(1.0f,1.0f,1.0f,0.5f);
	
	//Global Data
	public Vector3 scale = Vector3.one;
	public float gravity = 0.0f;
	public float deltatime = 0.01f;
	public float minLife = 1.0f;
	public float maxLife = 2.5f;
	public float minSize = 1.0f;
	public float maxSize = 1.0f;
	public Vector3 minVelocity = Vector3.zero;
	public Vector3 maxVelocity = Vector3.zero;
	public float minScaleOverLife = 1.0f;
	public float maxScaleOverLife = 1.0f;
	public float drag = 0.99f;
	public bool killOnTimer = true;
	public bool wrap = false;
}
