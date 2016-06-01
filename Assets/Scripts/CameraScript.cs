using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {
	public float dampTime = 1f;
	public float cameraRange = 5f;
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
			projectedTarget += target.up * cameraRange;

			Vector3 point = GetComponent<Camera>().WorldToViewportPoint(projectedTarget);
			Vector3 delta = projectedTarget - GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
			Vector3 destination = transform.position + delta;
			transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
		}
	}
}
