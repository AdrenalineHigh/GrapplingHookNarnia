using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class DX11ForceExtension : DX11ParticleSystem
{
	struct ForceData
	{
		public Vector3 position;
		public float force;
	};
	
	public GameObject[] forcers;
	public float[] force;
	
	ForceData[] m_ForceData;
	
	ComputeBuffer m_ForceBuffer;
	
	public override void ManualUpdate ()
	{	
		m_fCurrentUpdateTime -= Time.deltaTime;
		
		//m_Target.position = Vector3.Lerp(m_Target.position, chase.position, Time.time);
		
		if(m_fCurrentUpdateTime < 0.0f)
		{
			m_fCurrentUpdateTime = m_fUpdateTime;
			
			ApplyGlobalDataSettings();
			
			if(m_CS != null)
			{			
				//Assign data... This allows multiple instances of the same buffer and stops them nuking each others memory
				m_CS.SetBuffer(m_iKernel, "GlobalParticleData", m_GlobalParticleBuffer);
				
				//Dispatch this kernel on the GFX Card
				m_CS.Dispatch(m_iKernel, m_iNumParticles, 1, 1);
			}
		}
	}
	
	protected override void AssignBuffers ()
	{
		base.AssignBuffers ();
		
		//Create additional buffers
		int stride = 0;
		if(m_ForceBuffer != null)
		{
			m_ForceBuffer.Release();
			m_ForceBuffer = null;
		}
		
		stride = Marshal.SizeOf(m_ForceData[0]);
		Debug.Log(" Data stride passed to GPU for Per particle data : " + stride);
		m_ForceBuffer = new ComputeBuffer(m_iNumParticles, stride);
		
		//Link Data to buffers
		m_ForceBuffer.SetData(m_ForceData);
		
		//Connect to Compute shader
		m_CS.SetBuffer(m_iKernel, "forceData", m_ForceBuffer);
	}
	
	protected override void ApplyGlobalDataSettings ()
	{
		m_GlobalParticleData = new GlobalData[1];
		
		m_GlobalParticleData[0] = new GlobalData();
		
		if(m_Target != null)
		{
			m_GlobalParticleData[0].origin = m_Target.position;
		}
		else
		{
			m_GlobalParticleData[0].origin = Vector3.zero;
		}
		
		m_GlobalParticleData[0].wind = m_Wind;
		m_GlobalParticleData[0].scale = m_Scale;
		m_GlobalParticleData[0].gravity = m_fGravity;
		m_GlobalParticleData[0].deltatime = m_fDeltaTime;
		m_GlobalParticleData[0].minLife = m_fMinLife;
		m_GlobalParticleData[0].maxLife = m_fMaxLife;
		m_GlobalParticleData[0].minSize = m_fMinScale;
		m_GlobalParticleData[0].maxSize = m_fMaxScale;
		m_GlobalParticleData[0].minVelocity = m_MinVelocity;
		m_GlobalParticleData[0].maxVelocity = m_MaxVelocity;
		m_GlobalParticleData[0].minScaleOverLife = m_fMinScaleOverLife;
		m_GlobalParticleData[0].maxScaleOverLife = m_fMaxScaleOverLife;
		m_GlobalParticleData[0].drag = m_fDrag;
		
		//Convert boolean to int as Compute Shaders appear to struggle with bool values
		m_GlobalParticleData[0].killOnTimer = (true == m_bKillOnTimer) ? 1 : 0;
		m_GlobalParticleData[0].wrap = (true == m_bWrap) ? 1 : 0;
		
		m_GlobalParticleData[0].scrollX = 0f;
		m_GlobalParticleData[0].scrollY = 0f;
		
		m_GlobalParticleData[0].numForcers = forcers.Length;
		
		if(null != m_GlobalParticleBuffer)
		{
			m_GlobalParticleBuffer.SetData(m_GlobalParticleData);	
		}
		
		//Create force buffers
		int size = forcers.Length;
		m_ForceData = new ForceData[size];
		
		for(int i = 0; i < size; i++)
		{
			m_ForceData[i].position = forcers[i].transform.position;
			m_ForceData[i].force = force[i];
		}
		
		if(null != m_ForceBuffer)
		{
			m_ForceBuffer.SetData(m_ForceData);
		}
	}
}
