using UnityEngine;
using System.Collections;

public class FieldOfViewScript : MonoBehaviour {
	public bool drawFovs = true;
	public float frontFov; // in degrees
	public float peripheralFov; // in degrees
	public float backFov = 360;
	public float frontViewDistance;
	public float peripheralViewDistance;
	public float backViewDistance;

	private Mesh frontMesh;
	private Mesh peripheralMesh;
	private Mesh backMesh;

	private float segmentsPerDegree = 0.5f;

	public Material frontMaterial;
	public Material peripheralMaterial;
	public Material backMaterial;

	void Start(){
		//front fov
		if (drawFovs) {
			buildMesh ("Front FOV", frontMesh, frontMaterial, frontFov, frontViewDistance, 0.01f);
			buildMesh ("Peripheral FOV", peripheralMesh, peripheralMaterial, peripheralFov, peripheralViewDistance, 0.02f);
			buildMesh ("Back FOV", backMesh, backMaterial, backFov, backViewDistance, 0.03f);
		}
	}

	void buildMesh(string name, Mesh mesh, Material material, float angle, float radius, float height){

		GameObject meshObject = new GameObject();
		meshObject.transform.position = gameObject.transform.position;
		meshObject.transform.position -= (gameObject.transform.up * height);
		meshObject.transform.parent = gameObject.transform;
		meshObject.name = name;

		var MeshF = meshObject.AddComponent<MeshFilter>();
		var MeshR = meshObject.AddComponent<MeshRenderer>();
		MeshR.material = material;
		mesh = MeshF.mesh;

		//Clear the mesh
		mesh.Clear();

		// Calculate actual pythagorean angle
		float actualAngle = 90.0f - angle;

		// Segment Angle
		float segments = segmentsPerDegree * angle;
		float segmentAngle = angle * 2  /segments;

		// Initialise the array lengths
		Vector3[] verts = new Vector3[(int)segments * 3];
		Vector3[] normals = new Vector3[(int)segments * 3];
		int[] triangles = new int[(int)segments * 3];
		Vector2[] uvs = new Vector2[(int)segments * 3];

		// Initialise the Array to origin Points
		for (int i = 0; i < verts.Length; i++){
			verts[i] = new Vector3(0,0,0);
			normals[i] = Vector3.up;
		}

		// Create a dummy angle
		float a = actualAngle;

		// Create the Vertices
		for (int i = 1; i < verts.Length; i+=3){
			verts[i] = new Vector3( Mathf.Cos( Mathf.Deg2Rad*a) * radius, // x
				0,                                                                // y
				Mathf.Sin(Mathf.Deg2Rad*a) * radius);  // z

			a += segmentAngle;

			verts[i+1] = new Vector3( Mathf.Cos( Mathf.Deg2Rad*a) * radius, // x
				0,                                                                // y
				Mathf.Sin(Mathf.Deg2Rad*a) * radius);  // z          
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
			uvs[i] = new Vector2(verts[i].x, verts[i].z);
		}

		// Put all these back on the mesh
		mesh.vertices = verts;
		mesh.normals = normals;
		mesh.triangles = triangles;
		mesh.uv = uvs;   
	}  
}