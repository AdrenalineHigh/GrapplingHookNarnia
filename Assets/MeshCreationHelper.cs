/* ------------------------------------------------------------------------------------
   | Author:  Evelyn Liu (evelyn.liuqi@gmail.com).  
   | Created: 4/24/2010. (Modified 4/28/2010. Corrected a bug as suggested by Mark.)
   | Usage:   Mesh creation helper. To create a new mesh with one of the submesh only.
   ------------------------------------------------------------------------------------ */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*		//Simple code to pull up a few vertices
int i = 0;
while (i < oldVertices.Length*.7) {
    oldVertices[i] += Vector3.forward * Time.deltaTime;
    i++;
}
//Mesh newMesh = new Mesh();

//List<Vector3> newVertices = new List<Vector3>();
//List<Vector2> newUvs = new List<Vector2>();

oldMesh.vertices = oldVertices;
oldMesh.RecalculateBounds();
oldMesh.RecalculateNormals();

return oldMesh;
*/

 
public class MeshCreationHelper : MonoBehaviour
{
	///------------------------------------------------------------
	/// <summary>
	/// Create a new mesh with one of oldMesh's submesh
	/// </summary>
	///--------------~~~~------------------------------------------------
	public static Mesh CreateMesh(Mesh oldMesh)
	{
		
		Mesh newMesh = new Mesh();
		
        //Vector3[] oldVertices = oldMesh.vertices;
		
		//New mesh:
		//List<int> oldTriangles = new List<int>();
		//triangles.AddRange(oldMesh.GetTriangles(0)); // the triangles of the sub mesh
		//oldTriangles.AddRange(oldMesh.triangles);	// The triangles of the old mesh // Might cause errors
		//oldTriangles = oldMesh.triangles.ToList();	//broken
		int[] oldTriangles = oldMesh.triangles;
		int oldTrianglesSize = oldMesh.triangles.Length;
		
		List<Vector3> newVertices = new List<Vector3>();
		List<Vector2> newUvs = new List<Vector2>();
 
		List<int> validOldIndexes = new List<int>();	//Keep track of which indexes are used in new shard
		// Mark's method. 
		Dictionary<int, int> oldToNewIndices = new Dictionary<int, int>();
		int newIndex = 0;
		
		print("OldMesh vertices: " + oldMesh.vertices.Length);
		print("oldTrianglesSize : " + oldTrianglesSize);

		//int newIndex = 0;
		
		// Collect a smaller subset of vertices and uvs
		
		int loopIndex = (int)(oldMesh.vertices.Length*.7);	//Because unity seems to lack a method of rounding doubles; only rounds floats.
		
		for (int i = 0; i < loopIndex; i++)
		{
			newVertices.Add(oldMesh.vertices[i]);
			newUvs.Add(oldMesh.uv[i]);
			oldToNewIndices.Add(i, newIndex);
			++newIndex;
			validOldIndexes.Add (i);
		}
		
		//int[] newTriangles = new int[];
		List<int> newTriangles = new List<int>();
		//[oldTriangles.Count];
 
		
		int ind;
		bool goodTriangle;
		// Collect the new triangles indecies
		// Only collect triangles that have three valid indecies present in the new mesh
		for (int t = 0; t <= oldTrianglesSize - 3; t=t+3)	//Loop the triangles
		//for (int t = 0; t < loopIndex; t=t+3)	//Loop the triangles
		{
			goodTriangle = true;
			for (int i = 0; i < 3; i++)	//Loop their indexes
			{
				ind = i+(t);
				
				if (!validOldIndexes.Contains(oldTriangles[ind]))
				{
					goodTriangle = false;
					print ("invalid index "+ind);
					break;
				}
			}
			
			if (goodTriangle)
			{
				for (int i = 0; i < 3; i++)
				{
					ind = i+(t);
					//newTriangles.Add(ind);	//Create a new triangle with the proven-valid vertex/UV indexes
					//newTriangles.Add(oldToNewIndices[oldTriangles[ind]]);
					newTriangles.Add(oldTriangles[ind]);
				}
			}
		}
		
		
		// Assemble the new mesh with the new vertices/uv/triangles.
		newMesh.vertices = newVertices.ToArray();
		newMesh.uv = newUvs.ToArray();
		//newMesh.triangles = newTriangles;
 
				// Convert the list of new triangles to an array for Unity
		//string[] array = list.ToArray();
		newMesh.triangles = newTriangles.ToArray();
		
		// Re-calculate bounds and normals for the renderer.
		newMesh.RecalculateBounds();
		newMesh.RecalculateNormals();
 
		print("newMesh vertices: " + newMesh.vertices.Length);
		
		return newMesh;
		
		
		
		/*
		// Old 'submesh' code. For reference, then deletion
		Mesh newMesh = new Mesh();
 
		print("test " + oldMesh.vertices.Length);
		
		List<int> triangles = new List<int>();
		triangles.AddRange(oldMesh.GetTriangles(0)); // the triangles of the sub mesh
 
		List<Vector3> newVertices = new List<Vector3>();
		List<Vector2> newUvs = new List<Vector2>();
 
		// Mark's method. 
		Dictionary<int, int> oldToNewIndices = new Dictionary<int, int>();
		int newIndex = 0;
 
		// Collect the vertices and uvs
		for (int i = 0; i < oldMesh.vertices.Length; i++)
		{
			if (triangles.Contains(i))
			{
				newVertices.Add(oldMesh.vertices[i]);
				newUvs.Add(oldMesh.uv[i]);
				oldToNewIndices.Add(i, newIndex);
				++newIndex;
			}
		}
 
		int[] newTriangles = new int[triangles.Count];
 
		// Collect the new triangles indecies
		for (int i = 0; i < newTriangles.Length; i++)
		{
			newTriangles[i] = oldToNewIndices[triangles[i]];
		}
		// Assemble the new mesh with the new vertices/uv/triangles.
		newMesh.vertices = newVertices.ToArray();
		newMesh.uv = newUvs.ToArray();
		newMesh.triangles = newTriangles;
 
		// Re-calculate bounds and normals for the renderer.
		newMesh.RecalculateBounds();
		newMesh.RecalculateNormals();
 
		return newMesh;
		*/
	}
}