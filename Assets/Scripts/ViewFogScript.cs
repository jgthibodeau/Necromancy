using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ViewFogScript : MonoBehaviour {
	public float radius = 5.0f;
	public int segments = 12;
	public float curveAmount = 360.0f;
	private float calcAngle;
	private List<Vector3> nodes = new List<Vector3> ();
	private Transform player;
	private Vector3[] vertices;

	public Material material;
	private GameObject meshObject;
	private Mesh mesh;
	private MeshFilter MeshF;
	private MeshRenderer MeshR;

	void Start(){
		meshObject = new GameObject();
		meshObject.transform.parent = gameObject.transform;

		MeshF = meshObject.AddComponent<MeshFilter>();
		MeshR = meshObject.AddComponent<MeshRenderer>();

		MeshR.material = material;
		mesh = MeshF.mesh;
	}

	void Update() 
	{
		CalculatePoints();        
		DrawLines();    // just for testing    
		CheckForHits();
		DrawMesh ();
	}

	void CalculatePoints() 
	{
		nodes.Clear();
		calcAngle = 0;

		// Calculate Arc on Y-Z    
		for ( int i = 0; i < segments + 1; i ++ )
		{
			float posY = Mathf.Cos( calcAngle * Mathf.Deg2Rad ) * radius;
			float posZ = Mathf.Sin( calcAngle * Mathf.Deg2Rad ) * radius;            
			nodes.Add( transform.position + ( transform.forward * posY ) + ( transform.right * posZ ) );    
			calcAngle += curveAmount / segments;
		}
	}

	void CheckForHits() 
	{
		RaycastHit hit;
		vertices = new Vector3[nodes.Count];
		for ( int i = 0; i < nodes.Count - 1; i ++ )
		{
			if ( Physics.Linecast( transform.position, nodes[i], out hit ) )
			{
				vertices [i] = hit.point;
				Debug.DrawLine( transform.position, hit.point, Color.blue );
			}
		}
	}

	void DrawLines() 
	{
		for ( int i = 0; i < nodes.Count - 1; i ++ )
		{
			Debug.DrawLine( nodes[i], nodes[i + 1], Color.red );
		}
	}

	void DrawMesh(){
		meshObject.transform.position = gameObject.transform.position;
//			meshObject.transform.position -= (gameObject.transform.up * height);

		//Clear the mesh
		mesh.Clear();

		// Initialise the array lengths
//			Vector3[] verts = new Vector3[(int)segments * 3];
		Vector3[] normals = new Vector3[vertices.Length];
		int[] triangles = new int[vertices.Length];
		Vector2[] uvs = new Vector2[vertices.Length];

		// Initialise the Array to origin Points
		for (int i = 0; i < normals.Length; i++){
//				verts[i] = new Vector3(0,0,0);
			normals[i] = Vector3.up;
		}

		// Create Triangle
		for (int i = 0; i < triangles.Length; i+=3)
		{
			triangles[i] = 0;
			triangles[i+1] = i + 2;
			triangles[i+2] = i + 1;
		}

		// Generate planar UV Coordinates
		for (int i = 0; i < uvs.Length; i++){
			uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
		}

		// Put all these back on the mesh
		mesh.vertices = vertices;
		mesh.normals = normals;
		mesh.triangles = triangles;
		mesh.uv = uvs;
	}
}