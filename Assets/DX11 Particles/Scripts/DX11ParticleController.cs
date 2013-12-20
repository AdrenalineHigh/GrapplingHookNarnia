/// <summary>
/// DX11 Particle Controller.
/// 
/// Author		: 	Dean Stanfield
/// Date  		:		18/02/2013
/// 
/// Description	:
/// This is the Particle system controller which manages the updates and rendering of each
/// particle system created in the scene.
/// 
/// Use:
/// Ensure ManualUpdate(), RenderAllParticles() and OnManualDestroy() are called from one location on the corresponding unity functions
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class DX11ParticleController 
{
	//Local list of all particle systems in the scene... Particle systems add themselves during DX11ParticleSystem.Init()
	private static List<DX11ParticleSystem> m_ParticleSystems = new List<DX11ParticleSystem>();
	public static bool m_bComputeShadersSupported = false;
	
	public static void Init()
	{
		m_bComputeShadersSupported = SystemInfo.supportsComputeShaders;
	}
	
	/// <summary>
	/// Adds the particle system.
	/// 
	/// Do not call this function unless manually adding a particle system.
	/// 
	/// Most particle systems are automatically added during DX11ParticleSystem.Init()
	/// </summary>
	/// <param name='item'>
	/// Item.
	/// </param>
	public static void AddParticleSystem(DX11ParticleSystem item)
	{
		if(false == m_ParticleSystems.Contains(item))
		{
			m_ParticleSystems.Add(item);
			Debug.Log("System count = " + m_ParticleSystems.Count);
		}
	}
	
	/// <summary>
	/// Creates a new particle system on a given gameObject.
	/// </summary>
	/// <param name='obj'>
	/// Object.
	/// </param>
	/// <param name='target'>
	/// Target.
	/// </param>
	public static DX11ParticleSystem CreateNewParticleSystem(GameObject obj, Transform target, string location, string effectName)
	{
		DX11ParticleSystem script = null;
		
		if(true == m_bComputeShadersSupported)
		{
			script = obj.AddComponent<DX11ParticleSystem>();
			script.LoadParticleSystemFromXML(location, effectName, target);
		}
		else
		{
			throw new System.ArgumentException("Cannot create DX11ParticleSystem: Compute Shader supported: " + m_bComputeShadersSupported);
		}
		
		//Return script for manual editing if required
		return script;
	}
	
	/// <summary>
	/// Manuals the update.
	/// </summary>
	public static void ManualUpdate () 
	{
		if(true == m_bComputeShadersSupported)
		{
			for(int i = 0; i < m_ParticleSystems.Count; i++)
			{

				if(true == m_ParticleSystems[i].UpdateState)
				{
					m_ParticleSystems[i].ManualUpdate();	
				}
			}
		}
	}
	
	/// <summary>
	/// Raises the manual destroy event.
	/// 
	/// Call this on destruction of the session or program.
	/// 
	/// Cleans the scene of any particle systems running
	/// </summary>
	public static void OnManualDestroy()
	{		
		if(true == m_bComputeShadersSupported)
		{
			//Destroy
			for(int i = 0; i < m_ParticleSystems.Count; i++)
			{
				m_ParticleSystems[i].DeInit();	
			}
		}
	}
	
	/// <summary>
	/// Renders all particles.
	/// 
	/// This can be called for a specific camera using OnPostRender() or for the scene view using OnRenderObject()
	/// Use OnPostRender for multiple camera setups.
	/// </summary>
	public static void RenderAllParticles()
	{
		if(true == m_bComputeShadersSupported)
		{
			for(int i = 0; i < m_ParticleSystems.Count; i++)
			{
				if(true == m_ParticleSystems[i].RenderState)
				{
					m_ParticleSystems[i].ManualRender();
				}
			}
		}
	}
}
