using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VisionCone : MonoBehaviour {
	public List<GameObject> objects;
	public enum Type{Peripheral, Direct};
	public Type type = Type.Direct;

	void Start(){
	}

	void OnTriggerEnter(Collider other) {
		//add object to list of collided objects
		objects.Add(other.gameObject);
	}

	void OnTriggerExit(Collider other){
		//remove object from list of collided objects
		objects.Remove (other.gameObject);
	}
}