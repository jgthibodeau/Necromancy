using UnityEngine;
using System.Collections;

public class WaypointFollower : MonoBehaviour {
	//TODO savable
	// public int currentWaypoint;
	// public bool incrementIndex = true;

	// public UnityEngine.AI.NavMeshAgent agent;

	// //Patroling waypoints
	// public GameObject waypointRoot;
	// public Waypoint[] waypoints;
	// public bool loop = true;
	// public bool reverse = false;
	// public float minAngle;

	// Use this for initialization
	void Start () {
// //		waypointRoot.SetActive (true);
// 		waypoints = waypointRoot.GetComponentsInChildren<Waypoint> (true);
// //		waypointRoot.SetActive (false);

// 		Debug.Log (waypoints[0].transform.position +" "+waypointRoot.transform.position);
// 		Debug.Break ();

// 		agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// //TODO
	// public void SetToClosestNode(){
	// }

// 	public void FollowPath() {
// 		Waypoint waypoint = waypoints [currentWaypoint];
// 		Vector3 target = waypoint.transform.position;

// 		//TODO handle pausing
// 		Debug.Log("waypoint: "+waypoint+" "+waypoint.lookOnly);

// 		if (waypoint.lookOnly){
// //			agent.SetDestination (null);
// 			agent.updatePosition = false;
// 			agent.updateRotation = false;

// 			Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);
// 			transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, agent.angularSpeed * Time.deltaTime);

// 			if (LookingAt (target)) {
// 				NextWayPoint ();
// 			}
// 		}
// 		else{
// 			agent.updatePosition = true;
// 			agent.updateRotation = true;

// 			target = waypoints [currentWaypoint].transform.position;
// 			agent.ResetPath ();
// 			agent.SetDestination (target);
// 			Debug.Log ("agent "+agent.destination+" "+agent.pathPending+" "+agent.hasPath);
// 			if (AtTarget (target)) {
// 				NextWayPoint ();
// 			}
// 		}

// 	}

// 	bool LookingAt(Vector3 target){
// 		Vector3 targetDirection = target - transform.position;
// 		float angleBetween = Vector3.Angle (transform.forward, targetDirection);
// //		Debug.Log (angleBetween);
// 		return angleBetween < minAngle;
// 	}

// 	bool AtTarget(Vector3 target){
// //		Debug.Log (agent.pathPending+" "+agent.remainingDistance+" "+pathComplete());
// 		bool done = AtEndOfPath();
// 		Debug.Log (done);
// 		return done;//agent.hasPath && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
// //		return false;
// 	}

// 	private bool hasPath = false;
// 	public float pathEndThreshold = 0.1f;
// 	bool AtEndOfPath()
// 	{
// 		hasPath |= agent.hasPath;
// 		if (!agent.pathPending && hasPath && agent.remainingDistance <= agent.stoppingDistance + pathEndThreshold )
// 		{
// 			// Arrived
// 			hasPath = false;
// 			return true;
// 		}

// 		return false;
// 	}

// 	void NextWayPoint(){
// 		int prevWaypoint = currentWaypoint;

// 		//set to next waypoint
// 		if (incrementIndex) {
// 			currentWaypoint++;
// 			if (currentWaypoint > waypoints.Length - 1) {
// 				if (loop)
// 					currentWaypoint = 0;
// 				else if (reverse) {
// 					currentWaypoint--;
// 					incrementIndex = false;
// 				} else
// 					currentWaypoint = prevWaypoint;
// 			}
// 		} else {
// 			currentWaypoint--;
// 			if (currentWaypoint < 0) {
// 				if (loop)
// 					currentWaypoint = waypoints.Length - 1;
// 				else if (reverse) {
// 					currentWaypoint++;
// 					incrementIndex = true;
// 				} else
// 					currentWaypoint = prevWaypoint;
// 			}
// 		}
// 	}

// //	public void NextWaypoint(Waypoint waypoint)
// //	{
// //		//Collided with current waypoint target?
// //		if (waypoint == waypoints[currentWaypoint])
// //		{
// //			m_currentTargetIndex++;
// //
// //			if (m_currentTargetIndex == m_targets.Count)
// //				RandomActivity();
// //			else
// //			{
// //				Debug.Log("Target: " + (m_currentTargetIndex + 1) + "/" + m_targets.Count + " (" + m_targets[m_currentTargetIndex].transform.position + ")");
// //				m_navMeshAgent.SetDestination(m_targets[m_currentTargetIndex].transform.position);
// //			}
// //		}
// //	}
}
