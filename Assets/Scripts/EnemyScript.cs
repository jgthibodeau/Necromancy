using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;
using System.Collections.Generic;

public delegate void BehaviorDelegate ();

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
	public bool seenPlayer;
	public int timesInvestigateTriggered = 0;
	
	public EnemyData () : base () {}
	public EnemyData (SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) {}
}

[System.Serializable]
public class EnemyStateVariables{
	public float speed = 2f;
	public float turnSpeed = 2f;
	public float minDetectionLight;
	public float minDetectionSpeed;
	public float visionSensitivity = 1f;
	public float visibleLightValue = 10f;
	public float motionSensitivity = 1f;
	public float soundSensitivity = 1f;
	public float ignorableSoundLevel = 0f;
	public float maxInterest = 100f;
	public float highInterestRate = 100f;
	public float interestDecreaseRate = 1f;
}

public class EnemyScript : SavableScript {
	public EnemyData enemydata;

	//States
	public enum State{Patrol, Cautious, Investigate, Alert, Hunt};
	bool StateChanged;
	bool doDefault;
	BehaviorDelegate defaultBehavior;
	BehaviorDelegate customBehavior;

	//Whether to allow default behaviors or not
	public bool doDefaultPatrol = true;
	public bool doDefaultInvestigate = true;
	public bool doDefaultCautious = true;
	public bool doDefaultAlert = true;
	public bool doDefaultHunt = true;

	//State variables
	public EnemyStateVariables patrolVars = new EnemyStateVariables();
	public EnemyStateVariables cautiousVars = new EnemyStateVariables();
	public EnemyStateVariables investigateVars = new EnemyStateVariables();
	public EnemyStateVariables alertVars = new EnemyStateVariables();
	public EnemyStateVariables huntVars = new EnemyStateVariables();
	private EnemyStateVariables currentVars;

	//Player info
	public GameObject player;

	//Timing
	public float maxInvestigateTime;
	public float maxSearchTime;

	//Agent and state speeds
	public NavMeshAgent agent;

	//Patroling waypoints
	public GameObject patrolPath;
	public WayPoint[] waypoints;
	public bool loop = true;

	//Random values
	public float investigateRandomness;
	public float searchRandomness;

	//detection stuff
	public EntityDetector detector;
	public SoundDetector soundDetector;

	public GameObject highestPriorityVisibleEntity;
	public float highestPriorityVisibility;
	public GameObject highestPriorityInterestingEntity;
	public float highestPriorityInterest;
	public SoundData highestPrioritySound;
	public float highestPriorityVolume;

	public int maxInvestigations = 3;
	public float interestRate = 0f;
	
	// Use this for initialization
	protected virtual void Start () {
		player = GameObject.Find ("Player");
		agent = this.GetComponent<NavMeshAgent> ();

		ChangeState (State.Patrol);

		patrolPath.SetActive (true);

		waypoints = patrolPath.GetComponentsInChildren<WayPoint> ();
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

		//get new interest from entity senses
		float oldInterest = enemydata.interestLevel;
		interestRate = Look() + Listen();

		enemydata.interestLevel =  Mathf.Max (enemydata.interestLevel + interestRate, enemydata.interestLevel);

		//if no new interest, relax interest
		if (interestRate == 0)
			RelaxInterest ();

		//do behavior, and repeat without changing sensed entities until we settle on a full behavior
		do {
			StateChanged = false;
			if (doDefault)
				defaultBehavior ();
			else
				customBehavior ();
		} while(StateChanged);
			
		DebugExtension.DebugWireSphere (agent.destination, Color.red);
	}
	
	public virtual void Patrol(){}
	public virtual void Cautious(){}
	public virtual void Investigate(){}
	public virtual void Alert(){}
	public virtual void Hunt(){}

