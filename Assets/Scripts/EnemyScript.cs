using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;
using System.Collections.Generic;

[System.Serializable]
public class AwarenessLevel{
	public float visibility = 0f;
	public float noticeability = 0f;

	public void resetVisibility(){
		visibility = 0f;
	}

	public void resetNoticeability(){
		noticeability = 0f;
	}
}


[System.Serializable]
public class EnemyData : SaveData{
	public EnemyScript.State currentState;
	public EnemyScript.State previousState;
	public Vector3 lastKnownPosition;
	public float investigateTime;
	public float searchTime;
	public bool loop;
	public int currentWaypoint;
	public bool movingForward = true;
	public Hashtable awarenessLevels = new Hashtable();
	
	public EnemyData () : base () {}
	public EnemyData (SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) {}
}

public class EnemyScript : SavableScript {
	public EnemyData enemydata;

	//States
	public enum State{Patrol, Alert, Search, Investigate};

	//Whether to allow default behaviors or not
	public virtual bool doDefaultPatrol {get { return true; } }
	public virtual bool doDefaultInvestigate {get { return true; } }
	public virtual bool doDefaultAlert {get { return true; } }
	public virtual bool doDefaultSearch {get { return true; } }

	//Player info
	public GameObject player;

	//Detection radiuses
	public FieldOfViewScript fovScript;

	//Timing
	public float maxInvestigateTime;
	public float maxSearchTime;

	//Internally set speeds
	public float noticeTurnSpeed;
	public float alertedTurnSpeed;

	//Agent and state speeds
	public NavMeshAgent agent;
	public float patrolSpeed;
	public float alertSpeed;
	public float searchSpeed;
	public float investigateSpeed;
	
	public float patrolNoticeSpeed;
	public float alertNoticeSpeed;
	public float searchNoticeSpeed;
	public float investigateNoticeSpeed;

	//Patroling waypoints
	public GameObject patrolPath;
	public Transform[] waypoints;
	public bool loop = true;

	//Random values
	public float investigateRandomness;
	public float searchRandomness;

	//detection stuff
	public EntityDetector detector;
	public float visibilityScale = 1f;
	public float noticeabilityScale = 1f;
	
	// Use this for initialization
	protected virtual void Start () {
		player = GameObject.Find ("Player");
		agent = this.GetComponent<NavMeshAgent> ();

		fovScript = this.GetComponent<FieldOfViewScript> ();

		patrolPath.SetActive (true);

		List<Transform> waypointList = new List<Transform> ();
		waypoints = patrolPath.GetComponentsInChildren<Transform> ();
		foreach(Transform child in waypoints){
			if(child.CompareTag("WayPoint"))
				waypointList.Add(child.transform);
		}
		waypoints = waypointList.ToArray ();
		patrolPath.SetActive (false);
		agent = this.transform.GetComponent<NavMeshAgent> ();

		detector = GetComponentInChildren<EntityDetector> ();

		savedata = enemydata;
	}
	
	void Update () {
		if (GlobalScript.currentGameState == GlobalScript.GameState.InGame)
			InGame ();
	}
	
	void InGame () {
		enemydata = (EnemyData)savedata;

		DetectEntities ();

		switch (enemydata.currentState) {
		case State.Patrol:
			agent.speed = patrolSpeed;
			noticeTurnSpeed = patrolNoticeSpeed;

			if (CanSee (player))
				enemydata.currentState = State.Alert;
			else if (CanNotice (player)){
				enemydata.lastKnownPosition = player.transform.position;
				enemydata.investigateTime = maxInvestigateTime;
				enemydata.currentState = State.Investigate;
			}

			break;

		case State.Investigate:
			agent.speed = investigateSpeed;
			noticeTurnSpeed = investigateNoticeSpeed;

			if (CanSee (player))
				enemydata.currentState = State.Alert;
			else if (CanNotice (player)){
				enemydata.investigateTime = maxInvestigateTime;
				enemydata.lastKnownPosition = player.transform.position;
			} else if (enemydata.investigateTime > 0)
				enemydata.investigateTime -= Time.deltaTime;
			else
				enemydata.currentState = State.Patrol;

			break;
			
		case State.Alert:
			agent.speed = alertSpeed;
			noticeTurnSpeed = alertNoticeSpeed;

			enemydata.lastKnownPosition = player.transform.position;
			if (!CanSee (player)){
				enemydata.searchTime = maxSearchTime;
				enemydata.currentState = State.Search;
			}

			break;
		
		case State.Search:
			agent.speed = searchSpeed;
			noticeTurnSpeed = searchNoticeSpeed;

			if (CanSee (player))
				enemydata.currentState = State.Alert;
			else if (CanNotice (player)){
				enemydata.searchTime = maxSearchTime;
				enemydata.lastKnownPosition = player.transform.position;
			} else if (enemydata.searchTime > 0)
				enemydata.searchTime -= Time.deltaTime;
			else
				enemydata.currentState = State.Patrol;

			break;
		}

		DoBehavior ();
	}

