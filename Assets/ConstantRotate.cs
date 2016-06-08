using UnityEngine;
using System.Collections;

public class ConstantRotate : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate () {
		transform.Rotate (0, 50*Time.deltaTime, 0);
	}
}
