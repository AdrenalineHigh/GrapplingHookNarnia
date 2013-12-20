using UnityEngine;
using System.Collections;

public enum VisualEffect
{
	None,
	
}

public class VisualEffects : MonoBehaviour 
{
	public VisualEffect m_CurrentEffect = VisualEffect.None;
	private Material m_Material = null;
	
	// Use this for initialization
	void Init()
	{
		if(false == GatherResources())
		{
			Debug.LogError("Not all resources have been gathers for Visual Effects");	
		}
	}
	
	bool GatherResources()
	{
		return true;	
	}
	
	/// <summary>
	/// Raises the render image event.
	/// </summary>
	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		if(null == m_Material)
		{
			//Cant process anything on null material... fall out
			return;
		}
		
		Graphics.Blit(source, destination, m_Material);
	}
}
