using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EntityDetector : MonoBehaviour {
	public Hashtable entities = new Hashtable ();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void AddEntity(GameObject go, VisionCone cone){
		if (entities.Contains (go)) {
			((List<VisionCone>)entities [go]).Add (cone);
		} else {
			List<VisionCone> cones = new List<VisionCone> ();
			cones.Add (cone);
			entities.Add (go, cones);
		}
	}

	public void RemoveEntity(GameObject go, VisionCone cone){
		if (entities.Contains (go)) {
			((List<VisionCone>)entities [go]).Remove (cone);
			if (((List<VisionCone>)entities [go]).Count == 0)
				entities.Remove (go);
		}
	}
}
