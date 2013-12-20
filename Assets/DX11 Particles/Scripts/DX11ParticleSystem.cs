/// <summary>
/// DX11 Particle System.
/// 
/// Author		: 	Dean Stanfield
/// Date  		:		18/02/2013
/// 
/// Description	:
/// This is the main particle system control which controls all aspects of a single instance of a particle
/// system in the scene.
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class DX11ParticleSystem : MonoBehaviour 
{

	// These structures should match exactly the structs inside the compute shader
	#region Compute Shader Structures
	//Per particle data
	protected struct PerParticleData
	{
		public Vector3 Originalposition;
		public Vector3 position;
		public Vector3 velocity;
		public Vector3 scale;
		public Vector4 colour;
		public float life;
		public float startLife;
		public Vector4 struts;

		//public Vector4 connectedParticles;
	};
	
	[System.Serializable]
	public struct GlobalData
	{
		public Vector3 origin;
		public Vector3 wind;
		public Vector3 scale;
		public float gravity;
		public float deltatime;
		public float minLife;
		public float maxLife;
		public float minSize;
		public float maxSize;
		public Vector3 minVelocity;
		public Vector3 maxVelocity;
		public float minScaleOverLife;
		public float maxScaleOverLife;
		public float drag;
		public int killOnTimer;
		public int wrap;
		public float scrollX;
		public float scrollY;
		public int numForcers;
		public Vector4 startColour;
		public Vector4 targetColour;
	};
	#endregion


	
	#region Particle Controls
	public bool m_bRender = true;
	public bool m_bUpdate = true;
	
	public bool RenderState
	{
		get { return m_bRender; }	
		set { m_bRender = value; }
	}
	
	public bool UpdateState
	{
		get { return m_bUpdate; }	
		set { m_bUpdate = value; }
	}
	
	public float m_fUpdateTime = 0.05f;
	protected float m_fCurrentUpdateTime = 0f;
	#endregion
	
	#region Shader Details
	//Num particles
	public int m_iNumParticles = 1000;
	private string m_sKernelName = "";
	private string m_sKernelNamev = "";
	private string m_sCSName = "";
	//private string m_sCSvName = "";
	private string m_sMaterialName = "";
	private string m_sMaterialNamev = "";	//TODO: Shows up in unity inspector?
	#endregion
	
	#region Particle Data
	protected PerParticleData[] m_PerParticleData;
	
	protected GlobalData[] m_GlobalParticleData;
	
	//Buffers
	protected ComputeBuffer m_PerParticleBuffer;
	protected ComputeBuffer m_GlobalParticleBuffer;
	#endregion
	
	#region Material data
	//Render Materials
	public Material m_Material;
	public Material m_Materialv;
	
	//Material settings
	public float size = 1.0f;
	public Color color = new Color(1.0f,0.6f,0.3f,0.03f);
	public Texture2D sprite;
	#endregion
	
	#region Compute Shader Data
	//Compute shader
	protected int m_iKernel;
	protected int m_iKernelv;
	public ComputeShader m_CS;
	public ComputeShader m_CSv;
	
	//per particle data
	public Color m_ParticleStartColor = new Color(1.0f,0.6f,0.3f,0.03f);
	public Color m_ParticleTargetColor = new Color(1.0f,0.6f,0.3f,0.03f);
	
	//Compute shader Global Settings
	public Transform m_Target;
	public Vector3 m_Wind = Vector3.zero;
	public Vector3 m_Scale = new Vector3(1,1,1);
	public float m_fGravity = 0f;
	public float m_fDeltaTime = 0.01f;
	public float m_fMinLife = 2f;
	public float m_fMaxLife = 8f;
	public float m_fMinScale = 1f;
	public float m_fMaxScale = 6f;
	public Vector3 m_MinVelocity = new Vector3(0f,1.0f,0f);
	public Vector3 m_MaxVelocity = new Vector3(0f,4.0f,0f);
	public float m_fMinScaleOverLife = 0.0f;
	public float m_fMaxScaleOverLife = 0.0f;
	public float m_fDrag = 0.97f;
	public bool m_bKillOnTimer = false;
	public bool m_bWrap = false;
	#endregion
	
	private void Init(int numParticles, string kernelName, string CSName, string material, string spriteName)
	{
		//Set num particles
		m_iNumParticles = numParticles;
		
		//Load shader
		m_sCSName = CSName;
		//m_sCSvName = CSName;
		Utilities.LoadResource<ComputeShader>(ref m_CS, m_sCSName);
		Utilities.LoadResource<ComputeShader>(ref m_CSv, "CSvParticle");

		//Original shader
		if(null == m_CS)
		{
			throw new System.ArgumentException("Item cannot be NULL!", m_sCSName);
		}
		else
		{
			//m_sKernelName = "CSParticle";//kernelName;
			m_sKernelName = kernelName;

			//Access shader
			m_iKernel = m_CS.FindKernel(m_sKernelName);
			
			if(-1 != m_iKernel)
			{
				m_sMaterialName = material;
				//valid kernel load material
				m_Material = Resources.Load(m_sMaterialName) as Material;
				
				//Valid material?
				if(null != m_Material)
				{	
					//Create data
					//SetupData();	//Hold off until second kernal prepped (but do error check)
					
					//AssignBuffers();
				}
				else
				{
					throw new System.ArgumentException("Item cannot be NULL!", m_sMaterialName);
				}
			}
			else
			{

				throw new System.InvalidOperationException("Kernel not found or not compiled correctly... please ensure the correct kernel is being loaded");
			}
		}

		/*
		sprite = Resources.Load(spriteName) as Texture2D;
		
		if(null == sprite)
		{
			throw new System.ArgumentException("Item cannot be NULL!", spriteName);
		}
		*/


		//New (velocity) shader
		if(null == m_CSv)
		{
			throw new System.ArgumentException("Item cannot be NULL!", "CSvParticle");
		}
		else
		{
			m_sKernelNamev = "CSvParticle";
			
			//Access shader
			m_iKernelv = m_CSv.FindKernel(m_sKernelNamev);

			SetupData();	//Skip checks, re-setup data now that second shader prepped
			
			AssignBuffers();

			if(-1 != m_iKernelv)
			{
				/*
				m_sMaterialNamev = materialv;
				//valid kernel load material
				m_Materialv = Resources.Load(m_sMaterialNamev) as Material;
				
				//Valid material?
				if(null != m_Materialv)
				{	
					//Create data
					SetupData();
					
					AssignBuffers();
				}
				else
				{
					throw new System.ArgumentException("Item cannot be NULL!", m_sMaterialNamev);
				}
				*/
			}
			else
			{
				throw new System.InvalidOperationException("Kernel not found or not compiled correctly... please ensure the correct kernel is being loaded");
			}
		}
		
		sprite = Resources.Load(spriteName) as Texture2D;
		
		if(null == sprite)
		{
			throw new System.ArgumentException("Item cannot be NULL!", spriteName);
		}
	}

	//Original Shader
	public void DeInit()
	{
		Debug.Log("Cleaning");
		if(m_PerParticleBuffer != null)
		{
			m_PerParticleBuffer.Release();
			m_PerParticleBuffer = null;
		}
		
		if(null != m_GlobalParticleBuffer)
		{
			m_GlobalParticleBuffer.Release();
			m_GlobalParticleBuffer = null;
		}
		
		m_CS = null;
		
		m_Material = null;

		//New velocity Shader
		m_CSv = null;
		
		m_Materialv = null;
	}
	
	protected virtual void AssignBuffers()
	{
		int stride = 0;
		if(m_PerParticleBuffer != null)
		{
			m_PerParticleBuffer.Release();
			m_PerParticleBuffer = null;
		}
		
		stride = Marshal.SizeOf(m_PerParticleData[0]);
		Debug.Log(" Data stride passed to GPU for Per particle data : " + stride);
		m_PerParticleBuffer = new ComputeBuffer(m_iNumParticles, stride);

		//Create global particle buffer
		if(null != m_GlobalParticleBuffer)
		{
			m_GlobalParticleBuffer.Release();
			m_GlobalParticleBuffer = null;
		}
		
		stride = Marshal.SizeOf(m_GlobalParticleData[0]);
		Debug.Log(" Data stride passed to GPU for global data : " + stride);
		m_GlobalParticleBuffer = new ComputeBuffer(1, stride);
		
		//Link Data to buffers
		m_PerParticleBuffer.SetData(m_PerParticleData);
		m_GlobalParticleBuffer.SetData(m_GlobalParticleData);
		
		//Connect to Compute shader
		m_CS.SetBuffer(m_iKernel, "PerParticleData", m_PerParticleBuffer);
		m_CS.SetBuffer(m_iKernel, "GlobalParticleData", m_GlobalParticleBuffer);

		//Connect to VELOCITY Compute shader too	//(Possible error?)
		m_CSv.SetBuffer(m_iKernel, "PerParticleData", m_PerParticleBuffer);
		m_CSv.SetBuffer(m_iKernel, "GlobalParticleData", m_GlobalParticleBuffer);
		
		//Connect to shader
		m_Material.SetBuffer("pointBuffer", m_PerParticleBuffer);
	}
	
	protected void SetupData()
	{
		ApplyGlobalDataSettings();
		
		CreateParticleArray();
	}
	
	/// <summary>
	/// Adds the or remove particles.
	/// </summary>
	protected virtual void CreateParticleArray()
	{
		m_PerParticleData = new PerParticleData[m_iNumParticles];
		
		//adding particles
		for(int i = 0; i < m_iNumParticles; i++)
		{
			//Create particle and add to list
			m_PerParticleData[i] = CreateParticle();
		}
	}
	
	protected PerParticleData CreateParticle()
	{
		PerParticleData data = new PerParticleData();
		data.scale = m_Scale;
		data.colour = new Vector4(0.5f,0.5f,0.5f,0.5f);
		data.life = -0.1f;
		data.startLife = data.life;
		data.position = new Vector3(Random.Range(-m_Scale.x, m_Scale.x), Random.Range(-m_Scale.y, m_Scale.y),Random.Range(-m_Scale.z, m_Scale.z));			
		data.Originalposition = data.position;
		data.velocity = new Vector3(Random.Range(m_MinVelocity.x, m_MaxVelocity.x),Random.Range(m_MinVelocity.x, m_MaxVelocity.x),Random.Range(m_MinVelocity.x, m_MaxVelocity.x));
		
		return data;
	}
	
	public virtual void ManualUpdate()
	{
		if(null != m_PerParticleBuffer)
		{
			int count = m_PerParticleData.Length;
			if(m_iNumParticles != count)
			{
				SetupData();
				AssignBuffers();
			}
		}
		
		m_fCurrentUpdateTime -= Time.deltaTime;
		
		if(m_fCurrentUpdateTime < 0.0f)	//If shader has not run yet?
		{
			m_fCurrentUpdateTime = m_fUpdateTime;
			
			ApplyGlobalDataSettings();
			
			if(m_CS != null)
			{
				m_CS.SetBuffer(m_iKernel, "PerParticleData", m_PerParticleBuffer);
				
				//Assign data... This allows multiple instances of the same buffer and stops them nuking each others memory
				m_CS.SetBuffer(m_iKernel, "GlobalParticleData", m_GlobalParticleBuffer);
				
				//Dispatch this kernel on the GFX Card
				m_CS.Dispatch(m_iKernel, m_iNumParticles, 1, 1);
			}

			//New velocity shader
			if(m_CSv != null)
			{
				m_CSv.SetBuffer(m_iKernel, "PerParticleData", m_PerParticleBuffer);
				
				//Assign data... This allows multiple instances of the same buffer and stops them nuking each others memory
				m_CSv.SetBuffer(m_iKernel, "GlobalParticleData", m_GlobalParticleBuffer);
				
				//Dispatch this kernel on the GFX Card
				m_CSv.Dispatch(m_iKernel, m_iNumParticles, 1, 1);
			}
		}
	}
	
	protected virtual void ApplyGlobalDataSettings()
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
		
		m_GlobalParticleData[0].numForcers = 0;
		m_GlobalParticleData[0].startColour = m_ParticleStartColor;
		m_GlobalParticleData[0].targetColour = m_ParticleTargetColor;
		
		if(null != m_GlobalParticleBuffer)
		{
			m_GlobalParticleBuffer.SetData(m_GlobalParticleData);	
		}
	}
	
	public virtual void ManualRender()
	{
		if(m_Material == null)
		{
			//Cannot continue rendering without material
			return;
		}
		
		m_Material.SetTexture("_Sprite", sprite);
		m_Material.SetFloat("_Size", size);
		m_Material.SetColor("_Color", color);
		
		//Connect to shader
		m_Material.SetBuffer("pointBuffer", m_PerParticleBuffer);
		
		m_Material.SetPass (0);
		Graphics.DrawProcedural(MeshTopology.Points, m_iNumParticles);
	}
	
	public void OnDrawGizmos()
	{
		if(null != m_GlobalParticleData)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube(m_GlobalParticleData[0].origin, m_GlobalParticleData[0].scale);
		}
	}
	
	public void SaveParticleSystemToXML(string location, string name)
	{
		Debug.Log("Saving effect to: " + location + " under name " + name);
		ParticleSaveData data = new ParticleSaveData();
		
		//New data.. or default if strings are empty or items are null
		data.numParticles = (m_iNumParticles > 0) ? m_iNumParticles : data.numParticles;
		data.kernelName = (m_sKernelName == "") ? data.kernelName : m_sKernelName;
		data.CSName = (m_sCSName == "") ? data.CSName : m_sCSName;
		data.materialName = (m_sMaterialName == "") ? data.materialName : m_sMaterialName;
		data.spriteName = (sprite == null) ? data.spriteName : sprite.name;
		
		data.startingColour = m_ParticleStartColor;
		data.targetColour = m_ParticleTargetColor;
		
		data.size = size;
		data.color = color;
		
		data.scale = m_Scale;
		data.gravity = m_fGravity;
		data.deltatime = m_fDeltaTime;
		data.minLife = m_fMinLife;
		data.maxLife = m_fMaxLife;
		data.minSize = m_fMinScale;
		data.maxSize = m_fMaxScale;
		data.minVelocity = m_MinVelocity;
		data.maxVelocity = m_MaxVelocity;
		data.minScaleOverLife = m_fMinScaleOverLife;
		data.maxScaleOverLife = m_fMaxScaleOverLife;
		data.drag = m_fDrag;
		data.killOnTimer = m_bKillOnTimer;
		data.wrap = m_bWrap;
		
		Utilities.SaveClassToXML<ParticleSaveData>(data, location + "\\" + name + ".xml");
	}

	//So sick of false compile error; overload by me
	public void LoadParticleSystemFromXML(string location, string file)
	{
		Transform target = null;	//By me; set to default, as his overload method throws errors
		LoadParticleSystemFromXML (location, file, target);
	}
	//public void LoadParticleSystemFromXML(string location, string file, Transform target = null)
	public void LoadParticleSystemFromXML(string location, string file, Transform target)
	{
		if(target == null)
		{
			m_Target = transform;
		}
		else
		{
			m_Target = target;
		}
		
		ParticleSaveData data = new ParticleSaveData();
		Utilities.LoadClassFromXML<ParticleSaveData>(ref data, location + "\\" + file + ".xml");
		
		if(data != null)
		{
			color = data.color;
			m_ParticleStartColor = data.startingColour;
			m_ParticleTargetColor = data.targetColour;
			size = data.size;
			
			//convert data into this class
			m_Scale = data.scale;
			m_fGravity = data.gravity ;
			m_fDeltaTime = data.deltatime;
			m_fMinLife = data.minLife;
			m_fMaxLife = data.maxLife;
			m_fMinScale = data.minSize;
			m_fMaxScale = data.maxSize;
			m_MinVelocity = data.minVelocity;
			m_MaxVelocity = data.maxVelocity;
			m_fMinScaleOverLife = data.minScaleOverLife;
			m_fMaxScaleOverLife = data.maxScaleOverLife;
			m_fDrag = data.drag;
			m_bKillOnTimer = data.killOnTimer;
			m_bWrap = data.wrap;
			
			Init (data.numParticles, data.kernelName, data.CSName, data.materialName, data.spriteName);			
		}
		else
		{
			throw new System.ArgumentException("Item could not be found", file);	
		}
		
		DX11ParticleController.AddParticleSystem(this);
	}
}
