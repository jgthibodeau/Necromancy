using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PrimitivesPro.GameObjects;

public class VisionCone : MonoBehaviour {
	public enum Type{Peripheral, Direct};
	public Type type = Type.Direct;
	public float length;
	public float priority;
	public float sensitivity;

	private EntityDetector detector;

	void Start(){
		detector = GetComponentInParent<EntityDetector> ();
		length = GetComponent<SphericalCone> ().radius*2;
	}

	void OnTriggerEnter(Collider other) {
		//add object to list of collided objects
		if(other.gameObject.GetComponent<Detectable> () != null)
			detector.AddEntity(other.gameObject, this);
	}

	void OnTriggerExit(Collider other){
		//remove object from list of collided objects
		if(other.gameObject.GetComponent<Detectable> () != null)
			detector.RemoveEntity(other.gameObject, this);
	}
}