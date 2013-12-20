using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;

public static class Utilities
{
	public static bool LoadResource<T>(ref T item, string file) where T : UnityEngine.Object
	{
		item = Resources.Load(file) as T;
		
		if(null == item)
		{
			return false;
		}
		return true;
	}
	
	//Loading a class
	public static bool LoadClassFromXML<T>(ref T data, string directory) where T : class
	{
		try
		{
			//Deserialise file
			XmlSerializer serializer = new XmlSerializer( typeof( T ) );
	        FileStream stream = new FileStream( directory, FileMode.Open );
	       	data = serializer.Deserialize( stream ) as T;
	        stream.Close( );
		}
		catch(Exception e)
		{
			Debug.LogError("Failed to load : " + data.ToString() + e.Message);	
			return false;
		}
		
		Debug.Log("Deserialised Data");
		
		return true;
	}
	
	//Saving a class
	public static bool SaveClassToXML<T>(T data, string directory) where T : class
	{
		Debug.Log("Saving data to : " + directory);
		try
        {
    		XmlSerializer serializer = new XmlSerializer( typeof( T ) );
            FileStream stream = new FileStream( directory, FileMode.Create );
            TextWriter s = new StreamWriter(stream, Encoding.UTF8);
            serializer.Serialize( s, data);
            stream.Close( );
        }
        catch(Exception e)
        {
            Debug.Log("Saving Error - " + e.ToString());
			return false;
        }
		
		Debug.Log("Data saved!");
		
		return true;
	}
}
