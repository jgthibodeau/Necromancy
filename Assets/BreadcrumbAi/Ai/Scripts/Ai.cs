using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BreadcrumbAi{
	[System.Serializable]
	public class Ai : MonoBehaviour {
		
		#region Editor Variables
		// *****EDITOR VARIABLES***** //
		public bool _CanFollowPlayer, _CanFollowBreadcrumbs, _CanFollowAi, _CanWander, _CanPatrol, _IsEnemy,
					_CanIgnoreAi, // coming soon
					_CanWanderAnywhere,
					_CanHover,
					_CanFlee, _CanJump, _CanLongJump, _IsJumping,
					_HasAvoidance, _HasEdgeAvoidance,
					_HasVision, _HasFrontVision,
					_IsMelee, _IsRanged,
					_IsGround, _IsAir,
					_IsInvincible;
					
		public float followSpeed, wanderSpeed, patrolSpeed, rotationSpeed, gravity, drag, avoidSpeed,
					 jumpDistance, jumpForce, longJumpForce,
					 followDistance, wanderDistance, attackDistance, visionDistance, avoidDistance, edgeDistance, otherAiDistance,
					 wanderTimeLimit, wanderTimeRate,
					 hoverHeight, hoverForce,
					 Health;
		#endregion
		
		// States are used for adding actions, animations, sounds, etc to your Ai.
		#region STATES
		public enum LIFE_STATE{
			IsAlive,
			IsDazed,	// coming soon
			IsDead,
			IsInvincible};
		public LIFE_STATE lifeState = LIFE_STATE.IsAlive;
		
		public enum VISION_STATE{
			CanSeeNothing,
			CanSeePlayer,
			CanSeeTarget,
			CanSeeBreadcrumb,
			CanSeeFollowAi,
			CanSeeFollowAiTierTwo,
			CanSeeWaypoint};
		public VISION_STATE visionState = VISION_STATE.CanSeeNothing;
		
		public enum MOVEMENT_STATE{
			IsIdle,
			IsFollowingPlayer,
			IsFollowingTarget,
			IsFollowingBreadcrumb,
			IsFollowingAi,
			IsFollowingAiTierTwo,
			IsWandering,
			IsPatrolling,
			IsAttacking
		};
		public MOVEMENT_STATE moveState = MOVEMENT_STATE.IsIdle;
		
		public enum ATTACK_STATE{
			CanNotAttack,
			CanAttack,
			CanAttackOther};	// coming soon
		public ATTACK_STATE attackState = ATTACK_STATE.CanNotAttack;
		#endregion
		
		// GAMEOBJECTS
		[HideInInspector]
		public Transform Player,				// Targeted Player (works with multiple players)
						 Target,
						 FollowingTarget,
						 FollowingPlayer,		// Last Targeted Player
						 Breadcrumb,			// Closest Located Breadcrumb
						 FollowingAi,			// Ai To Follow
						 Waypoint,				// Current Waypoint Targeted
						 Hover,					// Hover Start Pos
						 Edge,					// Edge Avoidance Start Pos
						 LongJumpDetector,      // Jump to start Pos
						 JumpDetector;			// Jump detector
		
		// LAYERS
		[HideInInspector]
		public LayerMask  playerLayer, 		// Layer : Player
						enemyLayer, 		// Layer : Enemy
						friendlyLayer, 		// Layer : Friendly
						  breadcrumbLayer,	// Layer : Breadcrumb
						  waypointLayer,	// Layer : Waypoint
						  obstacleLayer;	// Layer : Obstacle
		
		// TAG STRINGS
		[HideInInspector]

	
		// PRIVATE VARIABLES
		private bool _IsWandering;
		private bool _IsAvoiding;				// Used for avoidance, removes velocity after avoidance
		private bool _HasWanderPos;
		private Vector3 currentWanderPos;
		private Vector3 wanderPos;				// Sets next random wander position
		private float wanderTimer, wanderNext;	// Used for timing the wander time limit
		private RaycastHit hit;
		
		
	
		void Start(){
			StartCoroutine(this.Ai_Lists());
			StartCoroutine(this.Ai_Layers());
//			GetComponent<Rigidbody> ().centerOfMass = Vector3.zero;
		}
	
		void Update(){
			Ai_LifeState();
			if(IsGrounded()){
				_IsJumping = false;
			}
		}
	
		void FixedUpdate (){
			GetComponent<Rigidbody> ().AddForce (Vector3.down * gravity);

			Ai_Controller(); 	// Controls Ai Movement & Attack States
			Ai_Avoidance(~(breadcrumbLayer | enemyLayer | playerLayer | waypointLayer | friendlyLayer));	// Controls Ai wall avoidance
			Ai_Hover();

			Vector3 velocity = GetComponent<Rigidbody> ().velocity;
			velocity.x *= drag;
			velocity.z *= drag;
			GetComponent<Rigidbody> ().velocity = velocity;
		}
				
		private void Ai_Controller(){
			Vector3 movement = Vector3.zero;

//			GetComponent<Rigidbody> ().AddForce (Vector3.down * 10);

			//TODO refactor to allow for following other entities and attacking them
			if (this.Ai_FindTarget ()) {
				Debug.Log ("found target " + Target);
				_HasWanderPos = false; // TODO: this needs to be fixed
				Debug.DrawLine(transform.position, Target.position, Color.red);
				visionState = VISION_STATE.CanSeeTarget;

				if (moveState == MOVEMENT_STATE.IsAttacking) {
					return;
				}
				// CHANGE THIS TO FLEE (_CanFlee)
				if(_IsRanged){ // Is this a ranged ground unit?
					if(Vector3.Distance(transform.position,Target.position) > followDistance){
						moveState = MOVEMENT_STATE.IsFollowingTarget;
						attackState = ATTACK_STATE.CanNotAttack;
						movement = Ai_Movement(Target.position, followSpeed);
					} else if(_CanFlee && Vector3.Distance(transform.position,Target.position) <= attackDistance) {
						moveState = MOVEMENT_STATE.IsFollowingTarget;
						attackState = ATTACK_STATE.CanNotAttack;
						movement = Ai_Flee_Target();
					} else {
						moveState = MOVEMENT_STATE.IsIdle;
						attackState = ATTACK_STATE.CanAttack;
						Ai_Rotation(Target.position);
					}
				} else if(_IsMelee){ // Is this a melee ground unit?
					if(Vector3.Distance(transform.position,Target.position) > followDistance){
						moveState = MOVEMENT_STATE.IsFollowingTarget;
						attackState = ATTACK_STATE.CanNotAttack;
						movement = Ai_Movement(Target.position, followSpeed);
					} else if(Vector3.Distance(transform.position,Target.position) <= attackDistance) {
						moveState = MOVEMENT_STATE.IsIdle;
						attackState = ATTACK_STATE.CanAttack;
						Ai_Rotation(Target.position);
					}
				}

			// Checks if following player is enabled and a player has been found	
//			if(_CanFollowPlayer && this.Ai_FindPlayer()){
//				_HasWanderPos = false; // TODO: this needs to be fixed
//				visionState = VISION_STATE.CanSeePlayer;
//				
//				
//				// CHANGE THIS TO FLEE (_CanFlee)
//				if(_IsRanged){ // Is this a ranged ground unit?
//					if(Vector3.Distance(transform.position,Player.position) > followDistance){
//						moveState = MOVEMENT_STATE.IsFollowingPlayer;
//						attackState = ATTACK_STATE.CanNotAttack;
//						Ai_Movement(Player.position, followSpeed);
//					} else if(_CanFlee && Vector3.Distance(transform.position,Player.position) <= attackDistance) {
//						moveState = MOVEMENT_STATE.IsFollowingPlayer;
//						attackState = ATTACK_STATE.CanNotAttack;
//						Ai_Flee();
//					} else {
//						moveState = MOVEMENT_STATE.IsIdle;
//						attackState = ATTACK_STATE.CanAttack;
//						Ai_Rotation(Player.position);
//					}
//				} else if(_IsMelee){ // Is this a melee ground unit?
//					if(Vector3.Distance(transform.position,Player.position) > followDistance){
//						moveState = MOVEMENT_STATE.IsFollowingPlayer;
//						attackState = ATTACK_STATE.CanNotAttack;
//						Ai_Movement(Player.position, followSpeed);
//					} else if(Vector3.Distance(transform.position,Player.position) <= attackDistance) {
//						moveState = MOVEMENT_STATE.IsIdle;
//						attackState = ATTACK_STATE.CanAttack;
//						Ai_Rotation(Player.position);
//					}
//				}
//				Debug.DrawLine(transform.position, Player.position, Color.red);
				
			// Checks if following breadcrumbs is enabled as well as if a player was spotted and a breadcrumb has been found
			} else if(_CanFollowBreadcrumbs && FollowingPlayer && this.Ai_FindBreadcrumb()){
				Debug.Log ("Following breadcrumbs");
				_HasWanderPos = false; // TODO: this needs to be fixed
				visionState = VISION_STATE.CanSeeBreadcrumb;
				moveState = MOVEMENT_STATE.IsFollowingBreadcrumb;
				attackState = ATTACK_STATE.CanNotAttack;
				movement = Ai_Movement(Breadcrumb.position, followSpeed);
				Debug.DrawLine(transform.position, Breadcrumb.position, Color.green);
				
			// Checks if following other ai is enabled and if an ai has been found
			} else if(_CanFollowAi && this.Ai_FindAi()){
				Debug.Log ("Following ai " + FollowingAi);
				_HasWanderPos = false; // TODO: this needs to be fixed
				visionState = VISION_STATE.CanSeeFollowAi;
				moveState = MOVEMENT_STATE.IsFollowingAi;
				attackState = ATTACK_STATE.CanNotAttack;
				if(Vector3.Distance(transform.position, FollowingAi.position) > otherAiDistance){
					movement = Ai_Movement(FollowingAi.position,followSpeed);
				} else {
					moveState = MOVEMENT_STATE.IsIdle;
				}
				Debug.DrawLine(transform.position, FollowingAi.position, Color.magenta);
				
			// Checks if following other ai is enabled and if a tier two ai has been found	
			} else if(_CanFollowAi && this.Ai_FindAiTierTwo()){
				Debug.Log ("Following ai tier 2 " + FollowingAi);
				_HasWanderPos = false; // TODO: this needs to be fixed
				visionState = VISION_STATE.CanSeeFollowAiTierTwo;
				moveState = MOVEMENT_STATE.IsFollowingAiTierTwo;
				attackState = ATTACK_STATE.CanNotAttack;
				if(Vector3.Distance(transform.position, FollowingAi.position) > otherAiDistance){
					movement = Ai_Movement(FollowingAi.position,followSpeed);
				} else {
					moveState = MOVEMENT_STATE.IsIdle;
				}
				Debug.DrawLine(transform.position, FollowingAi.position, Color.white);
				
			// Checks if wandering is enabled and if the timer has reached its limit
			} else if(_CanWander && wanderTimer < wanderTimeLimit) {
				Debug.Log ("Wandering");
				visionState = VISION_STATE.CanSeeNothing;
				attackState = ATTACK_STATE.CanNotAttack;
				movement = Ai_Wander ();
				
			// Checks if patrolling is enabled and a waypoing has been found
			} else if(_CanPatrol && this.Ai_FindWaypoint()) {
				Debug.Log ("Patroling " + Waypoint);
				_HasWanderPos = false; // TODO: this needs to be fixed
				visionState = VISION_STATE.CanSeeWaypoint;
				moveState = MOVEMENT_STATE.IsPatrolling;
				attackState = ATTACK_STATE.CanNotAttack;
				movement = Ai_Movement(Waypoint.position, patrolSpeed);
				Debug.DrawLine(transform.position, Waypoint.position, Color.yellow);
			
			// Nothing is found, reset all variables
			} else {
				Debug.Log ("Reseting");
				Ai_Reset();
			}

//			if (!GetComponent<CharacterController> ().isGrounded) {
//				movement.y = -10f;
//			}
//			GetComponent<CharacterController> ().Move (movement);
//
//			movement.y = 0;
//			if (Target != null) {
//				transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (Target.position - transform.position), Time.fixedDeltaTime * rotationSpeed);
//			} else if (movement.magnitude > 0) {
//				transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (movement), Time.fixedDeltaTime * rotationSpeed);
//			}
		}
		
		private void Ai_Reset(){
			Player = null;
			FollowingPlayer = null;
			Breadcrumb = null;
			FollowingAi = null;
			Waypoint = null;
			wanderTimer = 0;
			moveState = MOVEMENT_STATE.IsIdle;
			attackState = ATTACK_STATE.CanNotAttack;
		}
		

		// Move the rigidbody forward based on the speed value 
		private Vector3 Ai_Movement(Vector3 position, float speed){
			Vector3 movement = Vector3.zero;
			if(_CanJump && CanJump()){
				if(moveState == MOVEMENT_STATE.IsFollowingPlayer || 
					moveState == MOVEMENT_STATE.IsFollowingAi ||
					moveState == MOVEMENT_STATE.IsFollowingAiTierTwo ||
					moveState == MOVEMENT_STATE.IsFollowingTarget){
					Debug.Log ("Jumping");
					Ai_Jump();
				}
			}
			if(Ai_EdgeAvoidance() && !_IsJumping){
				Debug.Log ("Moving to " + position + " at " + speed);
				GetComponent<Rigidbody>().AddForce(transform.forward * Time.fixedDeltaTime * speed);
//				movement = (position - transform.position).normalized * Time.fixedDeltaTime * speed;
			} else if(_CanLongJump){
				if(moveState == MOVEMENT_STATE.IsFollowingPlayer || 
				   moveState == MOVEMENT_STATE.IsFollowingAi ||
					moveState == MOVEMENT_STATE.IsFollowingAiTierTwo ||
					moveState == MOVEMENT_STATE.IsFollowingTarget){
					Debug.Log ("Long Jumping");
					Ai_LongJump();
				}
			}
			Ai_Rotation(position);

			return movement;
		}
		
		// Rotate the Ai to look towards it's target at a set Rotation speed
		private void Ai_Rotation(Vector3 position){
			Vector3 playerPos = Vector3.zero;
			if(_IsGround){
				playerPos = new Vector3(position.x,transform.position.y,position.z); // Adjust Y position so Ai doesn't rotate up/down
			} else if(_IsAir){
				playerPos = new Vector3(position.x,position.y,position.z);
			}
			GetComponent<Rigidbody>().MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerPos - transform.position, Vector3.up), rotationSpeed));
		}
		
		private Vector3 Ai_Flee(){
			GetComponent<Rigidbody>().MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(transform.position - Player.position, Vector3.up), rotationSpeed));
			Vector3 movement = Vector3.zero;
			if(Ai_EdgeAvoidance()){
				GetComponent<Rigidbody>().AddForce(transform.forward * Time.fixedDeltaTime * followSpeed);
				movement = (transform.position - Target.position).normalized * Time.fixedDeltaTime * followSpeed;
			}
			return movement;
		}

		private Vector3 Ai_Flee_Target(){
			Debug.Log ("Fleeing " + Target);
			Vector3 movement = Vector3.zero;
			GetComponent<Rigidbody>().MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(transform.position - Target.position, Vector3.up), rotationSpeed));
			if(Ai_EdgeAvoidance()){
				GetComponent<Rigidbody>().AddForce(transform.forward * Time.fixedDeltaTime * followSpeed);
				movement = (transform.position - Target.position).normalized * Time.fixedDeltaTime * followSpeed;
			}
			return movement;
		}
		
		private void Ai_Jump(){
			if(IsGrounded() && !_IsJumping){
				GetComponent<Rigidbody>().AddForce((Vector3.up * jumpForce) + (transform.forward * (jumpForce/2)), ForceMode.VelocityChange);
				Vector3 movement = (Vector3.up * jumpForce) + (transform.forward * (jumpForce / 2));

				_IsJumping = true;
			}
		}
		
		private void Ai_LongJump(){
			if(IsGrounded() && !_IsJumping){
				if(Physics.Linecast(LongJumpDetector.position, LongJumpDetector.position + (-Vector3.up * edgeDistance))){
					GetComponent<Rigidbody>().AddForce((Vector3.up + transform.forward) * longJumpForce, ForceMode.VelocityChange );

					Vector3 movement = (Target.position - transform.position).normalized * Time.fixedDeltaTime * followSpeed;

					_IsJumping = true;
				}
				Debug.DrawLine(LongJumpDetector.position,LongJumpDetector.position + (-Vector3.up * edgeDistance));
			}
		}

		// This wander function selects a random location around the Ai and moves towards it.
		// This will be update in the future to allow specific wander radius rather than "anywhere"		
		private Vector3 Ai_Wander(){
			Vector3 movement = Vector3.zero;
			wanderTimer += Time.fixedDeltaTime;
			if(wanderTimer >= wanderTimeLimit){
				_IsWandering = false;
            } else {
            	_IsWandering = true;
            }
            
            if(_CanWanderAnywhere){
				currentWanderPos = transform.position;
			} else {
				if(!_HasWanderPos){
					currentWanderPos = transform.position;
					_HasWanderPos = true;
				}	
			}
            
            if(_IsWandering){
	            if(Time.time > wanderNext){
					wanderNext = Time.time + wanderTimeRate;
					float wanderX = Random.Range(currentWanderPos.x - wanderDistance, currentWanderPos.x + wanderDistance);
					float wanderZ = Random.Range(currentWanderPos.z - wanderDistance, currentWanderPos.z + wanderDistance);
					wanderPos = new Vector3(wanderX,currentWanderPos.y,wanderZ);
				}
				if(Vector3.Distance(transform.position, wanderPos) > 1f){
					movement = Ai_Movement(wanderPos, wanderSpeed);
					moveState = MOVEMENT_STATE.IsWandering;
				} else {
					moveState = MOVEMENT_STATE.IsIdle;
				}
			}
			return movement;
		}

		// Avoidance casts a ray around the Ai so that it can bounce of walls and other obstacles
		// Velocity is set to zero so that when the AddForce is no longer being applied it will stop the Ai from sliding around
		private void Ai_Avoidance(LayerMask Layer){
			if(_HasAvoidance){
				if (Physics.SphereCast(transform.position, 0.5f, transform.forward, out hit, avoidDistance, Layer)){
					Debug.DrawLine(transform.position, hit.point, Color.cyan);
					Vector3 direction = transform.position - hit.point;
					direction.y = 0;
					GetComponent<Rigidbody>().AddForce(direction * avoidSpeed);
					Debug.DrawRay(transform.position, direction * avoidSpeed, Color.yellow);
					_IsAvoiding = true;
				} 
				if (Physics.Raycast(transform.position,transform.forward,out hit,avoidDistance, Layer)){
					Debug.DrawLine(transform.position, hit.point, Color.cyan);
					GetComponent<Rigidbody>().AddForce(-transform.forward * avoidSpeed);
					Debug.DrawRay(transform.position, -transform.forward * avoidSpeed, Color.yellow);
					_IsAvoiding = true;
				} 


				if (Physics.Raycast(transform.position,-transform.right,out hit,avoidDistance, Layer)){
					Debug.DrawLine(transform.position, hit.point, Color.cyan);
					GetComponent<Rigidbody>().AddForce(transform.right * avoidSpeed);
					Debug.DrawRay(transform.position, transform.right * avoidSpeed, Color.yellow);
					_IsAvoiding = true;
				} 
				if (Physics.Raycast(transform.position,transform.right,out hit,avoidDistance, Layer)){
					Debug.DrawLine(transform.position, hit.point, Color.cyan);
					GetComponent<Rigidbody>().AddForce(-transform.right * avoidSpeed);
					Debug.DrawRay(transform.position, -transform.right * avoidSpeed, Color.yellow);
					_IsAvoiding = true;
				} 
				if (Physics.Raycast(transform.position,transform.forward + -transform.right *2,out hit,avoidDistance, Layer)){
					Debug.DrawLine(transform.position, hit.point, Color.cyan);
					GetComponent<Rigidbody>().AddForce(transform.right * avoidSpeed);
					Debug.DrawRay(transform.position, transform.right * avoidSpeed, Color.yellow);
					_IsAvoiding = true;	
				} 
				if (Physics.Raycast(transform.position,transform.forward + transform.right * 2,out hit,avoidDistance, Layer)){
					Debug.DrawLine(transform.position, hit.point, Color.cyan);
					GetComponent<Rigidbody>().AddForce(-transform.right * avoidSpeed);
					Debug.DrawRay(transform.position, -transform.right * avoidSpeed, Color.yellow);
					_IsAvoiding = true;
				} 
				if (Physics.Raycast(transform.position,-transform.forward,out hit,avoidDistance, Layer)){
					Debug.DrawLine(transform.position, hit.point, Color.cyan);
					GetComponent<Rigidbody>().AddForce(transform.forward * avoidSpeed);
					Debug.DrawRay(transform.position, transform.right * avoidSpeed, Color.yellow);
					_IsAvoiding = true;
				} 
				
				// This raycast helps avoid other Ai that are directly infront
				if(Physics.Raycast(transform.position,transform.forward, out hit, transform.GetComponent<Collider>().bounds.extents.z + 0.1f)){
					if(hit.collider.tag == AiManager.enemyString){
						GetComponent<Rigidbody>().AddForce(transform.right * avoidSpeed);
						_IsAvoiding = true;
					}
				} 
				if(_IsAvoiding){
					GetComponent<Rigidbody>().velocity = Vector3.zero;
					_IsAvoiding = false;
				}
			}
		}
		
		// Self Harm Avoidance casts a ray to see if there's ground infront of the Ai, if there's no ground return false
		private bool Ai_EdgeAvoidance(){
			if(_HasEdgeAvoidance && _HasAvoidance){
				Debug.DrawLine(Edge.position, Edge.position + -Edge.up * edgeDistance);
				return Physics.Raycast(Edge.position,-Edge.up,edgeDistance);
			} else {
				return true;
			}
		}
		
		// We simply check to see if this Ai is invincible, if so then the lifestate is set to IsInvincible.
		// Otherwise check to see if the Health is equal or lower to 0 before setting to IsDead state.
		private void Ai_LifeState(){
			if(_IsInvincible){
				lifeState = LIFE_STATE.IsInvincible;
			} else {
				if(Health <= 0.0f){
					lifeState = LIFE_STATE.IsDead;
				}
			}
		}
		
		// Checks if a position is within range if vision is enabled
		public bool InRange(Vector3 position, float vision){
			if(_HasVision){
				Vector3 pos1 = transform.position;
				pos1.y = 0;
				Vector3 pos2 = position;
				pos2.y = 0;
				if(Vector3.Distance(pos1, pos2) < vision){
					if(_HasFrontVision){
						float visionAngle = Vector3.Dot(position - transform.position, transform.forward);
						if(visionAngle > 0){ return true;
						} else { return false; }
					} else { return true;}
				} else { return false; } 
			} else { return true; }
		}
		
		public bool CanJump(){
			return Physics.Raycast(JumpDetector.position, JumpDetector.forward, jumpDistance, obstacleLayer);
		}
		
		// This checks if the Ai is grounded, collider is required on the GameObject that has this script
		// TODO: add customizable collider in case users have different collider gameobject.
		public bool IsGrounded(){
			if(GetComponent<Collider>() != null){
				return Physics.Raycast(transform.position, -Vector3.up, GetComponent<Collider>().bounds.extents.y + 0.1f);
			} else {
				return true;
			}
		}
		
		// Still in testing, this is used to make your "ground" unity hover slightly, or hover a lot, it's customizable.
		private void Ai_Hover(){
			if(_CanHover){
				Ray hoverRay = new Ray(transform.position, -transform.up);
				RaycastHit hit;
				
				if(Physics.Raycast(hoverRay, out hit, hoverHeight)){
					float proportionalHeight = (hoverHeight - hit.distance) / hoverHeight;
					Vector3 appliedHoverForce = Vector3.up * proportionalHeight * hoverForce;
					GetComponent<Rigidbody>().AddForce(appliedHoverForce, ForceMode.Acceleration);
					Debug.DrawLine(Hover.position, hit.point, Color.blue);
				}
			}
		}

		public void SetIsEnemy(bool isEnemy) {
			_IsEnemy = isEnemy;
			
			if ((isEnemy && gameObject.tag != AiManager.enemyString) ||
				(!isEnemy && gameObject.tag != AiManager.friendlyString)
			) {
				Target = null;

				if (isEnemy) {
					Debug.Log ("Setting tag to " + AiManager.enemyString);
					gameObject.tag = AiManager.enemyString;
					SetLayerRecursively(gameObject, LayerMask.NameToLayer(AiManager.enemyString));
				} else {
					Debug.Log ("Setting tag to " + AiManager.friendlyString);
					gameObject.tag = AiManager.friendlyString;
					SetLayerRecursively(gameObject, LayerMask.NameToLayer(AiManager.friendlyString));
				}
			}
		}

		public void SetLayerRecursively(GameObject obj, int newLayer){
			obj.layer = newLayer;

			foreach(Transform child in obj.transform ) {
				SetLayerRecursively( child.gameObject, newLayer );
			}
		}
	}
}