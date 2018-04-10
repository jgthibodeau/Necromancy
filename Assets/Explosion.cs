using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof (NetworkTransform))]
public class Explosion : NetworkBehaviour {
	public float radius = 1f;
	public float forceScale = 1f;
	public ForceMode forceMode = ForceMode.Impulse;
	public float damage = 1f;
	public bool scaleForceByDistance = true;
	public bool scaleDamageByDistance = true;
	public bool splashEffects = true;
	public bool splashDamage = true;

	public List<GameObject> ignoreForDamage = new List<GameObject> ();

	void Start(){
		Collider[] colliders = Physics.OverlapSphere (transform.position, radius);
		HashSet<Rigidbody> rigidbodies = new HashSet<Rigidbody>();
		HashSet<IHittable> hittables = new HashSet<IHittable>();
		foreach (Collider collider in colliders) {
			float explosionPercent;

			//for each unique rigidbody
			Rigidbody rigidBody = collider.GetComponentInParent<Rigidbody> ();
			if (rigidBody != null && !rigidbodies.Contains (rigidBody)) {
				rigidbodies.Add (rigidBody);
				//add force based on distance and away from explosion point
				//					Vector3 center = rigidBody.position;
				Vector3 center = rigidBody.transform.TransformPoint (rigidBody.centerOfMass);
				Vector3 direction = center - transform.position;
				if (scaleForceByDistance) {
					explosionPercent = 1 - (Vector3.Magnitude (direction) / radius);
				} else {
					explosionPercent = 1f;
				}
				if (explosionPercent >= 0) {
					Vector3 explosionForce = direction.normalized * forceScale;
					if (scaleForceByDistance) {
						explosionForce *= explosionPercent;
					}
//					Debug.Log ("adding force to rigidbody " + rigidBody + " " + explosionForce);
					rigidBody.AddForce (explosionForce, forceMode);
				}
			}

			if (splashEffects) {
				if (!ignoreForDamage.Contains (collider.gameObject)) {
					IHittable hittable = collider.gameObject.GetComponent<IHittable> ();
					if (hittable != null && !hittables.Contains (hittable)) {
						hittables.Add (hittable);

						float thisDamage = 0;
						if (splashDamage) {
							if (scaleDamageByDistance) {
								Vector3 center = collider.transform.position;
								Vector3 direction = center - transform.position;
								explosionPercent = 1 - (Vector3.Magnitude (direction) / radius);
							} else {
								explosionPercent = 1;
							}
							thisDamage = damage * explosionPercent;
						}

						hittable.Hit (thisDamage);
					}
				}
			}
		}
	}
}
