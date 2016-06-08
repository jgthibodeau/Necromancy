using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VisionCone : MonoBehaviour {
//	public List<GameObject> objects;
	public enum Type{Peripheral, Direct};
	public Type type = Type.Direct;
	private EntityDetector detector;

	void Start(){
		detector = GetComponentInParent<EntityDetector> ();
	}

	void OnTriggerEnter(Collider other) {
		//add object to list of collided objects
		if(other.gameObject.layer == 10)
//			objects.Add(other.gameObject);
			detector.AddEntity(other.gameObject, this);
	}

	void OnTriggerExit(Collider other){
		//remove object from list of collided objects
		if(other.gameObject.layer == 10)
//			objects.Remove (other.gameObject);
			detector.RemoveEntity(other.gameObject, this);
	}
}