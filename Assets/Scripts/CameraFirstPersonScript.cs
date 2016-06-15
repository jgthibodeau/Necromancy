using UnityEngine;
using System.Collections;

public class CameraFirstPersonScript : MonoBehaviour {

	public Vector2 controllerSensitivity = new Vector2(1.0f, 0.7f);
	public bool controllerInvertY = true;
	public int verticalFov = 60;

	[HideInInspector]
	public float x;
	[HideInInspector]
	public float y;

	void Start(){
		
	}

	void LateUpdate () 
	{
		Vector2 lookInput = GlobalScript.GetStick (GlobalScript.RightStick);
		//player moving camera
		Vector3 rotation = transform.rotation.eulerAngles;
		float newX = rotation.x + lookInput.y * controllerSensitivity.y *(controllerInvertY ? 1 : -1);

		newX %= 360;

		if (newX > (verticalFov) && newX < 180)
			newX = verticalFov;
		if (newX < (360 - verticalFov) && newX > 180)
			newX = (360 - verticalFov);

		rotation.x = newX;

		transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.Euler (rotation), 40*Time.deltaTime);
//			Quaternion.Euler (rotation);
	}
}
