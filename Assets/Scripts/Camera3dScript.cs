using UnityEngine;
using System.Collections;

public class Camera3DScript : MonoBehaviour {
	public float collideTime = 1f;
	public float dampTime = 1f;
	public float distance = 5f;

	private bool collided = false;

	public float currentX = 0f;
	public float currentY = -45f;
	private float sensitivityX = 4f;
	private float sensitivityY = 4f;

	public float maxY = 90f;
	public float minY = 0f;

	public Transform lookAt;
	public Transform camTransform;

	private Camera cam;


	void Start(){
		lookAt = GameObject.Find ("Player").transform;

		camTransform = transform;
		cam = Camera.main;
	}

	void LateUpdate(){
		if (!collided) {
			Vector2 lookInput = GlobalScript.GetAxis (GlobalScript.RightStick);
			currentX += sensitivityX * lookInput.x;
			currentY += sensitivityY * lookInput.y;

//			if (currentY > maxY)
//				currentY = maxY;
//			else if (currentY < minY)
//				currentY = minY;
			
			Vector3 direction = new Vector3 (0, distance, 0);
			Quaternion rotation = Quaternion.Euler (currentY, currentX, 0);

			camTransform.position = Vector3.Lerp (camTransform.position, lookAt.position + rotation * direction, Time.deltaTime*dampTime);
			//		camTransform.position = lookAt.position + rotation * direction;
		} else {
//			camTransform.position = Vector3.Lerp (camTransform.position, lookAt.position, Time.deltaTime*collideTime);
		}
			

		camTransform.LookAt (lookAt);
	}

	void OnCollisionEnter(Collision collision) {
		collided = true;
	}

	void OnCollisionExit(Collision collision){
		collided = false;
	}
}
