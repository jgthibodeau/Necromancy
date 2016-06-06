using UnityEngine;
using System.Collections;

public class MyThirdPersonCamera : MonoBehaviour {
	public float distanceAway;
	public float distanceUp;
	public float smooth;

	private Transform follow;
	private Vector3 targetPosition;

	void Start(){
		follow = GameObject.FindWithTag ("Player").transform;
	}

	void LateUpdate(){
		targetPosition = follow.position + follow.up * distanceUp - follow.forward * distanceAway;

		Debug.DrawRay (follow.position, follow.up * distanceUp, Color.red);
		Debug.DrawRay (follow.position, -1f * follow.forward * distanceAway, Color.blue);
		Debug.DrawLine (follow.position, targetPosition, Color.magenta);

		transform.position = Vector3.Lerp (transform.position, targetPosition, Time.deltaTime * smooth);
		transform.LookAt (follow);
	}
}
