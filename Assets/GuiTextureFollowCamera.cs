using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiTextureFollowCamera : MonoBehaviour {
	private Vector3 initialPosition;

	public Vector3 offset;

	void Start() {
		initialPosition = transform.localPosition;
	}

	void Update () {
		//update position
		transform.localPosition = initialPosition;
		transform.position = transform.position + offset;

		//update rotation
		transform.LookAt (Camera.main.transform.position, Camera.main.transform.up);

		Vector3 rotation = transform.eulerAngles;
		rotation.z = 0;

		transform.eulerAngles = rotation;
	}
}
