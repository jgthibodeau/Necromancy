using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]

public class Waypoint : MonoBehaviour {
	public float pauseTime = 0f;
	public bool lookOnly = false;
	public float lookSpeed = 0f;
	public bool forceExactCenter = false;
//	public bool lookAround = false;
//	public bool lookForward = false;

	public void OnTriggerEnter(Collider collider)
	{
		if (collider.tag == "Enemy")
			collider.GetComponent<AgentController>().CollideWaypoint(this);
	}
}
