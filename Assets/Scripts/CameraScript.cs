using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

//	private GameObject player;
//
//	// Use this for initialization
//	void Start () {
//		player = GameObject.Find ("Player");
//	}
//	
//	// Update is called once per frame
//	void LateUpdate () {
//		this.transform.position = new Vector3(player.transform.position.x, player.transform.position.y+5, player.transform.position.z);
//	}
	public float dampTime = 1f;
	private Vector3 velocity = Vector3.zero;
	public Transform target;

	void Start(){
		target = GameObject.Find ("Player").transform;
	}
	
	// Update is called once per frame
	void LateUpdate () 
	{
		if (target)
		{
			Vector3 point = GetComponent<Camera>().WorldToViewportPoint(target.position);
			Vector3 delta = target.position - GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
			Vector3 destination = transform.position + delta;
			transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
		}
//		transform.position = target.position + new Vector3(0,10,0);
		
	}
}