	public void DoBehavior(){
		switch (enemydata.currentState) {
		case State.Patrol:
			if(doDefaultPatrol)
				DefaultPatrol ();
			Patrol ();
			break;
		case State.Investigate:
			if(doDefaultInvestigate)
				DefaultInvestigate ();
			Investigate ();
			break;
		case State.Alert:
			if(doDefaultAlert)
				DefaultAlert ();
			Alert ();
			break;
		case State.Search:
			if(doDefaultSearch)
				DefaultSearch ();
			Search ();
			break;
		}
	}
	
	public virtual void Patrol(){}
	public virtual void Alert(){}
	public virtual void Search(){}
	public virtual void Investigate(){}

	public void DetectEntities(){
		//foreach entity visible to this guard
		foreach (GameObject go in detector.entities.Keys) {
			/*Raycast to make sure we have line of sight to the entity in question before adding values*/
			Bounds bounds = go.GetComponent<Collider> ().bounds;
			Vector3[] boundPoints = new Vector3[5];

			Vector3 eyeheight = transform.FindChild ("Eyes").transform.position;

			//top and bottom
			boundPoints [0] = bounds.center + go.transform.up * (bounds.size.y / 2f - .1f);
			boundPoints [1] = bounds.center + go.transform.up * (bounds.size.y / -2f + .1f);

			//left and right
			Vector3 directionToCenter = eyeheight - bounds.center;
			boundPoints [2] = Vector3.Cross(directionToCenter, go.transform.up);
			boundPoints [2] *= (bounds.size.x / 2f - .1f) / (boundPoints [2].magnitude);
			boundPoints [2] += bounds.center;

			boundPoints [3] = Vector3.Cross(go.transform.up, directionToCenter);
			boundPoints [3] *= (bounds.size.x / 2f - .1f) / (boundPoints [3].magnitude);
			boundPoints [3] += bounds.center;

			//front
			boundPoints [4] = directionToCenter;
			boundPoints [4] *= (bounds.size.x / 2f - .1f) / (boundPoints [4].magnitude);
			boundPoints [4] += bounds.center;

			//raycast to all points
			int totalAgree = 0;
			foreach(Vector3 point in boundPoints){
				//draw line onto entities collider
				Debug.DrawLine (bounds.center, point, Color.magenta);

				//check for visibility from us to entity
				RaycastHit hit;
				if (Physics.Linecast (eyeheight, point, out hit)) {
					if (hit.transform == go.transform) {
						Debug.DrawLine (eyeheight, point, Color.blue);
						totalAgree++;
					}
					else{
						Debug.DrawLine (eyeheight, point, Color.red);
					}
				}
			}

			//entity is visible if majority agree
			if (totalAgree < 5) {
				break;
			}

			Debug.Log (go);

			/*incremement awareness about the entity*/
			//get awareness level for this object, creating it if it doesnt exist
			AwarenessLevel currentAwareness;
			if (!enemydata.awarenessLevels.Contains (go))
				enemydata.awarenessLevels.Add (go, new AwarenessLevel ());
			currentAwareness = (AwarenessLevel) enemydata.awarenessLevels [go];

			//if gameobject has a direct cone, add visibility based on light level
			bool hasDirect = false;
			foreach (VisionCone cone in (List<VisionCone>)detector.entities[go]) {
				if (cone.type == VisionCone.Type.Direct) {
					hasDirect = true;
					break;
				}
			}
			if (hasDirect) {
//				currentAwareness.visibility += Mathf.Clamp (go.lightLevel * visibilityScale * Time.deltaTime, 0, 100);
			}

			//add noticeability based on motion vector
//			currentAwareness.noticeability +=Mathf.Clamp (go.speed * noticeabilityScale * Time.deltaTime, 0, 100);


			/*react to detecting entity*/
			//if entity surpassed visual threshold
			//go to a chasing behavior
			//if entity surpassed peripheral threshold
			//go to an investigation behavior
		}
	}

