using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour {
	public Animator animator;
	public AnimationAttackTrigger trigger;
	public float damage;

	private int attackStateHash = Animator.StringToHash("Base Layer.Attack");

	private Vector3 initialPosition;
	private Quaternion initialRotation;

	Health parentHealth;

	List<Health> alreadyHit = new List<Health> ();

	void Start() {
		initialPosition = transform.localPosition;
		initialRotation = transform.localRotation;

		Collider col = GetComponent<Collider> ();
		foreach (Collider c in GetComponentsInParent<Collider>()) {
			Physics.IgnoreCollision (col, c);
		}
	}

	void Update() {
		transform.localPosition = initialPosition;
		transform.localRotation = initialRotation;

		parentHealth = GetComponentInParent<Health> ();

		if (IsAttacking()) {
			Debug.DrawRay (transform.position, transform.forward * 5, Color.red);
		} else if (alreadyHit.Count > 0) {
			alreadyHit.Clear ();
		}
	}

	public bool IsAttacking() {
		AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
		return info.nameHash == attackStateHash && trigger.isAttacking;
	}

	void OnTriggerEnter(Collider other) {
		Debug.Log ("Hit " + other);
		if (IsAttacking() && other.gameObject.tag != "Weapon") {
			Health otherHealth = other.gameObject.GetComponentInParent<Health> ();
			Debug.Log ("Hit health " + otherHealth);
			if (otherHealth != null && otherHealth != parentHealth && !alreadyHit.Contains(otherHealth)) {
				alreadyHit.Add (otherHealth);
				otherHealth.Hit (damage);
			}
		}
	}
}
