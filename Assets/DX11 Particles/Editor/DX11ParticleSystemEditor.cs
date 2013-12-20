using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(DX11ParticleSystem))]
public class DX11ParticleSystemEditor : Editor 
{
	string saveName = "SaveName";
	string location = Application.dataPath + "\\DX11 Particles\\ParticleSystems";
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		
		if(GUILayout.Button("Save as XML"))
		{
      		((DX11ParticleSystem)target).SaveParticleSystemToXML(location, saveName);
		}
		
		if(GUILayout.Button("Load from XML"))
		{
			
      		((DX11ParticleSystem)target).LoadParticleSystemFromXML(location, saveName);
		}
		
		location = GUILayout.TextField(location);
		saveName = GUILayout.TextField(saveName);
	}
}