	public void DefaultPatrol(){
		Vector3 target = waypoints [enemydata.currentWaypoint].position;
		Vector3 moveDirection = target - transform.position;
		
		if (moveDirection.magnitude < 1.5) {
			//			if (curTime == 0)
			//				curTime = Time.time; // Pause over the Waypoint
			//			if ((Time.time - curTime) >= pauseDuration){
			if (enemydata.movingForward) {
				enemydata.currentWaypoint++;
				if (enemydata.currentWaypoint > waypoints.Length - 1) {
					if (loop)
						enemydata.currentWaypoint = 0;
					else {
						enemydata.currentWaypoint--;
						enemydata.movingForward = false;
					}
				}
			} else {
				enemydata.currentWaypoint--;
				if (enemydata.currentWaypoint < 0) {
					if (loop)
						enemydata.currentWaypoint = waypoints.Length - 1;
					else {
						enemydata.currentWaypoint++;
						enemydata.movingForward = true;
					}
				}
			}
			//				curTime = 0;
			//			}
		}//else{        
		//			var rotation = Quaternion.LookRotation(target - transform.position);
		//			transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * dampingLook);
		//			character.Move(moveDirection.normalized * patrolSpeed * Time.deltaTime);
		//		} 
		agent.SetDestination (waypoints[enemydata.currentWaypoint].position);
	}

	public void DefaultInvestigate(){
		//TODO if reached lastknownposition, look left and right then pick a new one
		if (Vector3.Distance (transform.position, enemydata.lastKnownPosition) < 1.5)
			GetNewPosition (investigateRandomness);

		Vector3 targetPoint = new Vector3(enemydata.lastKnownPosition	.x, transform.position.y, enemydata.lastKnownPosition.z) - transform.position;
		if (Vector3.Angle (transform.forward, targetPoint) > 10) {
			agent.ResetPath ();
			TurnTowards (enemydata.lastKnownPosition, noticeTurnSpeed);
		} else {
			agent.SetDestination (enemydata.lastKnownPosition);
		}
	}

	public void DefaultAlert(){
		agent.SetDestination (player.transform.position);
	}

	public void DefaultSearch(){
		//TODO if reached lastknownposition, look left and right then pick a new one
		if (Vector3.Distance (transform.position, enemydata.lastKnownPosition) < 1.5)
			GetNewPosition (searchRandomness);
		agent.SetDestination (enemydata.lastKnownPosition);
	}

	public void GetNewPosition(float randomness){
		//TODO
//		lastKnownPosition = transform.forward;
//		lastKnownPosition += new Vector3 (lastKnownPosition.x + Random.Range (-randomness, randomness),
//		                                lastKnownPosition.y,
//		                                 lastKnownPosition.z + Random.Range (-randomness, randomness));
	}

	public void TurnTowards(Vector3 target, float speed){
		Vector3 targetPoint = new Vector3(target.x, transform.position.y, target.z) - transform.position;
		Quaternion targetRotation = Quaternion.LookRotation (targetPoint, Vector3.up);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
	}
	
	public void ChangeState(State state){
		enemydata.previousState = enemydata.currentState;
		enemydata.currentState = state;
	}
	
	public bool CanSee(GameObject target){
//		// Detect if target is within the field of view
//		Vector3 direction = target.transform.position - transform.position;
//		
//		if((Vector3.Angle(direction, transform.forward)) < fovScript.frontFov){
//			// Detect if target within viewDistance
//			RaycastHit hit;
//			if (Physics.Raycast (transform.position, direction, out hit, fovScript.frontViewDistance, GlobalScript.IgnoreInteractableLayerMask)) {
//				if (hit.transform == target.transform)
//					return true;
//			}
//		}
		return false;
	}
	
	public bool CanNotice(GameObject target){
//		// Detect if target is within the peripheral view
//		Vector3 direction = target.transform.position - transform.position;
//		
////		Debug.DrawRay(transform.position, direction, Color.red);
//		
//		RaycastHit hit;
//		if((Vector3.Angle(direction, transform.forward)) < fovScript.peripheralFov){
//			// Detect if player within peripheralViewDistance
//			if (Physics.Raycast (transform.position, direction, out hit, fovScript.peripheralViewDistance, GlobalScript.IgnoreInteractableLayerMask)) {
//				if (hit.transform == target.transform)
//					return true;
//			}
//			return false;
//		}
//		
//		// Detect if player is beind
//		Physics.Raycast (transform.position, direction, out hit, fovScript.backViewDistance, GlobalScript.IgnoreInteractableLayerMask);
//		if(hit.transform == target.transform){
//			return true;
//		}
		
		//TODO remove this once we do sound!
		return false;	
		//TODO Detect if player is heard
	}
}