	public void DefaultPatrol(){
		//if player directly visible, go to alert state
		if(highestPriorityVisibility > currentVars.visibleLightValue){
			ChangeState (State.Alert);
			return;
		}

		//if interestRate too high, go to investigate state
		//TODO base this off times investigated too
		if (interestRate >= currentVars.highInterestRate) {
			ChangeState (State.Investigate);
			return;
		}

		//if interest too high, go to cautious state
		//TODO base this off times investigated too
		if (enemydata.interestLevel >= currentVars.maxInterest) {
			ChangeState (State.Cautious);
			return;
		}

		//regular patrol
		WayPoint wayPoint = waypoints [enemydata.currentWaypoint];
		Vector3 target = wayPoint.transform.position;
		Vector3 moveDirection = target - transform.position;

		//if at current waypoint, 
		if (moveDirection.magnitude < 1.5) {
			//TODO pause for a time, and look around if need to
//			PauseAndLook (wayPoint.pauseTime, wayPoint.lookAround);

			//set to next waypoint
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

			wayPoint = waypoints [enemydata.currentWaypoint];
		}

		//move towards current waypoint
		agent.SetDestination (wayPoint.transform.position);
	}

	public void DefaultCautious(){
		//if player directly visible, go to alert state
		if(highestPriorityVisibility > currentVars.visibleLightValue){
			ChangeState (State.Alert);
			return;
		}

		//if interest too high, or built up rapidly go to investigate state
		if (enemydata.interestLevel >= currentVars.maxInterest || interestRate >= currentVars.highInterestRate) {
			ChangeState (State.Investigate);
			return;
		}

		//if interest has dropped enough, go to patrol state
//		if (enemydata.interestLevel == 0) {
//			ChangeState (State.Patrol);
//			return;
//		}

		//else, patrol but stop every so often to look around based off caution level



		//		//TODO if reached lastknownposition, look left and right then pick a new one
		//		if (Vector3.Distance (transform.position, enemydata.lastKnownPosition) < 1.5)
		//			GetNewPosition (searchRandomness);
		//		agent.SetDestination (enemydata.lastKnownPosition);
	}

	public void DefaultInvestigate(){
		//if player directly visible or investigate triggered enough, go to alert state
		if(highestPriorityVisibility > currentVars.visibleLightValue || enemydata.timesInvestigateTriggered > maxInvestigations){
			ChangeState (State.Alert);
			return;
		}

		//if interest has dropped enough, go to cautious state
//		if (enemydata.interestLevel == 0) {
//			ChangeState (State.Cautious);
//			return;
//		}

		//if not at interesting location, move towards interesting location
		//else, look around for some time, choose a new interesting location, or return to cautious state




//		//TODO if reached lastknownposition, look left and right then pick a new one
//		if (Vector3.Distance (transform.position, enemydata.lastKnownPosition) < 1.5)
//			GetNewPosition (investigateRandomness);
//
//		Vector3 targetPoint = new Vector3(enemydata.lastKnownPosition	.x, transform.position.y, enemydata.lastKnownPosition.z) - transform.position;
//		if (Vector3.Angle (transform.forward, targetPoint) > 10) {
//			agent.ResetPath ();
//			TurnTowards (enemydata.lastKnownPosition, noticeTurnSpeed);
//		} else {
//			agent.SetDestination (enemydata.lastKnownPosition);
//		}
	}

	public void DefaultAlert(){
		//on first time here, alert other guards in vicinity
		//after alerting other guards, go to hunt state



//		agent.SetDestination (player.transform.position);
	}

	public void DefaultHunt(){
		//if player directly visible
			//move towards player
			//update last known position

			//TODO if at player, do something
		//else
			//move towards last known position
			//if at last known position
				//TODO sweep the area
	}

	public void PauseAndLook(float time, bool look){
	}

	public void GetRandomPosition(float randomness){
		//TODO
//		lastKnownPosition = transform.forward;
//		lastKnownPosition += new Vector3 (lastKnownPosition.x + Random.Range (-randomness, randomness),
//		                                lastKnownPosition.y,
//		                                 lastKnownPosition.z + Random.Range (-randomness, randomness));
	}

