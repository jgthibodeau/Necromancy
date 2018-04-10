using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BreadcrumbAi;

[System.Serializable]
public class EnemySounds{
	public AudioClip audio_hit_1, audio_hit_2, audio_dead_1, audio_dead_2, audio_melee_attack_1, audio_melee_attack_2;
}

[RequireComponent(typeof(Ai))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour {
	public EnemySounds audioClips;
	private Ai ai;
	private Health health;
	private Rigidbody rigidBody;

	public enum EnemyType {Melee, Ranged, Special};
	public EnemyType enemyType;
//	public GameObject healthPickUpPrefab;
//	public bool _canDropPickUp;
	public Rigidbody rangedProjectilePrefab;
	public GameObject bloodPrefab;
	private Transform target;
	public Animator anim;
	private string animRun = "Run";
	private string animDeath1 = "Death1";
	private string animAttack = "Attack";
	private int attackStateHash = Animator.StringToHash("Base Layer.Attack");
	private AudioSource audioSource;
	private bool _removeBody, _isHit, _animAttack;

	public float standupSpeed = 5f;

	private float rangedAttackNext = 0.0f;
	public float rangedAttackRate = 2.0f;

	private float meleeAttackNext = 0.0f;
	public float meleeAttackRate = 1.0f;
	public float meleeDamage = 10.0f;

	public float gravity = 10f;

	public Transform ragdollRoot;
	public RagdollHelper ragdollHelper;
	public Collider normalCollider;
	public Collider ragdollCollider;

	// Use this for initialization
	void Start () {
		ai = GetComponent<BreadcrumbAi.Ai> ();
		health = GetComponent<Health> ();
		rigidBody = GetComponent<Rigidbody> ();

//		anim = GetComponent<Animator>();
		audioSource = gameObject.AddComponent<AudioSource>();
//		GameObject go = GameObject.FindGameObjectWithTag("Player");
//		if(go){
//			target = go.transform;
//		}
	}

	void FixedUpdate(){
		GetComponent<Rigidbody> ().AddForce (Vector3.down * gravity);
		Animation();
		Attack();
	}

	// Update is called once per frame
	void Update () {
		//		CheckHealth();
		ResetTargetAndTag ();

		if (health.IsDead ()) {
			ragdollHelper.ragdolled = true;
			ai.enabled = false;
//			rigidBody.freezeRotation = false;
			Vector3 ragdollPosition = ragdollRoot.position;
			Vector3 position = new Vector3(ragdollPosition.x, transform.position.y, ragdollPosition.z);
			transform.position = position;
			ragdollRoot.position = ragdollPosition;
			rigidBody.constraints = RigidbodyConstraints.FreezeAll;

//			GetComponent<Collider> ().isTrigger = true;
			normalCollider.enabled = false;
			ragdollCollider.enabled = true;
		} else {
//			if (Vector3.Angle (transform.up, Vector3.up) > 5f) {
//				Quaternion desiredRotation = Quaternion.LookRotation (transform.forward, Vector3.up);
//				transform.rotation = Quaternion.Slerp (transform.rotation, desiredRotation, standupSpeed * Time.deltaTime);
//			} else {
//				Quaternion desiredRotation = Quaternion.LookRotation (transform.forward, Vector3.up);
//				transform.rotation = desiredRotation;

				ragdollHelper.ragdolled = false;
				ai.enabled = true;
				rigidBody.constraints = RigidbodyConstraints.FreezeRotation;

//				GetComponent<Collider> ().isTrigger = false;
				normalCollider.enabled = true;
				ragdollCollider.enabled = false;
//			}
		}
	}

	private void Animation(){
		if(!health.IsDead()){
//			ai.lifeState = Ai.LIFE_STATE.IsDead;

			bool run = (ai.moveState != Ai.MOVEMENT_STATE.IsIdle && ai.moveState != Ai.MOVEMENT_STATE.IsAttacking);
			anim.SetBool(animRun, run);

			if(_animAttack){
				anim.SetTrigger(animAttack);
			}
		} else {
			ai.lifeState = Ai.LIFE_STATE.IsDead;
			anim.SetBool(animRun, false);
			anim.SetBool(animAttack, false);
			anim.SetBool(animDeath1, true);
		}
	}

	private void ResetTargetAndTag(){
		if (health.state == Health.State.Reanimated) {
			ai.SetIsEnemy (false);
		} else if (health.state == Health.State.Alive) {
			ai.SetIsEnemy (true);
		} else if (health.state == Health.State.Dead) {
			gameObject.tag = "Untagged";
			ai.SetLayerRecursively(gameObject, 0);
			ai.Target = null;
		}

		if (ai.Target != null && ai.Target.GetComponent<Health> ().IsDead ()) {
			ai.Target = null;
		}
	}

	public bool IsAttacking() {
		AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
		return info.nameHash == attackStateHash;
	}

	private void Attack(){
		if (IsAttacking ()) {
			ai.moveState = Ai.MOVEMENT_STATE.IsAttacking;
			return;
		} else if (ai.moveState == Ai.MOVEMENT_STATE.IsAttacking) {
			ai.moveState = Ai.MOVEMENT_STATE.IsFollowingTarget;
		}

		if(ai.Target != null && !health.IsDead()){
//			if(enemyType != EnemyType.Ranged){
				if(ai.attackState == Ai.ATTACK_STATE.CanAttack && Time.time > meleeAttackNext){
					meleeAttackNext = Time.time + meleeAttackRate;
					float rand = Random.value;
					if(rand <= 0.4f){
						audioSource.clip = audioClips.audio_melee_attack_1;
					} else {
						audioSource.clip = audioClips.audio_melee_attack_2;
					}
					audioSource.PlayOneShot(audioSource.clip);

					_animAttack = true;

				} else {
					_animAttack = false;
				}
//			} else {
//				if(ai.attackState == Ai.ATTACK_STATE.CanAttack && Time.time > rangedAttackNext){
//					rangedAttackNext = Time.time + rangedAttackRate;
//					Rigidbody spit = Instantiate(rangedProjectilePrefab, transform.position + transform.forward + transform.up, transform.rotation) as Rigidbody;
//					spit.AddForce(transform.forward * 500);
//					_animAttack = true;
//				} else {
//					_animAttack = false;
//				}
//			}
		}
	}

	private void CheckHealth(){
		if(_isHit && this != null){
			float rand = Random.value;
			if(ai.Health > 0){
				if(rand > 0.5f){
					if(rand < 0.7f){
						audioSource.clip = audioClips.audio_hit_2;
					} else {
						audioSource.clip = audioClips.audio_hit_1;
					}
					audioSource.PlayOneShot(audioSource.clip);
				}
			}
			if(ai.Health <= 0){
				if(rand > 0.5f){
					audioSource.clip = audioClips.audio_dead_1;
				} else {
					audioSource.clip = audioClips.audio_dead_2;
				}
				audioSource.PlayOneShot(audioSource.clip);
			}
			_isHit = false;
		}
	}
}
