using UnityEngine;
using System.Collections;

public class FpsCamera : MonoBehaviour {
	private Transform player;
	float rotation;

	void Start(){
		player = GameObject.FindGameObjectWithTag ("Player").transform;
		transform.parent = player;
	}

//	void LateUpdate(){
//		transform.position = player.position;
//		transform.RotateAround (transform.position, transform.up, rotation);
//		rotation += .5f;
//	}
}