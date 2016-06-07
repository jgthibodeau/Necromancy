using UnityEngine;
using System.Collections;

public class Camera2DScript : MonoBehaviour {
	public float dampTime = 1f;
	public float cameraRange = 5f;
	public float cameraHeight = 1f;
	private Vector3 velocity = Vector3.zero;
	public Transform target;

	void Start(){
		target = GameObject.Find ("Player").transform;
	}

	void LateUpdate () 
	{
		if (target)
		{
			Vector3 projectedTarget = target.position;
			//TODO only add this if it won't push the camera into a wall
			//ensure camera will not allow for looking beyond walls?

			Vector2 lookInput = GlobalScript.GetAxis (GlobalScript.RightStick);
			//player moving camera
			if (lookInput.magnitude > 0) {
				projectedTarget.z += lookInput.y * cameraRange;
				projectedTarget.x += lookInput.x * cameraRange;
			}
			//player not moving camera
			else {
				projectedTarget += target.forward * cameraRange;
			}

			Vector3 point = GetComponent<Camera>().WorldToViewportPoint(projectedTarget);
			Vector3 delta = projectedTarget - GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
			Vector3 destination = transform.position + delta;
			destination = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
			destination.y = target.position.y + cameraHeight;

			transform.position = destination;
		}
	}
}
