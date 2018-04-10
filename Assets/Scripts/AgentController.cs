using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
public class AgentController : MonoBehaviour
{
	// [SerializeField]
	// private Transform       m_waypointParent;
	// [SerializeField]
	// private float           m_minIdle;
	// [SerializeField]
	// private float           m_maxIdle;
	// [SerializeField]
	// private float           m_patrolChance;
	// [SerializeField]
	// private int             m_minPatrolPoints;
	// [SerializeField]
	// private int             m_maxPatrolPoints;

	// [SerializeField]
	// private Waypoint[]      m_waypoints;
	// private List<Waypoint>  m_availableTargets;
	// private List<Waypoint>  m_targets;
	// private Waypoint        m_tempWaypoint;
	// [SerializeField]
	// private int             m_currentTargetIndex;
	// private Waypoint		m_currentWaypoint;
	// private UnityEngine.AI.NavMeshAgent    m_navMeshAgent;

	// public bool incrementIndex = true;
	// public bool loop = true;
	// public bool reverse = false;
	// public float minAngle = 10;
	// public float turnSpeed = 10f;

	// public bool followPath = true;

	// private bool waypointCollided = false;
	// private float waypointPauseTime = 0f;
	// private float pauseStartTime;

	public void Start()
	{
// 		m_waypoints     = m_waypointParent.GetComponentsInChildren<Waypoint>();
// 		m_targets       = new List<Waypoint>();
// 		m_navMeshAgent  = GetComponent<UnityEngine.AI.NavMeshAgent>();
// 		m_currentWaypoint = m_waypoints [0];
// //		RandomActivity();
	}

	public void SetWaypointParent(Transform parent){
		// m_waypointParent = parent;
		// m_waypoints     = m_waypointParent.GetComponentsInChildren<Waypoint>();
		// m_targets       = new List<Waypoint>();
		// m_currentWaypoint = m_waypoints [0];
	}

	public void ResumePath(){
		// followPath = true;
	}
	public void PausePath(){
		// followPath = false;
	}
// 
	// public void Update(){
// 		if (followPath) {
// 			//do nothing until time has passed if paused
// 			if (waypointPauseTime > 0 && (Time.time - pauseStartTime < waypointPauseTime))
// 				return;

// 			bool waypointDone = false;
// 			waypointPauseTime = 0f;
// 
// 			//if looking towards waypoint
// 			if (m_currentWaypoint.lookOnly) {
// 				//rotate towards waypoint until we are looking at it
// //				m_navMeshAgent.updatePosition = false;
// 				m_navMeshAgent.updateRotation = false;
// 				Vector3 target = m_currentWaypoint.transform.position;
// 
// 				Quaternion targetRotation = Quaternion.LookRotation (target - transform.position);
// //				transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, m_currentWaypoint.lookSpeed * Time.deltaTime);
// 				transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

// 				if (LookingAt (target)) {
// 					waypointDone = true;
// 				}
// 			}

// 			//if moving towards waypoint
// 			else {
// 				//set agent destination
// 				m_navMeshAgent.updatePosition = true;
// 				m_navMeshAgent.updateRotation = true;
// 				m_navMeshAgent.SetDestination (m_currentWaypoint.transform.position);

// 				//if current waypoint has been triggered, wait until it is centered
// 				if (waypointCollided) {
// 					if (!m_currentWaypoint.forceExactCenter || m_navMeshAgent.remainingDistance <= m_navMeshAgent.stoppingDistance) {
// 						waypointCollided = false;
// 						waypointDone = true;
// 					}
// 				}
// 			}

// 			if (waypointDone) {
// 				pauseStartTime = Time.time;
// 				waypointPauseTime = m_currentWaypoint.pauseTime;
// 				m_currentWaypoint = NextWaypoint ();
// 			}
// 		}
	// }

// 	bool LookingAt(Vector3 target){
// 		Vector3 targetDirection = target - transform.position;
// 		float angleBetween = Vector3.Angle (transform.forward, targetDirection);
// 		//		Debug.Log (angleBetween);
// 		return angleBetween < minAngle;
// 	}

// 	public void CollideWaypoint(Waypoint p_waypoint){
// 		//Collided with current waypoint target?
// 		if ((m_currentTargetIndex == -1) || (p_waypoint == m_waypoints [m_currentTargetIndex]))
// 			waypointCollided = true;
// 	}

// 	public Waypoint NextWaypoint()
// 	{
// 		int prevWaypoint = m_currentTargetIndex;

// 		//set to next waypoint
// 		if (incrementIndex) {
// 			m_currentTargetIndex++;
// 			if (m_currentTargetIndex > m_waypoints.Length - 1) {
// 				if (loop) {
// 					if (reverse) {
// 						m_currentTargetIndex = m_waypoints.Length - 2;
// 						incrementIndex = false;
// 					} else
// 						m_currentTargetIndex = 0;
// 				}
// 				else
// 					m_currentTargetIndex = prevWaypoint;
// 			}
// 		} else {
// 			m_currentTargetIndex--;
// 			if (m_currentTargetIndex < 0) {
// 				if (loop) {
// 					if (reverse) {
// 						m_currentTargetIndex = 1;
// 						incrementIndex = true;
// 					} else
// 						m_currentTargetIndex = m_waypoints.Length - 1;
// 				}
// 				else
// 					m_currentTargetIndex = prevWaypoint;
// 			}
// 		}

// 		return m_waypoints [m_currentTargetIndex];

// //
// //			if (m_currentTargetIndex == m_targets.Count)
// //				RandomActivity();
// //			else
// //			{
// //				Debug.Log("Target: " + (m_currentTargetIndex + 1) + "/" + m_targets.Count + " (" + m_targets[m_currentTargetIndex].transform.position + ")");
// //				m_navMeshAgent.SetDestination(m_targets[m_currentTargetIndex].transform.position);
// //			}
// 	}

// 	private IEnumerator Idle(float p_time)
// 	{
// 		Debug.Log("Idling for " + p_time + "s");
// 		yield return new WaitForSeconds(p_time);
// 		RandomActivity();
// 	}


// 	private void RandomActivity()
// 	{
// 		if (m_waypoints.Length == 0)
// 		{
// 			Debug.Log("Enemy will idle forever (no waypoints found)");
// 			StartCoroutine(Idle(Random.Range(m_minIdle, m_maxIdle)));
// 			return;
// 		}

// 		if(Random.Range(0f, 1f) <= m_patrolChance)
// 		{
// 			//Available waypoints
// 			m_availableTargets = new List<Waypoint>(m_waypoints);

// 			//Remove currentpoint
// 			if(m_targets.Count > 0)
// 				m_availableTargets.Remove(m_targets[m_targets.Count - 1]);

// 			//Reset list
// 			m_targets.Clear();
// 			m_currentTargetIndex = -1;

// 			//Add patrol points
// 			for (int i = 0; i < Random.Range(m_minPatrolPoints, m_maxPatrolPoints + 1); i++)
// 			{
// 				m_tempWaypoint = m_availableTargets[Random.Range(0, m_availableTargets.Count)];
// 				m_targets.Add(m_tempWaypoint);
// 				m_availableTargets.Remove(m_tempWaypoint);
// 			}

// //			NextWaypoint(null);
// 		}
// 		else
// 			StartCoroutine(Idle(Random.Range(m_minIdle, m_maxIdle)));
// 	}
}