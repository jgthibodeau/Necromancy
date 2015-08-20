using UnityEngine;
using System.Collections;

public class EnemyScript : SavableScript {
	//States
	public enum State{Patrol, Alert, Search, Investigate};
	public State currentState;
	public State previousState;

	//Whether to allow default behaviors or not
	public bool doDefaultPatrol = true;
	public bool doDefaultInvestigate = true;
	public bool doDefaultAlert = true;
	public bool doDefaultSearch = true;

	//Player info
	public GameObject player;
	public Vector3 lastKnownPosition;

	//Detection radiuses
	public float fov; // in degrees
	public float peripheralFov; // in degrees
	public float viewDistance;
	public float peripheralViewDistance;
	public float behindDistance;

	//Timing
	public float maxInvestigateTime;
	public float investigateTime;
	public float maxSearchTime;
	public float searchTime;

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
	public int currentWaypoint = 0;
	public bool movingForward = true;

	//Random values
	public float investigateRandomness;
	public float searchRandomness;
	
	// Use this for initialization
	void Start () {
		player = GameObject.Find ("Player");
		agent = this.GetComponent<NavMeshAgent> ();

		patrolPath.SetActive (true);
		waypoints = patrolPath.GetComponentsInChildren<Transform> ();
		patrolPath.SetActive (false);
		agent = this.transform.GetComponent<NavMeshAgent> ();
	}
	
	// Update is called once per frame
	void Update () {
		switch (currentState) {
		case State.Patrol:
			agent.speed = patrolSpeed;
			noticeTurnSpeed = patrolNoticeSpeed;

			if (CanSee (player))
				currentState = State.Alert;
			else if (CanNotice (player)){
				lastKnownPosition = player.transform.position;
				investigateTime = maxInvestigateTime;
				currentState = State.Investigate;
			}

			break;

		case State.Investigate:
			agent.speed = investigateSpeed;
			noticeTurnSpeed = investigateNoticeSpeed;

			if (CanSee (player))
				currentState = State.Alert;
			else if (CanNotice (player)){
				investigateTime = maxInvestigateTime;
				lastKnownPosition = player.transform.position;
			} else if (investigateTime > 0)
				investigateTime -= Time.deltaTime;
			else
				currentState = State.Patrol;

			break;
			
		case State.Alert:
			agent.speed = alertSpeed;
			noticeTurnSpeed = alertNoticeSpeed;

			lastKnownPosition = player.transform.position;
			if (!CanSee (player)){
				searchTime = maxSearchTime;
				currentState = State.Search;
			}

			break;
		
		case State.Search:
			agent.speed = searchSpeed;
			noticeTurnSpeed = searchNoticeSpeed;

			if (CanSee (player))
				currentState = State.Alert;
			else if (CanNotice (player)){
				searchTime = maxSearchTime;
				lastKnownPosition = player.transform.position;
			} else if (searchTime > 0)
				searchTime -= Time.deltaTime;
			else
				currentState = State.Patrol;

			break;
		}

		DoBehavior ();
	}

	public void DoBehavior(){
		switch (currentState) {
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
	
	public virtual void Patrol(){
	}
	public virtual void Alert(){
	}
	public virtual void Search(){
	}
	public virtual void Investigate(){
	}

	public void DefaultPatrol(){
		Vector3 target = waypoints [currentWaypoint].position;
		Vector3 moveDirection = target - transform.position;
		
		if (moveDirection.magnitude < 1.5) {
			//			if (curTime == 0)
			//				curTime = Time.time; // Pause over the Waypoint
			//			if ((Time.time - curTime) >= pauseDuration){
			if (movingForward) {
				currentWaypoint++;
				if (currentWaypoint > waypoints.Length - 1) {
					if (loop)
						currentWaypoint = 0;
					else {
						currentWaypoint--;
						movingForward = false;
					}
				}
			} else {
				currentWaypoint--;
				if (currentWaypoint < 0) {
					if (loop)
						currentWaypoint = waypoints.Length - 1;
					else {
						currentWaypoint++;
						movingForward = true;
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
		agent.SetDestination (waypoints[currentWaypoint].position);
	}

	public void DefaultInvestigate(){
		//TODO if reached lastknownposition, look left and right then pick a new one
		if (Vector3.Distance (transform.position, lastKnownPosition) < 1.5)
			GetNewPosition (investigateRandomness);

		Vector3 targetPoint = new Vector3(lastKnownPosition	.x, transform.position.y, lastKnownPosition.z) - transform.position;
		if (Vector3.Angle (transform.forward, targetPoint) > 10) {
			agent.ResetPath ();
			TurnTowards (lastKnownPosition, noticeTurnSpeed);
		} else {
			agent.SetDestination (lastKnownPosition);
		}
	}

	public void DefaultAlert(){
		agent.SetDestination (player.transform.position);
	}

	public void DefaultSearch(){
		//TODO if reached lastknownposition, look left and right then pick a new one
		if (Vector3.Distance (transform.position, lastKnownPosition) < 1.5)
			GetNewPosition (searchRandomness);
		agent.SetDestination (lastKnownPosition);
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
		previousState = currentState;
		currentState = state;
	}
	
	public bool CanSee(GameObject target){
		// Detect if target is within the field of view
		Vector3 direction = target.transform.position - transform.position;
		
		if((Vector3.Angle(direction, transform.forward)) < fov){
			// Detect if target within viewDistance
			RaycastHit hit;
			if (Physics.Raycast (transform.position, direction, out hit, viewDistance, GlobalScript.IgnoreInteractableLayerMask)) {
				if (hit.transform == target.transform)
					return true;
			}
		}
		return false;
	}
	
	public bool CanNotice(GameObject target){
		// Detect if target is within the peripheral view
		Vector3 direction = target.transform.position - transform.position;
		
		Debug.DrawRay(transform.position, direction, Color.red);
		
		RaycastHit hit;
		if((Vector3.Angle(direction, transform.forward)) < peripheralFov){
			// Detect if player within peripheralViewDistance
			if (Physics.Raycast (transform.position, direction, out hit, peripheralViewDistance, GlobalScript.IgnoreInteractableLayerMask)) {
				if (hit.transform == target.transform)
					return true;
			}
			return false;
		}
		
		// Detect if player is beind
		Physics.Raycast (transform.position, direction, out hit, behindDistance, GlobalScript.IgnoreInteractableLayerMask);
		if(hit.transform == target.transform){
			return true;
		}
		
		//TODO remove this once we do sound!
		return false;	
		//TODO Detect if player is heard
	}
}