	public float Look(){
		float newInterest = 0f;
		highestPriorityVisibleEntity = null;
		highestPriorityVisibility = 0f;
		highestPriorityInterestingEntity = null;
		highestPriorityInterest = 0f;

		int highestVisiblePriority = 0;

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
			float lightValue = 0f;
			if (dd.isDirect ()) {
				//amount to add is porportional to light, sensitivity, amount of entity visible, distance to entity, and cone sensitivity
				lightValue = lightLevel * currentVars.visionSensitivity * dd.percentVisible * distancePercent * visionCone.sensitivity * Time.deltaTime;
				newInterest += lightValue;

				//if lightValue is high enough, entity is visible
				//if priority is higher than current highest priority visible entity
				Debug.Log(lightValue+" "+currentVars.visibleLightValue+" "+dd.priority+" "+highestVisiblePriority);
				if(lightValue >= currentVars.visibleLightValue && dd.priority > highestVisiblePriority){
					//set highest priority visible entity to this one
					highestVisiblePriority = dd.priority;
					highestPriorityVisibility = lightValue;
					highestPriorityVisibleEntity = go;
				}
			}

			//add interest based on motion
			//amount to add is porportional to light, sensitivity, amount of entity visible, speed, distance to entity, and cone sensitivity
			float motionValue = lightLevel * currentVars.motionSensitivity * dd.percentVisible * relativeSpeed * distancePercent * visionCone.sensitivity * Time.deltaTime;
			newInterest += motionValue;

			//if this entity has generated the most interest so far
			if(lightValue + motionValue > highestPriorityInterest){
				//set highest priority interesting entity to this one
				highestPriorityInterest = lightValue + motionValue;
				highestPriorityInterestingEntity = go;
			}
		}

		return newInterest;
	}

	public float Listen(){
		float newInterest = 0f;
		highestPrioritySound = null;
		highestPriorityVolume = 0f;

		foreach (SoundData sd in soundDetector.sounds) {
			if (sd.volume > currentVars.ignorableSoundLevel) {
				//				newInterest += maxInterest;
				newInterest += sd.volume * currentVars.soundSensitivity;

				//if this sound is loudest
				if (newInterest > highestPriorityVolume) {
					//set highestPrioritySound to this one
					highestPriorityVolume = newInterest;
					highestPrioritySound = sd;
				}
			}
		}
		soundDetector.sounds.Clear ();

		return newInterest;
	}

	public void RelaxInterest(){
		enemydata.interestLevel = Mathf.Max (enemydata.interestLevel - currentVars.interestDecreaseRate * Time.deltaTime, 0);
	}

	public void TurnTowards(Vector3 target, float speed){
		Vector3 targetPoint = target - transform.position;
		Quaternion targetRotation = Quaternion.LookRotation (targetPoint, Vector3.up);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
	}
	
	public void ChangeState(State state){
		StateChanged = true;

		enemydata.previousState = enemydata.currentState;
		enemydata.currentState = state;
		enemydata.interestLevel = 0;

		//set vars correctly
		switch (enemydata.currentState) {
		case State.Patrol:
			currentVars = patrolVars;
			agent.speed = patrolVars.speed;

			doDefault = doDefaultPatrol;
			defaultBehavior = DefaultPatrol;
			customBehavior = Patrol;

			break;
		case State.Cautious:
			currentVars = cautiousVars;
			agent.speed = cautiousVars.speed;

			doDefault = doDefaultCautious;
			defaultBehavior = DefaultCautious;
			customBehavior = Cautious;
			break;
		case State.Investigate:
			currentVars = investigateVars;
			agent.speed = investigateVars.speed;

			doDefault = doDefaultInvestigate;
			defaultBehavior = DefaultInvestigate;
			customBehavior = Investigate;

			enemydata.timesInvestigateTriggered++;
			break;
		case State.Alert:
			currentVars = alertVars;
			agent.speed = alertVars.speed;

			doDefault = doDefaultAlert;
			defaultBehavior = DefaultAlert;
			customBehavior = Alert;
			break;
		case State.Hunt:
			currentVars = huntVars;
			agent.speed = huntVars.speed;

			doDefault = doDefaultHunt;
			defaultBehavior = DefaultHunt;
			customBehavior = Hunt;
			break;
		}
	}
}
