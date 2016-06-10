using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;
using System.Collections.Generic;

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

	public float interestLevel = 0f;
	public Vector3 interestingLocation;
	public bool interestPiqued;
	public int timesInterestPiqued;
	public bool seenPlayer;
	
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
	public float minLightLevel = 0f;
	public float minSpeed = 0f;
	public float visionSensitivity = 1f;
	public float motionSensitivity = 1f;

	public SoundDetector soundDetector;
	public float ignorableSoundLevel = 0f;

	public int maxTimesInterestPiqued = 3;
	public float maxInterest = 100f;
	public float highInterestRate = 100f;
	
	// Use this for initialization
	protected virtual void Start () {
		player = GameObject.Find ("Player");
		agent = this.GetComponent<NavMeshAgent> ();

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
		soundDetector = GetComponentInChildren<SoundDetector> ();

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
		float newInterest = 0f;
		GameObject highestPriorityEntity;
		int highestPriority = 99;

		//foreach entity visible to this guard
		foreach (GameObject go in detector.entities.Keys) {
			DetectionData dd = (DetectionData)detector.entities [go];

			/*increase awareness about the entity*/

			//get light, distance, and motion data about entity
			float lightLevel = go.GetComponent<LightLevel> ().level;
			float distance = Vector3.Distance (transform.position, go.transform.position);
			VisionCone visionCone = dd.HighestPriorityCone ();
			float distancePercent = distance / visionCone.length;

//			TODO speed = magnitude of component of motion vector perpendicular to us, clamped
//			Debug.DrawRay (go.transform.position, go.GetComponent<Rigidbody> ().velocity, Color.magenta);
//
//			Vector3 direction = go.transform.position - transform.position;
//			Debug.DrawRay (go.transform.position, direction, Color.green);
//
//			Vector3 movementPerpendicular = Vector3.ProjectOnPlane (go.GetComponent<Rigidbody> ().velocity, direction);
//			Debug.DrawRay (go.transform.position, movementPerpendicular, Color.cyan);
//
//			float relativeSpeed = movementPerpendicular.magnitude;
			float relativeSpeed = go.GetComponent<Rigidbody> ().velocity.magnitude;

			//if entity is directly visible, add interest based on light level only
			if (dd.isDirect ()) {
				//amount to add is porportional to light, sensitivity, amount of entity visible, distance to entity, and cone sensitivity
				newInterest += lightLevel * visionSensitivity * dd.percentVisible * distancePercent * visionCone.sensitivity;

				//TODO if light level high enough and distance low enough, we have seen the entity
			}

			//add interest based on motion
			//amount to add is porportional to light, sensitivity, amount of entity visible, speed, distance to entity, and cone sensitivity
			newInterest += lightLevel * motionSensitivity * dd.percentVisible * relativeSpeed * distancePercent * visionCone.sensitivity;

			//TODO set highestPriorityInterestingEntity if priority of object is highest
			if (dd.priority < highestPriority)
				highestPriorityEntity = go;
		}

		//TODO add interest based on sounds
		foreach (SoundData sd in soundDetector.sounds) {
			if (sd.volume > ignorableSoundLevel) {
				newInterest += maxInterest;
				//TODO set highestPriorityInterestingEntity if priority of object is highest
			}
			soundDetector.RemoveSound (sd);
		}

		//TODO decrease interest if nothing interesting is happening
//		if(newInterest = 0)

		//add new interest
		float oldInterest = enemydata.interestLevel;
		enemydata.interestLevel = Mathf.Clamp (enemydata.interestLevel + newInterest, 0, maxInterest);

		//if interest is newly full, and interest not currently piqued, then interest has been piqued
		//or if newInterest is high enough, and interest is currently piqued, interest has been piqued again
		if ((oldInterest != enemydata.interestLevel && enemydata.interestLevel == maxInterest && !enemydata.interestPiqued) 
			|| (newInterest >= highInterestRate && enemydata.interestPiqued)) {
			enemydata.interestPiqued = true;
			enemydata.timesInterestPiqued = Mathf.Clamp (enemydata.timesInterestPiqued++, 0, maxTimesInterestPiqued);
		}
		//if newInterest is high enough, and interest is currently piqued, interest has been piqued again

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
