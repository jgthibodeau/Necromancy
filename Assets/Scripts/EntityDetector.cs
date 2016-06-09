using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DetectionData {
	public float distance;
	public float percentVisible;
	public int numberCones;
	public int numberDirectCones;
	public Vector3[] boundPoints = new Vector3[0];
	public bool markedForDeletion = false;

	public bool isDirect(){
		return numberDirectCones > 0;
	}

	public Vector3 bottom(){
		return (boundPoints.Length > 0 ? boundPoints [1] : Vector3.zero);
	}
}

public class EntityDetector : MonoBehaviour {
	public Hashtable entities = new Hashtable ();
	int tick = 0;
	int tickRate = 5;
	public bool immediatelyDelete = true;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (tick == 0)
			DetectV2 ();
		tick = (tick + 1) % tickRate;
	}

	void DetectV2(){
		foreach (GameObject go in entities.Keys) {
			/*Raycast to make sure we have line of sight to the entity in question before adding values*/
			Bounds bounds = go.GetComponent<Collider> ().bounds;
			Vector3[] boundPoints = new Vector3[6];

			Vector3 eyeheight = transform.position;

			//top and bottom
			boundPoints [0] = go.transform.up * (bounds.size.y / 2f - .1f);
			boundPoints [1] = boundPoints [0] * -1;

			//left and right
			Vector3 directionToCenter = eyeheight - bounds.center;
			boundPoints [2] = Vector3.Cross (directionToCenter, go.transform.up);
			boundPoints [2] *= (bounds.size.x / 2f - .1f) / (boundPoints [2].magnitude);

			boundPoints [3] = boundPoints [2] * -1;

			//front and back
			boundPoints [4] = Vector3.Cross (go.transform.up, boundPoints[2]);
			boundPoints [4] *= (bounds.size.x / 2f - .1f) / (boundPoints [4].magnitude);

			boundPoints [5] = boundPoints [4] * -1;

			//corners
//			boundPoints [6] = Vector3.Lerp (boundPoints [0], boundPoints [2], 0.5f);
//			boundPoints [7] = Vector3.Lerp (boundPoints [0], boundPoints [3], 0.5f);
//			boundPoints [8] = Vector3.Lerp (boundPoints [1], boundPoints [2], 0.5f);
//			boundPoints [9] = Vector3.Lerp (boundPoints [1], boundPoints [3], 0.5f);
//
//			boundPoints [10] = Vector3.Lerp (boundPoints [0], boundPoints [4], 0.5f);
//			boundPoints [11] = Vector3.Lerp (boundPoints [0], boundPoints [5], 0.5f);
//			boundPoints [12] = Vector3.Lerp (boundPoints [1], boundPoints [4], 0.5f);
//			boundPoints [13] = Vector3.Lerp (boundPoints [1], boundPoints [5], 0.5f);
//
//			boundPoints [14] = Vector3.Lerp (boundPoints [4], boundPoints [2], 0.5f);
//			boundPoints [15] = Vector3.Lerp (boundPoints [4], boundPoints [3], 0.5f);
//			boundPoints [16] = Vector3.Lerp (boundPoints [5], boundPoints [2], 0.5f);
//			boundPoints [17] = Vector3.Lerp (boundPoints [5], boundPoints [3], 0.5f);

			//raycast to all points
			int totalAgree = 0;
			for(int i=0; i<boundPoints.Length;i++){
				boundPoints [i] += bounds.center;
				Vector3 point = boundPoints [i];
				//draw line onto entities collider
				Debug.DrawLine (bounds.center, point, Color.magenta);

				//check for visibility from us to entity
				RaycastHit hit;
				if (Physics.Linecast (eyeheight, point, out hit)) {
					if (hit.transform == go.transform) {
						Debug.DrawLine (eyeheight, point, Color.blue);
						totalAgree++;
					} else {
						Debug.DrawLine (eyeheight, point, Color.red);
					}
				}
			}

			DetectionData dd = (DetectionData)entities [go];
			dd.percentVisible = totalAgree / boundPoints.Length;
			dd.distance = Vector3.Distance (transform.position, go.transform.position);
			dd.boundPoints = boundPoints;
		}
	}

	void DetectV1(){
		foreach (GameObject go in entities.Keys) {
			/*Raycast to make sure we have line of sight to the entity in question before adding values*/
			Bounds bounds = go.GetComponent<Collider> ().bounds;
			Vector3[] boundPoints = new Vector3[9];

			Vector3 eyeheight = transform.position;

			//top and bottom
			boundPoints [0] = bounds.center + go.transform.up * (bounds.size.y / 2f - .1f);
			boundPoints [1] = bounds.center + go.transform.up * (bounds.size.y / -2f + .1f);

			//left and right
			Vector3 directionToCenter = eyeheight - bounds.center;
			boundPoints [2] = Vector3.Cross (directionToCenter, go.transform.up);
			boundPoints [2] *= (bounds.size.x / 2f - .1f) / (boundPoints [2].magnitude);
			boundPoints [2] += bounds.center;

			boundPoints [3] = Vector3.Cross (go.transform.up, directionToCenter);
			boundPoints [3] *= (bounds.size.x / 2f - .1f) / (boundPoints [3].magnitude);
			boundPoints [3] += bounds.center;

			//front
			boundPoints [4] = directionToCenter;
			boundPoints [4] *= (bounds.size.x / 2f - .1f) / (boundPoints [4].magnitude);
			boundPoints [4] += bounds.center;

			//corners
			boundPoints [5] = Vector3.Lerp (boundPoints [0], boundPoints [2], 0.5f);
			boundPoints [6] = Vector3.Lerp (boundPoints [0], boundPoints [3], 0.5f);
			boundPoints [7] = Vector3.Lerp (boundPoints [1], boundPoints [2], 0.5f);
			boundPoints [8] = Vector3.Lerp (boundPoints [1], boundPoints [3], 0.5f);

			//raycast to all points
			int totalAgree = 0;
			foreach (Vector3 point in boundPoints) {
				//draw line onto entities collider
				Debug.DrawLine (bounds.center, point, Color.magenta);

				//check for visibility from us to entity
				RaycastHit hit;
				if (Physics.Linecast (eyeheight, point, out hit)) {
					if (hit.transform == go.transform) {
						Debug.DrawLine (eyeheight, point, Color.blue);
						totalAgree++;
					} else {
						Debug.DrawLine (eyeheight, point, Color.red);
					}
				}
			}

			DetectionData dd = (DetectionData)entities [go];
			dd.percentVisible = totalAgree / boundPoints.Length;
			dd.distance = Vector3.Distance (transform.position, go.transform.position);
		}
	}

	public void AddEntity(GameObject go, VisionCone cone){
		if (entities.Contains (go)) {
			DetectionData dd = ((DetectionData)entities [go]);
			dd.numberCones++;
			dd.markedForDeletion = false;
			if(cone.type == VisionCone.Type.Direct)
				dd.numberDirectCones++;
		} else {
			DetectionData dd = new DetectionData();
			dd.numberCones++;
			dd.markedForDeletion = false;
			if(cone.type == VisionCone.Type.Direct)
				dd.numberDirectCones++;
			entities.Add (go, dd);
		}
	}

	public void RemoveEntity(GameObject go, VisionCone cone){
		if (entities.Contains (go)) {
			DetectionData dd = ((DetectionData)entities [go]);
			dd.numberCones--;
			if(cone.type == VisionCone.Type.Direct)
				dd.numberDirectCones--;
			if (dd.numberCones == 0) {
				if(immediatelyDelete)
					entities.Remove (go);
				else
					dd.markedForDeletion = false;
			}
		}
	}

	public void DeleteEntity(GameObject go){
		entities.Remove (go);
	}
}
